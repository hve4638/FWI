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
        int beginIndex;
        int endIndex;
        int BeginIndex
        {
            get { return beginIndex; }
            set {
                if (value < 1) beginIndex = 1;
                else if (value < rawArgs.Length) beginIndex = value;
                else beginIndex = rawArgs.Length;
                beginIndex = (value < rawArgs.Length) ? value : rawArgs.Length;
            }
        }

        public PromptArgs(string text) : this(text.Split(' '))
        {

        }

        public PromptArgs(string[] rawArgs)
        {
            this.rawArgs = rawArgs;
            BeginIndex = 1;
            endIndex = rawArgs.Length;
        }

        public bool HasArg(int index)
        {
            return (index >= 0 && index < Count);
        }

        public string GetArg(int index, string def = null)
        {
            if (HasArg(index)) return rawArgs[index + beginIndex];
            else if (def == null) throw new IndexOutOfRangeException();
            return def;
        }

        public int GetArgInt(int index, int? def = null)
        {
            var arg = GetArg(index);
            if (int.TryParse(arg, out int result)) return result;
            else if (def is null) throw new ParseException($"'{arg}' is not parse to int");
            else return (int)def;
        }

        public string GetArgs(int begin = 0) => GetArgs(begin, Count);
        public string GetArgs(int begin, int end)
        {
            StringBuilder builder = new StringBuilder();
            for (var i = begin; i < end; i++)
            {
                if (HasArg(i))
                {
                    builder.Append(GetArg(i));
                    builder.Append(" ");
                }
            }

            return builder.ToString().Trim();
        }

        public string this[int index]
        {
            get { return GetArg(index); }
        }

        public PromptArgs Slice(int begin)
        {
            var sliced = new PromptArgs(rawArgs)
            {
                BeginIndex = BeginIndex + begin,
            };

            return sliced;
        }

        public int Count => (endIndex - BeginIndex);
        public string Command
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (var i = 0; i < beginIndex; i++)
                {
                    sb.Append(rawArgs[i]);
                    sb.Append(" ");
                }
                return sb.ToString().Trim();
            }
        }
        public string CommandLastWord
        {
            get
            {
                return rawArgs[beginIndex-1];
            }
        }

        public override string ToString()
        {
            return string.Join(" ", rawArgs);
        }
    }
}
