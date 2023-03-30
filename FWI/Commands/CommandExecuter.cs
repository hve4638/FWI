using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace FWI.Commands
{
    public class CommandExecuter
    {
        readonly Dictionary<string, CommandExecuter> dict;
        CommandDelegate? OnExecute { get; set; }

        public CommandExecuter()
        {
            dict = new Dictionary<string, CommandExecuter>();
        }

        public bool Execute(CommandArgs args, IOutputStream output)
        {
            if (dict.TryGetValue(args.CommandLastWord, out var executer))
            {
                return executer.Execute(args.Slice(1), output);
            }
            else if (OnExecute is null)
            {
                return false;
            }
            else
            {
                args = args.Slice(-1);
                OnExecute(args, output);
                return true;
            }
        }

        public bool AddCommand(Queue<string> queue, CommandDelegate command)
        {
            var executer = FindExecuter(queue);
            if (executer.OnExecute is null)
            {
                executer.OnExecute = command;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddCommandForce(Queue<string> queue, CommandDelegate command)
        {
            var executer = FindExecuter(queue);

            executer.OnExecute = command;
        }

        public CommandExecuter FindExecuter(Queue<string> queue)
        {
            if (queue.Count == 0) return this;
            else 
            {
                var keyword = queue.Dequeue();
                if (!dict.TryGetValue(keyword, out CommandExecuter executer))
                {
                    executer = new CommandExecuter();

                    dict.Add(keyword, executer);
                }

                return executer.FindExecuter(queue);
            }
        }

        public List<string> GetCommands()
        {
            var list = new List<string>();
            foreach (var key in dict.Keys) list.Add(key);
            return list;
        }

        public List<string> GetCommands(Queue<string> queue)
        {
            if (queue.Count == 0) return GetCommands();

            var key = queue.Dequeue();

            if (dict.TryGetValue(key, out var child)) return child.GetCommandsWithPrefix(queue, key);
            else return new List<string>();
        }

        List<string> GetCommandsWithPrefix(string prefix)
        {
            var list = new List<string>();
            foreach(var key in dict.Keys) list.Add($"{prefix} {key}");
            return list;
        }

        List<string> GetCommandsWithPrefix(Queue<string> queue, string prefix)
        {
            if (queue.Count == 0) return GetCommandsWithPrefix(prefix);

            var key = queue.Dequeue();
            if (!dict.TryGetValue(key, out var child)) return new List<string>();

            return child.GetCommandsWithPrefix(queue, $"{prefix} {key}");
        }
    }
}
