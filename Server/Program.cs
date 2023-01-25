using System.Text;
using FWIConnection;
using FWI;
using FWI.Prompt;
using FWI.Exceptions;
using System.Diagnostics;
using CommandLine;
using System.Net;
using System.Net.Sockets;

using YamlDotNet.RepresentationModel;
using System.Reflection.Metadata;


namespace FWIServer
{
    static class Program
    {
        static readonly string Version = "0.5d dev 3";
        static readonly DateTime runDateTime = DateTime.Now;
        static public readonly IOutputStream stdOut = new FormatStandardOutputStream();

        static public TimeSpan Elapsed => (DateTime.Now - runDateTime);
        static public bool VerboseMode { get; set; }
        static public IOutputStream Out => stdOut;
        static public IOutputStream VerboseOut
        {
            get
            {
                if (VerboseMode) return stdOut;
                else return NullOutputStream.Instance;
            }
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.CurrentDomain_UnhandledException);

            Console.Title = "FWIServer";
            Console.WriteLine($"FWI Server");
            Console.WriteLine($"version: {Version}");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        static void Run(Options options)
        {
            var pathDict = new Dictionary<string, string>
            {
                ["rank"] = @"{document}\FWI\fwi.{signiture}.rank",
                ["timeline"] = @"{document}\FWI\fwi.{signiture}.timeline",
                ["alias"] = @"{document}\FWI\fwi.{signiture}.alias",
                ["ignore"] = @"{document}\FWI\fwi.{signiture}.ignore"
            };

            if (options.Path is not null)
            {
                if (!File.Exists(options.Path))
                {
                    WriteDefaultOption(options.Path);
                    Console.WriteLine($"{options.Path} 을 찾을 수 없습니다. 기본 설정 파일을 생성합니다.");
                }
                
                try
                {
                    SetUpOptions(ref options, ref pathDict);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"옵션을 불러오는데 실패했습니다. \"{options.Path}\"");
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"{e}");
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("");
                }
            }

            options.Port ??= 7000;
            options.Interval ??= 5;
            options.Verbose ??= false;
            options.Signiture ??= "main";

            Console.WriteLine($"- port: {options.Port}");
            Console.WriteLine($"- interval: {options.Interval}");
            Console.WriteLine($"- verbose: {options.Verbose}");
            Console.WriteLine($"- signiture: {options.Signiture}");

            var pathParser = new PathParser(options.Signiture);
            foreach(var key in pathDict.Keys) pathDict[key] = pathParser.Parse(pathDict[key]);

            var runner = new ServerRunner(options, pathDict);
            runner.Run();
        }

        static void WriteDefaultOption(string filename)
        {
            var writer = new StreamWriter(filename);
            var pathRank = @"{document}\\FWI\\fwi.{signiture}.rank";
            var pathTimeline = @"{document}\\FWI\\fwi.{signiture}.timeline";
            var pathAlias = @"{document}\\FWI\\fwi.alias";
            var pathIgnore = @"{document}\\FWI\\fwi.ignore";

            writer.WriteLine("setting:");
            writer.WriteLine("  interval: 5");
            writer.WriteLine("  port: 7000");
            writer.WriteLine("  verbose: true");
            writer.WriteLine("  signiture: main");
            writer.WriteLine("");
            writer.WriteLine("path:");
            writer.WriteLine($"  rank: \"{pathRank}\"");
            writer.WriteLine($"  timeline: \"{pathTimeline}\"");
            writer.WriteLine($"  alias: \"{pathAlias}\"");
            writer.WriteLine($"  ignore: \"{pathIgnore}\"");
            writer.Close();
        }

        static void SetUpOptions(ref Options option, ref Dictionary<string, string> pathDict)
        {
            var path = option.Path!;
            string yamlString = File.ReadAllText(path);

            var input = new StringReader(yamlString);
            var yaml = new YamlStream();
            yaml.Load(input);

            var document = yaml.Documents[0];
            var root = (YamlMappingNode)document.RootNode;
            var settingNode = (YamlMappingNode)root.Children[new YamlScalarNode("setting")];

            try
            {
                var intervalNode = settingNode.Children[new YamlScalarNode("interval")];
                var portNode = settingNode.Children[new YamlScalarNode("port")];
                var verboseNode = settingNode.Children[new YamlScalarNode("verbose")];
                var signitureNode = settingNode.Children[new YamlScalarNode("signiture")];

                var interval = int.Parse(intervalNode.ToString() ?? "ERROR");
                var port = int.Parse(portNode.ToString() ?? "ERROR");
                var signiture = signitureNode.ToString() ?? "main";
                var verbose = verboseNode.ToString() == "true";

                option.Interval = interval;
                option.Port = port;
                option.Verbose = verbose;
                option.Signiture = signiture;
            }
            catch (KeyNotFoundException)
            {

            }
            catch (FormatException)
            {

            }

            var pathNode = (YamlMappingNode)root.Children[new YamlScalarNode("path")];
            pathDict["rank"] = pathNode.Children[new YamlScalarNode("rank")].ToString()!;
            pathDict["timeline"] = pathNode.Children[new YamlScalarNode("timeline")].ToString()!;
            pathDict["alias"] = pathNode.Children[new YamlScalarNode("alias")].ToString()!;
            pathDict["ignore"] = pathNode.Children[new YamlScalarNode("ignore")].ToString()!;
        }
    }
}