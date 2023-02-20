using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FWI;
using static System.Net.Mime.MediaTypeNames;
#nullable enable

namespace FWI.Prompt
{
    public class Prompt
    {
        private static readonly object _Lock = new object();
        readonly PromptExecuter executer;
        readonly Stack<IOutputStream> outputStreamStack;
        public CommandDelegate? OnNotExecuted { get; set; }
        public bool UnhandleException { get; set; }
        public IOutputStream DefaultOutputStream { get; set; }
        public IOutputStream OutputStream { get; set; }

        public Prompt()
        {
            outputStreamStack = new Stack<IOutputStream>();
            executer = new PromptExecuter();

            OutputStream = NullOutputStream.Instance;
            DefaultOutputStream = new StandardOutputStream();
            OnNotExecuted = (args, output) => output.WriteLine("Unknown Command");
            UnhandleException = false;
        }
        
        public Task LoopAsync(IInputStream inputStream, IOutputStream outputStream)
        {
            var task = new Task(() => Loop(inputStream, outputStream));
            task.Start();

            return task;
        }

        public void Loop(IInputStream inputStream, IOutputStream outputStream)
        {
            outputStream ??= DefaultOutputStream;
            outputStreamStack.Push(outputStream);
            try
            {
                while (true)
                {
                    outputStream.Write("$ ");
                    outputStream.Flush();
                    var cmd = inputStream?.Read() ?? "";
                    if (cmd == "") continue;
                    else Execute(cmd, outputStream);
                }
            }
            finally
            {
                outputStreamStack.Pop();
            }
        }

        public void Execute(string cmd, IOutputStream? outputStream = null)
        {
            var args = new PromptArgs(cmd);

            lock(_Lock)
            {
                var output = outputStream ?? DefaultOutputStream;
                outputStreamStack.Push(output);

                try
                {
                    var executed = executer.Execute(args, output);
                    if (!executed) OnNotExecuted?.Invoke(args, output);

                    output?.Flush();
                }
                catch (Exception e)
                {
                    if (UnhandleException)
                    {
                        throw e;
                    }
                    else
                    {
                        output.WriteLine("[D][W] Prompt 명령 실행중 오류가 발생했습니다.");
                        output.WriteLine($"-------------------------------");
                        output.WriteLine($"{e}");
                        output.WriteLine($"-------------------------------");
                        output.Flush();
                    }
                }
                finally
                {
                    outputStreamStack.Pop();
                }
            }
        }

        public bool Add(string text, CommandDelegate action)
        {
            var commands = text.Split(' ').ToQueue();

            return executer.AddCommand(commands, action);
        }

        public void AddRedirect(string text, string redirectTo)
        {
            Add(text, (args, output) => {
                if (args.Count > 0) Execute($"{redirectTo} {args.GetArgs()}", output);
                else Execute(redirectTo, output);
            });
        }

        public List<string> GetCommands(string cmd = "")
        {
            if (cmd.Length == 0)
            {
                return executer.GetCommands();
            }
            else
            {
                var queue = cmd.Split(' ').ToQueue();
                return executer.GetCommands(queue);
            }
        }

        public List<string> GetCommands(Queue<string> queue)
        {
            return executer.GetCommands(queue);
        }

        public IOutputStream Out => outputStreamStack.LastOrDefault();
    }
}
