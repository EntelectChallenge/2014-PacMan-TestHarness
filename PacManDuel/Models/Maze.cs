using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using PacManDuel.Exceptions;
using PacManDuel.Shared;

namespace PacManDuel.Models
{
    class Maze
    {
        private readonly char[][] _map;

        public Maze(String filePath)
        {
            try
            {
                _map = new char[Properties.Settings.Default.MazeHeight][];
                var fileContents = System.IO.File.ReadAllText(filePath);
                var rowCount = 0;
                foreach (var row in Regex.Split(fileContents, "\n"))
                {
                    _map[rowCount] = row.ToCharArray();
                    rowCount++;
                }
            }
            catch (Exception e)
            {
                throw new UnreadableMazeException("Maze could not be read", e);
            }
        }

        public Maze(Maze maze)
        {
            _map = new char[Properties.Settings.Default.MazeHeight][];
            for (var x = 0; x < Properties.Settings.Default.MazeHeight; x++)
            {
                var row = new char[Properties.Settings.Default.MazeWidth];
                for (var y = 0; y < Properties.Settings.Default.MazeWidth; y++)
                {
                    row[y] = maze.GetSymbol(x,y);
                }
                _map[x] = row;
            }
        }

        public char GetSymbol(int x, int y)
        {
            return _map[x][y];
        }

        public void SetSymbol(int x, int y, char symbol)
        {
            _map[x][y] = symbol;
        }

        public String ToFlatFormatString()
        {
            var result = "";
            for (var x = 0; x < Properties.Settings.Default.MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default.MazeWidth; y++)
                {
                    result += _map[x][y];
                }
                if (x != Properties.Settings.Default.MazeHeight - 1)
                    result += '\n';
            }
            return result;
        }

        public void SwapPlayerSymbols()
        {
            for (var x = 0; x < Properties.Settings.Default.MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default.MazeWidth; y++)
                {
                    if (_map[x][y].Equals(Properties.Settings.Default.SymbolPlayerA))
                    {
                        _map[x][y] = Properties.Settings.Default.SymbolPlayerB;
                    } else if (_map[x][y].Equals(Properties.Settings.Default.SymbolPlayerB))
                    {
                        _map[x][y] = Properties.Settings.Default.SymbolPlayerA;
                    }
                }
            }
        }

        public void Print()
        {
            Console.Out.WriteLine(ToFlatFormatString());
        }

        public Point FindCoordinateOf(char symbol)
        {
            for (var x = 0; x < Properties.Settings.Default.MazeHeight; x++)
            {
                for (var y = 0; y < Properties.Settings.Default.MazeWidth; y++)
                {
                    if (_map[x][y].Equals(symbol))
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point();
        }

        public void WriteMaze(String filePath)
        {
            using (var file = new System.IO.StreamWriter(filePath))
            {
                var output = "";
                for (var x = 0; x < Properties.Settings.Default.MazeHeight; x++)
                {
                    for (var y = 0; y < Properties.Settings.Default.MazeWidth; y++)
                    {
                        output += _map[x][y];
                    }
                    if (x != Properties.Settings.Default.MazeHeight - 1) output += ('\n');
                }
                file.Write(output);
                file.Close();
            }
        }

    }
}
