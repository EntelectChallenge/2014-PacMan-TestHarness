
using System;
using PacManDuel.Models;

namespace PacManDuel
{
    class Program
    {
        static void Main(string[] args)
        {
            var playerAPath = args[0];
            var playerABot = args[1];
            var playerBPath = args[2];
            var playerBBot = args[3];

            var playerA = new Player("botB", playerBPath, playerBBot, 'B');
            var playerB = new Player("botA", playerAPath, playerABot, 'A');
            var game = new Game(playerA, playerB, Properties.Settings.Default.SettingInitialMazeFilePath);                                                            
            game.Run("Match-" + DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss"));
            
        }
    }
}
