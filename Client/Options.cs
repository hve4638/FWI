using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public class Options
    {
        [Option('p', "port", Required = false, Default = 7001, HelpText = "Destination Port")]
        public int Port { get; set; }

        [Option('d', "dest", Default = "127.0.0.1", HelpText = "Destination IP Address")]
        public string? IP { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('t', "target", Default = false, HelpText = "Target")]
        public bool Target { get; set; }

        [Option('a', "afk", Default = 10, HelpText = "AFK Time")]
        public int AFK { get; set; }

        [Option('x', "debug", Default = false, HelpText = "Debug Mode")]
        public bool DebugMode { get; set; }

        [Option('a', "autoreload", Default = false, HelpText = "auto reload")]
        public bool AutoReload { get; set; }
    }
}
