using CommandLine;
using IniParser;
using IniParser.Model;
using System;
using System.IO;

namespace FWIClient
{
    public static class ConfigExtender
    {
        public static bool Read(this ref Config config, string path)
        {
            if (!File.Exists(path)) return false;

            var parser = new FileIniDataParser();
            var data = parser.ReadFile("config.ini");

            var connection = data["Connection"];
            config.serverIP = connection.Get("ip", "127.0.0.1");
            config.serverPort = connection.Get("port", "7000");

            var general = data["General"];
            config.afkTime = general.Get("afktime", "10");
            config.observerMode = general.Get("observe", "0") == "1";
            config.autoReload = general.Get("autoreload", "0") == "1";
            config.openConsoleWhenStartup = general.Get("startupconsole", "0") == "1";
            config.debug = general.Get("debug", "0") == "1";

            return true;
        }

        static string Get(this KeyDataCollection collection, string key, string def)
        {
            var value = collection[key];

            return (value is null) ? def : value;
        }

        public static void Write(this Config config, string path)
        {
            var data = new IniData();
            data["Connection"]["ip"] = config.serverIP;
            data["Connection"]["port"] = config.serverPort;

            data["General"]["afktime"] = config.afkTime;
            data["General"]["observe"] = config.observerMode ? "1" : "0";
            data["General"]["autoreload"] = config.autoReload ? "1" : "0";
            data["General"]["startupconsole"] = config.openConsoleWhenStartup ? "1" : "0";
            data["General"]["debug"] = config.debug ? "1" : "0";


            var parser = new FileIniDataParser();
            parser.WriteFile("config.ini", data);
        }
    }
}
