using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using PacManDuel.Models;

namespace PacManDuel
{
    class Program
    {
        static readonly string[] helpOptions = { "/?", "-h", "--help" };

        static int Main(string[] args)
        {
            args = args ?? new string[] {};

            if (args.Any(x => helpOptions.Contains(x)) || args.Length != 4)
            {
                PrintUsage();
                return 1;
            }

            var playerAPath = args[0];
            var playerABot = args[1];
            var playerBPath = args[2];
            var playerBBot = args[3];

            if (!Directory.Exists(playerAPath))
            {
                Console.WriteLine("error: <adir> '{0}' does not exist or is not a directory", playerAPath);
                return 1;
            }

            if (!File.Exists(Path.Combine(playerAPath, playerABot)))
            {
                Console.WriteLine("error: <abot> '{0}' does not exist inside <adir> or is not a file", playerABot);
                return 1;
            }

            if (!Directory.Exists(playerBPath))
            {
                Console.WriteLine("error: <bdir> '{0}' does not exist or is not a directory", playerBPath);
                return 1;
            }

            if (!File.Exists(Path.Combine(playerBPath, playerBBot)))
            {
                Console.WriteLine("error: <bbot> '{0}' does not exist inside <bdir> or is not a file", playerBBot);
                return 1;
            }

            var playerA = new Player("botB", playerBPath, playerBBot, 'B');
            var playerB = new Player("botA", playerAPath, playerABot, 'A');
            var game = new Game(playerA, playerB, Properties.Settings.Default.SettingInitialMazeFilePath);                                                            
            game.Run("Match-" + DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss"));

            return 0;
        }

        static void PrintUsage()
        {
            Console.WriteLine(
                "usage: {1} <adir> <abot> <bdir> <bbot>{0}" + 
                "{0}" + 
                "args:{0}" +
                "<adir> Bot A's directory{0}" + 
                "<abot> Bot A's exe name{0}" + 
                "<bdir> Bot B's directory{0}" + 
                "<bbot> Bot B's exe name", 
                Environment.NewLine, GetExeName());
        }

        static string GetExeName()
        {
            return Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
        }
    }
}
