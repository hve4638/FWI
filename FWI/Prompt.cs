using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FWI
{
    public class Prompt
    {
        readonly Dictionary<string, Action<ArraySegment<string>>> commands;
        Action<ArraySegment<string>> commandDefault;
        public Prompt()
        {
            commands = new Dictionary<string, Action<ArraySegment<string>>>();
            commandDefault = (_) => {
                Console.WriteLine("Unknown Command");
            };

            Add("help", (_) => {
                foreach(var cmd in GetCommandList())
                {
                    Console.WriteLine($"- {cmd}");
                }
            });
        }

        public Thread RunAsync()
        {
            var thread = new Thread(Run);
            thread.Start();

            return thread;
        }
        public void Run()
        {
            string[] cmdArray;
            string cmd;
            ArraySegment<string> args;
            while (true)
            {
                Console.Write("$ ");
                cmdArray = Console.ReadLine()?.Split(' ');
                if (cmdArray == null || cmdArray[0] == "") continue;
                else
                {
                    cmd = cmdArray[0];
                    args = new ArraySegment<string>(cmdArray, 1, cmdArray.Length - 1);

                    if (commands.ContainsKey(cmd))
                    {
                        var run = commands[cmd];
                        try
                        {
                            run(args);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Prompt 명령 실행중 오류가 발생했습니다.");
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        commandDefault(args);
                    }
                }
            }
        }

        public void Add(string cmd, Action<ArraySegment<string>> action)
        {
            if (commands.ContainsKey(cmd)) commands.Remove(cmd);
            
            commands.Add(cmd, action);
        }
        public void AddDefault(Action<ArraySegment<string>> action)
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

        static public Prompt operator +(Prompt s, Tuple<string, Action<ArraySegment<string>>> items)
        {
            s.Add(items.Item1, items.Item2);

            return s;
        }
    }
}
