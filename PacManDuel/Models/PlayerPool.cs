using System.Collections.Generic;

namespace PacManDuel.Models
{
    class PlayerPool
    {
        private readonly List<Player> _players;
        private int _currentPlayerIndex;

        public PlayerPool(Player playerA, Player playerB)
        {
            _players = new List<Player> {playerA, playerB};
            _currentPlayerIndex = 1;
        }

        public Player GetNextPlayer()
        {
            _currentPlayerIndex++;
            if (_currentPlayerIndex > _players.Count - 1) 
                _currentPlayerIndex = 0;
            return _players[_currentPlayerIndex];
        }

        public List<Player> GetPlayers()
        {
            return _players;
        }

    }
}
