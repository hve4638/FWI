using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace FWI.Prompt
{
    public class PromptExecuter
    {
        readonly Dictionary<string, PromptExecuter> dict;
        CommandDelegate? OnExecute { get; set; }
        readonly string keyword;

        public PromptExecuter(string keyword = "")
        {
            dict = new Dictionary<string, PromptExecuter>();
            this.keyword = keyword;
        }

        public void Execute(PromptArgs args, IOutputStream output)
        {
            if (dict.TryGetValue(args.Command, out var executer))
            {
                executer.Execute(args.Slice(1), output);
            }
            else
            {
                OnExecute?.Invoke(args, output);
            }
        }

        public bool AddCommand(Queue<string> queue, CommandDelegate command)
        {
            if (queue.Count > 0)
            {
                var keyword = $"{this.keyword} {queue.Dequeue()}".Trim();

                if (dict.ContainsKey(keyword)) return false;
                var executer = new PromptExecuter(keyword);

                dict.Add(keyword, executer);
                executer.AddCommand(queue, command);
                return true;
            }
            else
            {
                OnExecute = command;
                return true;
            }
        }
    }
}
