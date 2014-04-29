using System.Drawing;
using PacManDuel.Models;
using PacManDuel.Shared;

namespace PacManDuel.Helpers
{
    class TurnMarshaller
    {
        public static Enums.TurnOutcome ProcessMove(Maze currentMaze, Maze previousMaze, Point currentPosition, Point previousPosition, Point opponentPosition, Player currentPlayer)
        {
            currentPlayer.SetCurrentPosition(currentPosition);

            if (IsMoveMadeAndScoredPoint(previousMaze, currentPosition))
            {
                currentPlayer.AddToScore(Properties.Settings.Default.SettingPointsPerPill);
                return Enums.TurnOutcome.MoveMadeAndPointScored;
            }

            if (IsMoveMadeAndScoredBonusPoint(previousMaze, currentPosition))
            {
                currentPlayer.AddToScore(Properties.Settings.Default.SettingPointsPerBonusPill);
                return Enums.TurnOutcome.MoveMadeAndBonusPointScored;
            }
            
            if (IsMoveMadeAndDiedFromPoisonPill(previousMaze, currentPosition))
            {
                currentMaze.SetSymbol(currentPosition.X, currentPosition.Y, Symbols.SYMBOL_EMPTY);
                currentMaze.SetSymbol(Properties.Settings.Default.MazeCenterX, Properties.Settings.Default.MazeCenterY, Symbols.SYMBOL_PLAYER_A);
                return Enums.TurnOutcome.MoveMadeAndDiedFromPoisonPill;
            }

            if (IsMoveMadeAndKilledOpponent(currentPosition, opponentPosition))
            {
                currentMaze.SetSymbol(Properties.Settings.Default.MazeCenterX, Properties.Settings.Default.MazeCenterY, Symbols.SYMBOL_PLAYER_B);
                return Enums.TurnOutcome.MoveMadeAndKilledOpponent;
            }

            if (IsMoveMadeAndDroppedPoisonPill(currentMaze, previousPosition))
            {
                if (!currentPlayer.IsAllowedPoisonPillDrop())
                    return Enums.TurnOutcome.MoveMadeAndDroppedPoisonPillIllegally;

                currentPlayer.UsePoisonPill();
                return Enums.TurnOutcome.MoveMadeAndDroppedPoisonPill;
            }

            return (int)Enums.TurnOutcome.MoveMade;
        }

        private static bool IsMoveMadeAndScoredPoint(Maze previousMaze, Point currentPosition)
        {
            return previousMaze.GetSymbol(currentPosition.X, currentPosition.Y) == Symbols.SYMBOL_PILL;
        }

        private static bool IsMoveMadeAndScoredBonusPoint(Maze previousMaze, Point currentPosition)
        {
            return previousMaze.GetSymbol(currentPosition.X, currentPosition.Y) == Symbols.SYMBOL_BONUS_PILL;
        }

        private static bool IsMoveMadeAndDiedFromPoisonPill(Maze previousMaze, Point currentPosition)
        {
            return previousMaze.GetSymbol(currentPosition.X, currentPosition.Y) == Symbols.SYMBOL_POISON_PILL;
        }

        private static bool IsMoveMadeAndKilledOpponent(Point currentPosition, Point opponentPosition)
        {
            if (opponentPosition.IsEmpty) return false;
            return currentPosition.X == opponentPosition.X && currentPosition.Y == opponentPosition.Y;
        }

        private static bool IsMoveMadeAndDroppedPoisonPill(Maze currentMaze, Point previousPosition)
        {
            return currentMaze.GetSymbol(previousPosition.X, previousPosition.Y) == Symbols.SYMBOL_POISON_PILL;
        }

    }
}
