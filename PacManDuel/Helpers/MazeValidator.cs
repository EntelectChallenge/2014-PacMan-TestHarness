using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PacManDuel.Models;
using PacManDuel.Shared;
using System.IO;

namespace PacManDuel.Helpers
{
    class MazeValidator
    {
        private const int _EXACT_NUMBER_OF_DIFFERENCES_FOR_LEGAL_MOVE = 2;

        public static Enums.MazeValidationOutcome ValidateMaze(Maze currentMaze, Maze previousMaze, StreamWriter logFile)
        {
            if (!IsMazeValid(currentMaze, previousMaze, logFile))
                return Enums.MazeValidationOutcome.InvalidMazeTooManyChanges;
            if(!IsPossibleMoveMade(currentMaze, previousMaze))
                return Enums.MazeValidationOutcome.InvalidMazeIllegalMoveMade;
            if (IsPillDroppedInRespawnZone(currentMaze, previousMaze))
                return Enums.MazeValidationOutcome.InvalidMazePillDroppedInRespawnZone;

            return (int)Enums.MazeValidationOutcome.ValidMaze;
        }

        private static bool IsPillDroppedInRespawnZone(Maze currentMaze, Maze previousMaze)
        {
            var previousCoordinateA = previousMaze.FindCoordinateOf(Symbols.SYMBOL_PLAYER_A);
            if (currentMaze.GetSymbol(previousCoordinateA) == Symbols.SYMBOL_POISON_PILL)
                return WasInRespawnZone(previousCoordinateA.X, previousCoordinateA.Y);
            return false;
        }

        private static bool IsMazeValid(Maze currentMaze, Maze previousMaze, StreamWriter logFile)
        {
            var diffs = GetNumberOfDifferences(currentMaze, previousMaze);
            if (diffs == _EXACT_NUMBER_OF_DIFFERENCES_FOR_LEGAL_MOVE) return true;

            logFile.WriteLine("[Validator] : Failure: Number of changes is: " + diffs);
            return false;
        }

        private static int GetNumberOfDifferences(Maze currentMaze, Maze previousMaze)
        {
            var difference = 0;
            for (var x = 0; x < Properties.Settings.Default.MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default.MazeWidth; y++)
                {
                    if (currentMaze.GetSymbol(x, y) != previousMaze.GetSymbol(x, y))
                        difference++;
                }
            }
            return difference;
        }

        private static Boolean IsPossibleMoveMade(Maze currentMaze, Maze previousMaze)
        {
            var currentPosition = currentMaze.FindCoordinateOf(Symbols.SYMBOL_PLAYER_A);
            return GetPossibleMoves(previousMaze).Any(coordinate => coordinate.X.Equals(currentPosition.X) && coordinate.Y.Equals(currentPosition.Y));
        }

        private static IEnumerable<Point> GetPossibleMoves(Maze previousMaze)
        {
            var previousCoordinate = previousMaze.FindCoordinateOf(Symbols.SYMBOL_PLAYER_A);
            var moveList = new List<Point>();
            // Right
            if (previousCoordinate.Y + 1 < Properties.Settings.Default.MazeWidth)
                if (previousMaze.GetSymbol(previousCoordinate.X, previousCoordinate.Y + 1) != Symbols.SYMBOL_WALL &&
                    !WasInRespawnPoint(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X, previousCoordinate.Y + 1));

            // Left
            if (previousCoordinate.Y - 1 >= 0)
                if (previousMaze.GetSymbol(previousCoordinate.X, previousCoordinate.Y - 1) != Symbols.SYMBOL_WALL &&
                    !WasInRespawnPoint(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X, previousCoordinate.Y - 1));

            // Down
            if (previousCoordinate.X + 1 < Properties.Settings.Default.MazeHeight)
                if (previousMaze.GetSymbol(previousCoordinate.X + 1, previousCoordinate.Y) != Symbols.SYMBOL_WALL &&
                    !WasInRespawnEntranceA(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X + 1, previousCoordinate.Y));

            // Up
            if (previousCoordinate.X - 1 >= 0)
                if (previousMaze.GetSymbol(previousCoordinate.X - 1, previousCoordinate.Y) != Symbols.SYMBOL_WALL &&
                    !WasInRespawnEntranceB(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X - 1, previousCoordinate.Y));

            // Wrap right
            if (previousCoordinate.X == (Properties.Settings.Default.MazeHeight / 2 - 1) && previousCoordinate.Y == (Properties.Settings.Default.MazeWidth - 1))
                moveList.Add(new Point(Properties.Settings.Default.MazeHeight / 2 - 1, 0));

            // Wrap left
            if (previousCoordinate.X == (Properties.Settings.Default.MazeHeight / 2 - 1) && previousCoordinate.Y == 0)
                moveList.Add(new Point(Properties.Settings.Default.MazeHeight / 2 - 1, Properties.Settings.Default.MazeWidth - 1));

            return moveList;
        }

        private static bool WasInRespawnEntranceA(int previousX, int previousY)
        {
            return ((previousX == (Properties.Settings.Default.MazeCenterX - 2) || previousX == (Properties.Settings.Default.MazeCenterX - 1)) && 
                previousY == Properties.Settings.Default.MazeCenterY);
        }

        private static bool WasInRespawnEntranceB(int previousX, int previousY)
        {
            return ((previousX == (Properties.Settings.Default.MazeCenterX + 2) || previousX == (Properties.Settings.Default.MazeCenterX + 1)) &&
                previousY == Properties.Settings.Default.MazeCenterY);
        }

        private static bool WasInRespawnPoint(int previousX, int previousY)
        {
            return (previousX == Properties.Settings.Default.MazeCenterX &&
                previousY == Properties.Settings.Default.MazeCenterY);
        }

        private static bool WasInRespawnZone(int previousX, int previousY)
        {
            return WasInRespawnPoint(previousX, previousY) ||
                   (previousX == (Properties.Settings.Default.MazeCenterX + 1) &&
                    previousY == Properties.Settings.Default.MazeCenterY) ||
                   (previousX == (Properties.Settings.Default.MazeCenterX - 1) &&
                    previousY == Properties.Settings.Default.MazeCenterY);
        }

    }
}
