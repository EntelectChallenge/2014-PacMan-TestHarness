using PacManDuel.Shared;
using System.Collections.Generic;

namespace PacManDuel.Models
{
    class GameResult
    {
        public List<Player> Players { get; set; }
        public Enums.GameOutcome Outcome { get; set; }
        public int Iterations { get; set; }
        public string Folder { get; set; }
    }
}
