using System;
using PacManDuel.Models;
using System.Collections.Generic;

namespace PacManDuel
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckArguments(args);
            ShowArguments(args);

            var playerAPath = args[0];
            var playerABot = args[1];
            var playerBPath = args[2];
            var playerBBot = args[3];
            int rounds;
            if (!(args.Length == 5 && int.TryParse(args[4], out rounds)))
                rounds = 1;

            var games = new List<GameResult>();

            for (var i = 0; i < rounds; i++)
            {
                var playerA = new Player("botA", playerAPath, playerABot, 'B');
                var playerB = new Player("botB", playerBPath, playerBBot, 'A');
                var game = new Game(playerA, playerB, Properties.Settings.Default.SettingInitialMazeFilePath);
                var result = game.Run("Match-" + DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss"));
                games.Add(result);

                playerA = new Player("botB", playerBPath, playerBBot, 'B');
                playerB = new Player("botA", playerAPath, playerABot, 'A');
                game = new Game(playerA, playerB, Properties.Settings.Default.SettingInitialMazeFilePath);
                result = game.Run("Match-" + DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss"));
                games.Add(result);

            }

            GameSummary(games);
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

        private static void CheckArguments(string[] args)
        {
            int dummy;
            if (args.Length == 5 && !int.TryParse(args[4], out dummy))
                Console.WriteLine("Rounds not a valid number.");
            else if (args.Length >= 4 && args.Length <= 5)
                return;
            Console.WriteLine("Invalid arguments.");
            Console.WriteLine("Please provide four or five arguments.");
            Console.WriteLine("For each bot, provide the directory and executable name.");
            Console.WriteLine("You can also optionally supply the number of rounds to run.");
            Console.WriteLine("Command line example:");
            Console.WriteLine("  PacManDuel C:\\pacman\\testA start.bat C:\\pacman\\testB start.bat 2");
            Environment.Exit(1);
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
    }
}
