using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FWI;

namespace FWI.Prompt
{
    public class Prompt
    {
        private static readonly object _Lock = new object();
        readonly Dictionary<string, Action<PromptArgs, IOutputStream>> commands;
        readonly Stack<IOutputStream> outputStreamStack;
        Action<PromptArgs, IOutputStream> commandDefault;
        public bool UnhandleException { get; set; }
        public IOutputStream DefaultOutputStream { get; set; }

        public Prompt()
        {
            outputStreamStack = new Stack<IOutputStream>();
            commands = new Dictionary<string, Action<PromptArgs, IOutputStream>>();
            DefaultOutputStream = new StandardOutputStream();
            UnhandleException = false;

            commandDefault = (args, output) => output.WriteLine("Unknown Command");
            Add("help", (args, output) => {
                foreach(var cmd in GetCommandList())
                {
                    output.WriteLine($"- {cmd}");
                }
            });
        }
        
        public Thread LoopAsync(IInputStream inputStream = null, IOutputStream outputStream = null)
        {
            var thread = new Thread(() => { Loop(inputStream); });
            thread.Start();

            return thread;
        }

        public void Loop(IInputStream inputStream = null, IOutputStream outputStream = null)
        {
            outputStream = outputStream ?? DefaultOutputStream;
            outputStreamStack.Push(outputStream);
            try
            {
                while (true)
                {
                    outputStream.Write("$ ");
                    outputStream.Flush();
                    var cmd = inputStream.Read();
                    if (cmd is null || cmd == "") continue;
                    else Execute(cmd, outputStream);
                }
            }
            finally
            {
                outputStreamStack.Pop();
            }
        }

        public void Execute(string cmd, IOutputStream outputStream = null)
        {
            var args = new PromptArgs(cmd.Split(' '));
            var first = args.Command;

            Action<PromptArgs, IOutputStream> action;
            if (commands.ContainsKey(first)) action = commands[first];
            else
            {
                action = commandDefault;
            }

            lock(_Lock)
            {
                var currentOutputStream = outputStream ?? DefaultOutputStream;
                outputStreamStack.Push(currentOutputStream);

                try
                {
                    action(args, currentOutputStream);
                    Out?.Flush();
                }
                catch (Exception e)
                {
                    if (UnhandleException)
                    {
                        throw e;
                    }
                    else
                    {
                        Out.WriteLine("[D][W] Prompt 명령 실행중 오류가 발생했습니다.");
                        Out.WriteLine($"-------------------------------");
                        Out.WriteLine($"{e}");
                        Out.WriteLine($"-------------------------------");
                        Out.Flush();
                    }
                }
                finally
                {
                    outputStreamStack.Pop();
                }
            }
        }

        public void Add(string cmd, string redirectTo)
        {
            if (cmd.Split(' ').Length == 1)
            {
                Add(cmd, (args, output) =>
                {
                    Execute(redirectTo, output);
                });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Add(string cmd, Action<PromptArgs, IOutputStream> action)
        {
            if (commands.ContainsKey(cmd)) commands.Remove(cmd);

            commands.Add(cmd, action);
        }
        public void AddDefault(Action<PromptArgs, IOutputStream> action)
        {
            commandDefault = action;
        }

        public List<string> GetCommandList()
        {
            var list = new List<string>();
            foreach(var cmd in commands.Keys)
            {
                list.Add(cmd);
            }
            return list;
        }

        public IOutputStream Out => outputStreamStack.LastOrDefault();
    }
}
