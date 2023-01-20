using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIServer
{
    internal class PathParser
    {
        Dictionary<string, string> dict;
        
        public PathParser(string signiture)
        {
            dict = new Dictionary<string, string>
            {
                { "{document}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
                { "{desktop}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
                { "{signiture}", $"{signiture}" },
            };
        }

        public string Parse(string input)
        {
            foreach (var (key, value) in dict) input = input.Replace(key, value);
            return input;
        }
    }
}
