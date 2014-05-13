using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PacManDuel.Models;
using System.Collections.Generic;

namespace PacManDuel
{
    class Program
    {
        static readonly string[] helpOptions = { "/?", "-h", "--help" };

        static int Main(string[] args)
        {
            args = args ?? new string[] { };

            if (args.Any(x => helpOptions.Contains(x)) || args.Length != 4)
            {
                PrintUsage();
                return 1;
            }

            ShowArguments(args);

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

            var games = new List<GameResult>();

            var playerA = new Player("botB", playerBPath, playerBBot, 'B');
            var playerB = new Player("botA", playerAPath, playerABot, 'A');
            var game = new Game(playerA, playerB, Properties.Settings.Default.SettingInitialMazeFilePath);
            var result = game.Run("Match_" + DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss"));
            games.Add(result);

            GameSummary(games);
            return 0;
        }

        private static void GameSummary(List<GameResult> games)
        {
            Console.WriteLine();
            Console.WriteLine("Results:");
            Console.WriteLine("========");
            Console.WriteLine();
            Console.WriteLine(games[0].Players[0].GetPlayerName() + " = " + games[0].Players[0].GetPlayerPath());
            Console.WriteLine(games[0].Players[1].GetPlayerName() + " = " + games[0].Players[1].GetPlayerPath());
            Console.WriteLine();
            var playerATotal = 0;
            var playerBTotal = 0;
            var firstPlayer = games[0].Players[0].GetPlayerName();
            Console.WriteLine("{0,10}  {1,10}  Moves", games[0].Players[0].GetPlayerName(), games[0].Players[1].GetPlayerName());
            foreach (var game in games)
            {
                int p1 = 0, p2 = 0;
                if (game.Players[0].GetPlayerName() != firstPlayer) p1 = 1;
                p2 = 1 - p1;
                playerATotal += game.Players[p1].GetScore();
                playerBTotal += game.Players[p2].GetScore();
                Console.WriteLine("{0,10}{1} {2,10}{3}  {4,4} {5:-15} {6}",
                    game.Players[p1].GetScore(), p1 == 0 ? "*" : " ",
                    game.Players[p2].GetScore(), p2 == 0 ? "*" : " ",
                    game.Iterations, game.Outcome.ToString(), game.Folder);
            }
            Console.WriteLine("==========  ==========");
            Console.WriteLine("{0,10}  {1,10}", playerATotal, playerBTotal);
            Console.WriteLine();
            Console.WriteLine("* = Moved first");
        }

        private static void ShowArguments(string[] args)
        {
            Console.WriteLine("Arguments:");
            Console.WriteLine("Player A path:       " + args[0]);
            Console.WriteLine("Player A executable: " + args[1]);
            Console.WriteLine("Player B path:       " + args[2]);
            Console.WriteLine("Player B executable: " + args[3]);
            int rounds;
            if (args.Length == 5 && int.TryParse(args[4], out rounds))
                Console.WriteLine("Number of rounds:    " + rounds);
            Console.WriteLine();
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
                "<bbot> Bot B's exe name{0}" + 
                "{0}" +
                "For each bot, provide the directory and executable name.{0}" +
                "Command line example:{0}" +
                "  PacManDuel C:\\pacman\\testA start.bat C:\\pacman\\testB start.bat",
                Environment.NewLine, GetExeName());
        }

        static string GetExeName()
        {
            return Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
        }
    }
}
