using System.Linq;
using PacManDuel.Models;

namespace PacManDuel.Helpers
{
    class GameJudge
    {
        public static Player DetermineWinner(PlayerPool playerPool)
        {
            if (playerPool.GetPlayers()[0].GetScore() != playerPool.GetPlayers()[1].GetScore())
                return playerPool.GetPlayers().OrderByDescending(x => x.GetScore()).First();
            return null;
        }
    }
}
