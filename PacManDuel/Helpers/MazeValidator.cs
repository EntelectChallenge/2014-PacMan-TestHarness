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
        private const int _EXACT_NUMBER_OF_DIFFERENCES_FOR_TELEPORT = 3;
        private static Point center = new Point(Properties.Settings.Default.MazeCenterX, Properties.Settings.Default.MazeCenterY);

        public static Enums.MazeValidationOutcome ValidateMaze(Maze currentMaze, Maze previousMaze, StreamWriter logFile)
        {
            if (!IsMazeValid(currentMaze, previousMaze, logFile))
                return Enums.MazeValidationOutcome.InvalidMazeTooManyChanges;
            if(!IsPossibleMoveMade(currentMaze, previousMaze))
                return Enums.MazeValidationOutcome.InvalidMazeIllegalMoveMade;

            return (int)Enums.MazeValidationOutcome.ValidMaze;
        }

        private static bool IsMazeValid(Maze currentMaze, Maze previousMaze, StreamWriter logFile)
        {
            var previousCoordinateA = previousMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerA);
            var previousCoordinateB = previousMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerB);
            var currentCoordinateA = currentMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerA);
            var currentCoordinateB = currentMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerB);

            if ((currentMaze.GetSymbol(previousCoordinateA) != Properties.Settings.Default.SymbolEmpty &&
                currentMaze.GetSymbol(previousCoordinateA) != Properties.Settings.Default.SymbolPoisonPill &&
                currentMaze.GetSymbol(previousCoordinateA) != Properties.Settings.Default.SymbolPlayerB) ||
                (currentMaze.GetSymbol(previousCoordinateA) == Properties.Settings.Default.SymbolPlayerB &&
                currentMaze.GetSymbol(center) != Properties.Settings.Default.SymbolPlayerB))
            {
                logFile.WriteLine("[Validator] : Symbol at previous location is: " + currentMaze.GetSymbol(previousCoordinateA));
                return false;
            }

            var diffs = GetNumberOfDifferences(currentMaze, previousMaze);
            if (diffs == _EXACT_NUMBER_OF_DIFFERENCES_FOR_LEGAL_MOVE) return true;

            if (diffs == _EXACT_NUMBER_OF_DIFFERENCES_FOR_TELEPORT)
            {
                // A ate B
                if ((PointManhattanDistance(previousCoordinateA, previousCoordinateB) == 1 || // Adjacent
                    PointManhattanDistance(previousCoordinateA, previousCoordinateB) == 18) && // Warp distance
                    currentMaze.GetSymbol(previousCoordinateA) == Properties.Settings.Default.SymbolEmpty &&
                    currentCoordinateA == previousCoordinateB &&
                    previousCoordinateB != center &&
                    currentCoordinateB == center)
                {
                    logFile.WriteLine("[Validator] : Player ate other player");
                    return true;
                }

                // A ate poison
                if (currentMaze.GetSymbol(previousCoordinateA) == Properties.Settings.Default.SymbolEmpty &&
                    WasAdjacentPoisonConsumed(currentMaze, previousMaze, previousCoordinateA) &&
                    previousCoordinateB != center &&
                    currentCoordinateA == center)
                {
                    logFile.WriteLine("[Validator] : Player ate poison");
                    return true;
                }

                // A ate poison while B was at center
                // Rules are not yet defined, currently assumes B take A's previous position to maintain maze integrity
                if (currentMaze.GetSymbol(previousCoordinateA) == Properties.Settings.Default.SymbolPlayerB &&
                    WasAdjacentPoisonConsumed(currentMaze, previousMaze, previousCoordinateA) &&
                    previousCoordinateB == center &&
                    currentCoordinateA == center)
                {
                    logFile.WriteLine("[Validator] : Player ate poison while B was at center");
                    return true;
                }
            }
            logFile.WriteLine("[Validator] : Failure: Number of changes is: " + diffs);
            return false;
        }

        private static bool WasAdjacentPoisonConsumed(Maze currentMaze, Maze previousMaze, Point previousCoordinateA)
        {
            // Up
            Point up = new Point(previousCoordinateA.X - 1, previousCoordinateA.Y);
            if (currentMaze.GetSymbol(up) == Properties.Settings.Default.SymbolEmpty &&
                previousMaze.GetSymbol(up) == Properties.Settings.Default.SymbolPoisonPill) return true;
            // Down
            Point down = new Point(previousCoordinateA.X + 1, previousCoordinateA.Y);
            if (currentMaze.GetSymbol(down) == Properties.Settings.Default.SymbolEmpty &&
                previousMaze.GetSymbol(down) == Properties.Settings.Default.SymbolPoisonPill) return true;
            // Left
            Point left = new Point(previousCoordinateA.X, previousCoordinateA.Y - 1);
            if (currentMaze.GetSymbol(left) == Properties.Settings.Default.SymbolEmpty &&
                previousMaze.GetSymbol(left) == Properties.Settings.Default.SymbolPoisonPill) return true;
            // Right
            Point right = new Point(previousCoordinateA.X, previousCoordinateA.Y + 1);
            if (currentMaze.GetSymbol(left) == Properties.Settings.Default.SymbolEmpty &&
                previousMaze.GetSymbol(left) == Properties.Settings.Default.SymbolPoisonPill) return true;
            return false;
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
            var previousPosition = previousMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerA);
            if (WasAdjacentPoisonConsumed(currentMaze, previousMaze, previousPosition) &&
                currentPosition == center) return true;
            return GetPossibleMoves(previousMaze).Any(coordinate => coordinate.X.Equals(currentPosition.X) && coordinate.Y.Equals(currentPosition.Y));
        }

        private static IEnumerable<Point> GetPossibleMoves(Maze previousMaze)
        {
            var previousCoordinate = previousMaze.FindCoordinateOf(Properties.Settings.Default.SymbolPlayerA);
            var moveList = new List<Point>();
            // Right
            if (previousCoordinate.Y + 1 < Properties.Settings.Default.MazeWidth)
                if (!previousMaze.GetSymbol(previousCoordinate.X, previousCoordinate.Y + 1).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnZone(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X, previousCoordinate.Y + 1));

            // Left
            if (previousCoordinate.Y - 1 >= 0)
                if (!previousMaze.GetSymbol(previousCoordinate.X, previousCoordinate.Y - 1).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnZone(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X, previousCoordinate.Y - 1));

            // Down
            if (previousCoordinate.X + 1 < Properties.Settings.Default.MazeHeight)
                if (!previousMaze.GetSymbol(previousCoordinate.X + 1, previousCoordinate.Y).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnEntranceA(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X + 1, previousCoordinate.Y));

            // Up
            if (previousCoordinate.X - 1 >= 0)
                if (!previousMaze.GetSymbol(previousCoordinate.X - 1, previousCoordinate.Y).Equals(Properties.Settings.Default.SymbolWall) &&
                    !WasInRespawnEntranceB(previousCoordinate.X, previousCoordinate.Y))
                    moveList.Add(new Point(previousCoordinate.X - 1, previousCoordinate.Y));

            // Wrap right
            if (previousCoordinate.X.Equals(Properties.Settings.Default.MazeHeight / 2 - 1) && previousCoordinate.Y.Equals(Properties.Settings.Default.MazeWidth - 1))
                moveList.Add(new Point(Properties.Settings.Default.MazeHeight / 2 - 1, 0));

            // Wrap left
            if (previousCoordinate.X.Equals(Properties.Settings.Default.MazeHeight / 2 - 1) && previousCoordinate.Y.Equals(0))
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

        private static bool WasInRespawnZone(int previousX, int previousY)
        {
            return (previousX == Properties.Settings.Default.MazeCenterX &&
                previousY == Properties.Settings.Default.MazeCenterY);
        }

        private static int PointManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
