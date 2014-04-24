using System;
using System.Linq;
using PacManDuel.Models;
using PacManDuel.Shared;

namespace PacManDuel.Helpers
{
    class GameMarshaller
    {
        private int _numberOfTurnsWithNoPointsGained;

        public GameMarshaller()
        {
            _numberOfTurnsWithNoPointsGained = 0;
        }

        public Enums.GameOutcome ProcessGame(Maze maze, Enums.TurnOutcome turnOutcome)
        {
            ProcessTurnOutcome(turnOutcome);
            
            if (IsOutOfPills(maze))
                return Enums.GameOutcome.OutOfPills;
            if (_numberOfTurnsWithNoPointsGained > Properties.Settings.Default.SettingMaxTurnsWithNoPointsScored)
                return Enums.GameOutcome.NoScoringMaxed;

            return Enums.GameOutcome.ProceedToNextRound;
        }

        private void ProcessTurnOutcome(Enums.TurnOutcome turnOutcome)
        {
            if (turnOutcome != Enums.TurnOutcome.MoveMadeAndPointScored)
                _numberOfTurnsWithNoPointsGained++;
            else
                _numberOfTurnsWithNoPointsGained = 0;
        }

        public static bool IsOutOfPills(Maze maze)
        {
            var flatFormatMaze = maze.ToFlatFormatString();
            return !(flatFormatMaze.Contains(Properties.Settings.Default.SymbolPill) || flatFormatMaze.Contains(Properties.Settings.Default.SymbolBonusPill));
        }

        public String GetTurnsWithoutPointsInfo()
        {
            return _numberOfTurnsWithNoPointsGained + " / " + Properties.Settings.Default.SettingMaxTurnsWithNoPointsScored;
        }

    }
}
