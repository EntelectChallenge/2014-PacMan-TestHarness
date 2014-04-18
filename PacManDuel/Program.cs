
using System;
using PacManDuel.Models;

namespace PacManDuel
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckArguments(args);

            var playerAPath = args[0];
            var playerABot = args[1];
            var playerBPath = args[2];
            var playerBBot = args[3];

            var playerA = new Player("botB", playerBPath, playerBBot, 'B');
            var playerB = new Player("botA", playerAPath, playerABot, 'A');
            var game = new Game(playerA, playerB, Properties.Settings.Default.SettingInitialMazeFilePath);                                                            
            game.Run("Match-" + DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss"));
            
        }

        private static void CheckArguments(string[] args)
        {
            if (args.Length == 4) return;
            Console.WriteLine("Invalid arguments.");
            Console.WriteLine("Please provide four arguments. For each bot, provide the directory and executable name.");
            Console.WriteLine("Command line example:");
            Console.WriteLine("  PacManDuel C:\\pacman\\testA start.bat C:\\pacman\\testB start.bat");
            Environment.Exit(1);
        }
    }
}
