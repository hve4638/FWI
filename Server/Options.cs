using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIServer
{
    class Options
    {
#pragma warning disable CS8618
        [Option('p', "port", Default = null, HelpText = "Port")]
        public int? Port { get; set; }

        [Option('v', "verbose", Default = null, HelpText = "Prints all messages to standard output.")]
        public bool? Verbose { get; set; }

        [Option('s', "signiture", Default = null, HelpText = "signiture to save")]
        public string? Signiture { get; set; }
        
        [Option('o', "option", Default = null, HelpText = "Option path")]
        public string? Path { get; set; }

        [Option('i', "interval", Default = null, HelpText = "Interval")]
        public int? Interval { get; set; }

        [Option('a', "autosave", Default = null, HelpText = "Autosave")]
        public int AutoSave { get; set; }

        [Option('h', "help", Default = false, HelpText = "Show help text")]
        public bool Help { get; set; } 
#pragma warning restore CS8618
    }
}
