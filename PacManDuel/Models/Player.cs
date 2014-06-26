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

            var processName = _workingPath + System.IO.Path.DirectorySeparatorChar + _executableFileName;
            var arguments = "\"" + outputFilePath + "\"";

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                arguments = processName + " " + arguments;
                processName = "/bin/bash";
            }
            
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _workingPath,
                    FileName = processName,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            System.Diagnostics.DataReceivedEventHandler h = (sender, args) => {
                if (!String.IsNullOrEmpty(args.Data)) 
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(_workingPath + System.IO.Path.DirectorySeparatorChar + "botlogs_capture.txt", true))
                    {
                        file.WriteLine(args.Data);
                    }
                }
            };
            p.OutputDataReceived  += h;
            p.ErrorDataReceived  += h;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            bool didExit = p.WaitForExit(Properties.Settings.Default.SettingBotOutputTimeoutSeconds * 1000);
            if (!didExit)
            {
                p.Kill();
                logFile.WriteLine("[GAME] : Killed process " + processName);
            }

            if (p.ExitCode != 0)
            {
                logFile.WriteLine("[GAME] : Process exited " + p.ExitCode + " from player " + _playerName);
            }

            if (!File.Exists(playerOutputFilePath)) 
            {
                logFile.WriteLine("[GAME] : No output file from player " + _playerName);
                return null;
            }
            try
            {
                var mazeFromPlayer = new Maze(playerOutputFilePath);
                return mazeFromPlayer;
            }
            catch (UnreadableMazeException e)
            {
                Console.WriteLine(e.ToString());
                logFile.WriteLine("[GAME] : Unreadable maze from player: " + _playerName);
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
