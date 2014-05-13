using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using PacManDuel.Exceptions;

namespace PacManDuel.Models
{
    class Player
    {
        private readonly String _playerName;
        private readonly String _workingPath;
        private readonly String _executableFileName;
        private int _score;
        private int _numberOfPoisonPills;
        private Point _currentPosition;
        private char _symbol;

        public Player(String playerName, String workingPath, String executableFileName, char symbol)
        {
            _playerName = playerName;
            _workingPath = workingPath;
            _executableFileName = executableFileName;
            _score = 0;
            _numberOfPoisonPills = 1;
            _symbol = symbol;
        }

        public Maze GetMove(Maze maze, String outputFilePath, StreamWriter logFile)
        {
            var playerOutputFilePath = _workingPath + System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.SettingBotOutputFileName;
            File.Delete(playerOutputFilePath);
            var startTime = DateTime.Now;
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _workingPath,
                    FileName = _workingPath + System.IO.Path.DirectorySeparatorChar + _executableFileName,
                    Arguments = "\"" + outputFilePath + "\"",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            p.Start();
            System.Diagnostics.DataReceivedEventHandler h = (sender, args) => {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(_workingPath + System.IO.Path.DirectorySeparatorChar + "botlogs_capture.txt", true))
                {
                    file.Write(args.Data);
                }
            };
            p.OutputDataReceived  += h;
            p.ErrorDataReceived  += h;
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            try {
                startTime = p.StartTime; // Adjust for actual start time of process
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            var attemptFetchingMaze = true;
            while (attemptFetchingMaze)
            {
                if (File.Exists(playerOutputFilePath))
                {
                    Thread.Sleep(50); // Allow file write to complete, otherwise might get permission exception or corrupt file
                    if (!p.HasExited) p.Kill();
                    try
                    {
                        var mazeFromPlayer = new Maze(playerOutputFilePath);
                        return mazeFromPlayer;
                    }
                    catch (UnreadableMazeException e)
                    {
                        Console.WriteLine(e.ToString());
                        logFile.WriteLine("[GAME] : Unreadable maze from player " + _playerName);
                    }
                }
                if ((DateTime.Now - startTime).TotalSeconds > Properties.Settings.Default.SettingBotOutputTimeoutSeconds)
                {
                    attemptFetchingMaze = false;
                    if (!p.HasExited) p.Kill();
                    logFile.WriteLine("[GAME] : Timeout from player " + _playerName);
                }
                Thread.Sleep(100);
            }
            return null;
        }

        public void AddToScore(int score)
        {
            _score += score;
        }

        public int GetScore()
        {
            return _score;
        }

        public bool IsAllowedPoisonPillDrop()
        {
            return _numberOfPoisonPills > 0;
        }

        public void UsePoisonPill()
        {
            _numberOfPoisonPills--;
        }

        public String GetPlayerName()
        {
            return _playerName;
        }

        public String GetPlayerPath()
        {
            return _workingPath;
        }

        public Point GetCurrentPosition()
        {
            return _currentPosition;
        }

        public void SetCurrentPosition(Point coordinate)
        {
            _currentPosition = coordinate;
        }

        public char GetSymbol()
        {
            return _symbol;
        }

    }
}
