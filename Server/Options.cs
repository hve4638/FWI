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
        [Option('p', "port", Default = 7000, HelpText = "Port")]
        public int Port { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('s', "signiture", Default = "main", HelpText = "signiture to save")]
        public string? Signiture { get; set; }
    }
}
