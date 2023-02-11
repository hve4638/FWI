using FWI.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWI.Prompt
{
    public class PromptArgs
    {
        readonly string[] rawArgs;
        public PromptArgs(string[] rawArgs)
        {
            this.rawArgs = rawArgs;
        }
        
        public string GetCMD() { return rawArgs[0]; }

        public bool HasArg(int index)
        {
            return (index >= 0 && index + 1 < rawArgs.Length);
        }

        public string GetArg(int index, string def = null)
        {
            if (index >= 0 && index + 1 < rawArgs.Length) return rawArgs[index + 1];
            else if (def == null) throw new IndexOutOfRangeException();
            return def;
        }

        public int GetArgInt(int index, int? def=null)
        {
            var arg = GetArg(index);
            if (int.TryParse(arg, out int result)) return result;
            else if (def is null) throw new ParseException($"'{arg}' is not parse to int");
            else return (int)def;
        }

        public string GetArgs(int begin = 0)
        {
            var sliced = SliceArgs(begin);
            return string.Join(" ", sliced);
        }
        public string GetArgs(int begin, int end)
        {
            var sliced = SliceArgs(begin, end);
            return string.Join(" ", sliced);
        }

        IEnumerable<string> SliceArgs(int begin = 0)
        {
            if (begin < 0) throw new IndexOutOfRangeException();

            return rawArgs.Skip(begin + 1);
        }

        IEnumerable<string> SliceArgs(int begin, int end)
        {
            var sliced = SliceArgs(begin);

            return sliced.Take(end - begin);
        }

        public string this[int index]
        {
            get { return GetArg(index); }
        }

        public int Count => (rawArgs.Length - 1);

        public override string ToString()
        {
            return string.Join(" ", rawArgs);
        }
    }
}
