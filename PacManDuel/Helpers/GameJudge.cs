using System.Linq;
using NLog;
using PacManDuel.Models;

namespace PacManDuel.Services
{
    class GameJudge
    {
        public static Player DetermineWinner(PlayerPool playerPool)
        {
            if (!playerPool.GetPlayers()[0].GetScore().Equals(playerPool.GetPlayers()[1].GetScore()))
                return playerPool.GetPlayers().OrderByDescending(x => x.GetScore()).First();
            return null;
        }
    }
}
