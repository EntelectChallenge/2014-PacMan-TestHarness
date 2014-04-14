using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PacManDuel.Models;
using PacManDuel.Shared;

namespace PacManDuel.Helpers
{
    class MazeValidator
    {
        private const int _EXACT_NUMBER_OF_DIFFERENCES_FOR_LEGAL_MOVE = 2;

        public static Enums.MazeValidationOutcome ValidateMaze(Maze currentMaze, Maze previousMaze)
        {
            if (!IsMazeValid(currentMaze, previousMaze))
                return Enums.MazeValidationOutcome.InvalidMazeTooManyChanges;
            if(!IsPossibleMoveMade(currentMaze, previousMaze))
                return Enums.MazeValidationOutcome.InvalidMazeIllegalMoveMade;

            return (int)Enums.MazeValidationOutcome.ValidMaze;
        }

        private static bool IsMazeValid(Maze currentMaze, Maze previousMaze)
        {
            return GetNumberOfDifferences(currentMaze, previousMaze) == _EXACT_NUMBER_OF_DIFFERENCES_FOR_LEGAL_MOVE;
        }

        private static int GetNumberOfDifferences(Maze currentMaze, Maze previousMaze)
        {
            var difference = 0;
            for (var x = 0; x < Properties.Settings.Default.MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default.MazeWidth; y++)
                {
                    if (!currentMaze.GetSymbol(x, y).Equals(previousMaze.GetSymbol(x, y)))
                        difference++;
                }
            }
            return difference;
        }

        private static Boolean IsPossibleMoveMade(Maze currentMaze, Maze previousMaze)
        {
            var currentPosition = currentMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerA);
            return GetPossibleMoves(previousMaze).Any(coordinate => coordinate.X.Equals(currentPosition.X) && coordinate.Y.Equals(currentPosition.Y));
        }

        private static IEnumerable<Point> GetPossibleMoves(Maze previousMaze)
        {
            var previousCoordinate = previousMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerA);
            var moveList = new List<Point>();
            if (previousCoordinate.Y + 1 < Properties.Settings.Default.MazeWidth)
                if (!previousMaze.GetSymbol(previousCoordinate.X, previousCoordinate.Y + 1).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnEntranceA(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X, previousCoordinate.Y + 1));

            if (previousCoordinate.Y - 1 >= 0)
                if (!previousMaze.GetSymbol(previousCoordinate.X, previousCoordinate.Y - 1).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnEntranceB(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X, previousCoordinate.Y - 1));

            if (previousCoordinate.X + 1 < Properties.Settings.Default.MazeHeight)
                if (!previousMaze.GetSymbol(previousCoordinate.X + 1, previousCoordinate.Y).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnZone(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X + 1, previousCoordinate.Y));

            if (previousCoordinate.X - 1 >= 0)
                if (!previousMaze.GetSymbol(previousCoordinate.X - 1, previousCoordinate.Y).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnZone(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X - 1, previousCoordinate.Y));

            if (previousCoordinate.X.Equals(Properties.Settings.Default.MazeHeight / 2 - 1) && previousCoordinate.Y.Equals(Properties.Settings.Default.MazeWidth - 1))
                moveList.Add(new Point(Properties.Settings.Default.MazeHeight / 2 - 1, 0));

            if (previousCoordinate.X.Equals(Properties.Settings.Default.MazeHeight / 2 - 1) && previousCoordinate.Y.Equals(0))
                moveList.Add(new Point(Properties.Settings.Default.MazeHeight / 2 - 1, Properties.Settings.Default.MazeWidth - 1));

            return moveList;
        }

        private static bool WasInRespawnEntranceA(int previousX, int previousY)
        {
            return ((previousX == 8 || previousX == 9) && previousY == 9);
        }

        private static bool WasInRespawnEntranceB(int previousX, int previousY)
        {
            return ((previousX == 12 || previousX == 11) && previousY == 9);
        }

        private static bool WasInRespawnZone(int previousX, int previousY)
        {
            return (previousX == 10 &&  previousY == 9);
        }
    }
}
