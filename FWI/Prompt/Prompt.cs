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
        private static object _Lock = new object();

        IOutputStream currentOutputStream;
        readonly Dictionary<string, Action<PromptArgs, IOutputStream>> commands;
        Action<PromptArgs, IOutputStream> commandDefault;
        public Prompt()
        {
            commands = new Dictionary<string, Action<PromptArgs, IOutputStream>>();
            commandDefault = (args, output) => {
                output.WriteLine("Unknown Command");
            };

            DefaultOutputStream = new StandardOutputStream();

            Add("help", (_) => {
                foreach(var cmd in GetCommandList())
                {
                    Out.WriteLine($"- {cmd}");
                }
            });
        }

        public IOutputStream DefaultOutputStream { get; set; }

        public Thread LoopAsync()
        {
            var thread = new Thread(Loop);
            thread.Start();

            return thread;
        }

        public void Loop()
        {
            while (true)
            {
                Console.Write("$ ");
                var cmd = Console.ReadLine();
                if (cmd is null || cmd == "") continue;
                else Execute(cmd);
            }
        }

        public void Execute(string cmd, IOutputStream outputStream = null)
        {
            var args = new PromptArgs(cmd.Split(' '));
            var first = args.GetCMD();

            Action<PromptArgs, IOutputStream> action;
            if (commands.ContainsKey(first)) action = commands[first];
            else action = commandDefault;

            lock(_Lock)
            {
                currentOutputStream = outputStream ?? DefaultOutputStream;

                try
                {
                    action(args, currentOutputStream);
                    Out.Flush();
                }
                catch (Exception e)
                {
                    Out.WriteLine("[D][W] Prompt 명령 실행중 오류가 발생했습니다.");
                    Out.WriteLine($"-------------------------------");
                    Out.WriteLine($"{e}");
                    Out.WriteLine($"-------------------------------");
                    Out.Flush();
                }
                finally
                {
                    currentOutputStream = null;
                }
            }
        }

        public void Add(string cmd, Action action)
        {
            Add(cmd, (args, output) => { action(); });
        }

        public void Add(string cmd, Action<PromptArgs> action)
        {
            Add(cmd, (args, output) => action(args));
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

        public IOutputStream Out => currentOutputStream;
    }
}
