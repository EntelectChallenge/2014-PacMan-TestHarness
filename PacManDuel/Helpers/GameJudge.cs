using System.Linq;
using PacManDuel.Models;
using PacManDuel.Shared;

namespace PacManDuel.Helpers
{
    class GameJudge
    {
        public static Player DetermineWinner(PlayerPool playerPool, Enums.GameOutcome gameOutcome)
        {
            if (playerPool.GetPlayers()[0].GetScore() != playerPool.GetPlayers()[1].GetScore())
                return playerPool.GetPlayers().OrderByDescending(x => x.GetScore()).First();
            if (gameOutcome == Enums.GameOutcome.NoScoringMaxed)
                return playerPool.GetNextPlayer();
            return null;
        }
    }
}
