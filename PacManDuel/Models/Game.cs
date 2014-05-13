using System;
using System.Drawing;
using System.IO;
using PacManDuel.Helpers;
using PacManDuel.Shared;

namespace PacManDuel.Models
{
    class Game
    {
        private readonly PlayerPool _playerPool;
        private readonly GameMarshaller _gameMarshaller;
        private Maze _maze;
        private int _iteration;
        private Player _currentPlayer;
        private readonly char _secondMazePlayer;

        public Game(Player playerA, Player playerB, String pathToInitialMaze)
        {
            _playerPool = new PlayerPool(playerA, playerB);
            _maze = new Maze(pathToInitialMaze);
            _gameMarshaller = new GameMarshaller();
            _iteration = 1;
            _secondMazePlayer = 'A';
        }

        public GameResult Run(String folderPath)
        {
            var gamePlayDirectoryPath = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + folderPath;
            Directory.CreateDirectory(gamePlayDirectoryPath);
            var outputFilePath = gamePlayDirectoryPath + System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.SettingGamePlayFile;
            _maze.WriteMaze(outputFilePath);
            Player winner = null;
            var gameOutcome = Enums.GameOutcome.ProceedToNextRound;
            Directory.CreateDirectory(folderPath);
            Directory.CreateDirectory(folderPath + System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.SettingReplayFolder);
            var logFile = new StreamWriter(folderPath + System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.SettingMatchLogFileName);
            logFile.WriteLine("[GAME] : Match started");
            while (gameOutcome == Enums.GameOutcome.ProceedToNextRound)
            {
                _currentPlayer = _playerPool.GetNextPlayer();
                var mazeFromPlayer = _currentPlayer.GetMove(_maze, gamePlayDirectoryPath + System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.SettingGamePlayFile, logFile);
                if (mazeFromPlayer != null)
                {
                    var mazeValidationOutcome = GetMazeValidationOutcome(logFile, mazeFromPlayer);
                    if (mazeValidationOutcome == Enums.MazeValidationOutcome.ValidMaze)
                    {
                        var opponentPosition = _maze.FindCoordinateOf(Symbols.SYMBOL_PLAYER_B);
                        var previousPosition = _maze.FindCoordinateOf(Symbols.SYMBOL_PLAYER_A);
                        var currentPosition = mazeFromPlayer.FindCoordinateOf(Symbols.SYMBOL_PLAYER_A);
                        var turnOutcome = GetTurnOutcome(mazeFromPlayer, currentPosition, previousPosition, opponentPosition, logFile);
                        if (turnOutcome != Enums.TurnOutcome.MoveMadeAndDroppedPoisonPillIllegally)
                        {
                            RegenerateOpponentIfDead(opponentPosition, mazeFromPlayer);
                            gameOutcome = GetGameOutcome(mazeFromPlayer, logFile, gameOutcome, turnOutcome);
                            winner = DeterminIfWinner(gameOutcome, mazeFromPlayer, winner);
                        }
                        else gameOutcome = ProcessIllegalMove(logFile, gameOutcome, ref winner);
                    }
                    else gameOutcome = ProcessIllegalMove(logFile, gameOutcome, ref winner);
                    
                    _maze.WriteMaze(gamePlayDirectoryPath + System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.SettingGamePlayFile);
                    CreateIterationStateFile(folderPath);
                    _iteration++;
                    foreach (var player in _playerPool.GetPlayers())
                    {
                        Console.Write(player.GetSymbol() + "," + player.GetPlayerName() + ": " + player.GetScore() + "  ");
                    }
                    Console.WriteLine();
                    _maze.Print();
                }
                else gameOutcome = ProcessIllegalMove(logFile, gameOutcome, ref winner);
            }

            CreateMatchInfo(gameOutcome, winner, logFile);
            logFile.Close();
            var replayMatchOutcome = new StreamWriter(folderPath + System.IO.Path.DirectorySeparatorChar + "replay" + System.IO.Path.DirectorySeparatorChar + "matchinfo.out");
            CreateMatchInfo(gameOutcome, winner, replayMatchOutcome);
            replayMatchOutcome.Close();

            return new GameResult()
            {
                Players = _playerPool.GetPlayers(),
                Outcome = gameOutcome,
                Iterations = _iteration - 1,
                Folder = folderPath
            };
        }

        private Enums.GameOutcome ProcessIllegalMove(StreamWriter logFile, Enums.GameOutcome gameOutcome, ref Player winner)
        {
            logFile.WriteLine("[GAME] : Illegal move made by " + _currentPlayer.GetPlayerName());
            gameOutcome = Enums.GameOutcome.IllegalMazeState;
            winner = _playerPool.GetNextPlayer();
            return gameOutcome;
        }

        private Player DeterminIfWinner(Enums.GameOutcome gameOutcome, Maze mazeFromPlayer, Player winner)
        {
            mazeFromPlayer.SwapPlayerSymbols();
            _maze = mazeFromPlayer;
            if (gameOutcome != Enums.GameOutcome.ProceedToNextRound)
            {
                if (gameOutcome == Enums.GameOutcome.NoScoringMaxed)
                {
                    winner = _playerPool.GetNextPlayer();
                }
                else
                {
                    winner = GameJudge.DetermineWinner(_playerPool);
                }
            }
            return winner;
        }

        private Enums.GameOutcome GetGameOutcome(Maze mazeFromPlayer, StreamWriter logFile, Enums.GameOutcome gameOutcome, Enums.TurnOutcome turnOutcome)
        {
            logFile.WriteLine("[GAME] : Player " + _currentPlayer.GetPlayerName() + " has " + _currentPlayer.GetScore() + " points");
            logFile.WriteLine("[TURN] : Moved to " + _currentPlayer.GetCurrentPosition().X + ", " + _currentPlayer.GetCurrentPosition().Y);
            gameOutcome = _gameMarshaller.ProcessGame(mazeFromPlayer, turnOutcome);
            logFile.WriteLine("[TURN] : " + _gameMarshaller.GetTurnsWithoutPointsInfo() + " turns without points");
            logFile.WriteLine("[GAME] : " + gameOutcome);
            return gameOutcome;
        }

        private Enums.TurnOutcome GetTurnOutcome(Maze mazeFromPlayer, Point currentPosition, Point previousPosition,
            Point opponentPosition, StreamWriter logFile)
        {
            var turnOutcome = TurnMarshaller.ProcessMove(mazeFromPlayer, _maze, currentPosition, previousPosition, opponentPosition, _currentPlayer);
            logFile.WriteLine("[TURN] : " + turnOutcome);
            logFile.WriteLine("[TURN] : " + _currentPlayer.GetPlayerName() + " at " + currentPosition.X + ", " +
                              currentPosition.Y);
            return turnOutcome;
        }

        private Enums.MazeValidationOutcome GetMazeValidationOutcome(StreamWriter logFile, Maze mazeFromPlayer)
        {
            logFile.WriteLine("[GAME] : Received maze from player " + _currentPlayer.GetPlayerName());
            var mazeValidationOutcome = (MazeValidator.ValidateMaze(mazeFromPlayer, _maze, logFile));
            logFile.WriteLine("[MAZE] : " + mazeValidationOutcome);
            return mazeValidationOutcome;
        }


        private void CreateIterationStateFile(String folderPath)
        {
            var replayFile =
                new StreamWriter(folderPath + System.IO.Path.DirectorySeparatorChar + Properties.Settings.Default.SettingReplayFolder + System.IO.Path.DirectorySeparatorChar + "iteration" +
                                 _iteration + Properties.Settings.Default.SettingStateFileExtension);
            var mazeForFile = new Maze(_maze);
            if (_secondMazePlayer == _currentPlayer.GetSymbol())
                mazeForFile.SwapPlayerSymbols();
            replayFile.Write(mazeForFile.ToFlatFormatString());
            replayFile.Close();
        }

        private void CreateMatchInfo(Enums.GameOutcome gameOutcome, Player winner, StreamWriter file)
        {
            foreach (var player in _playerPool.GetPlayers())
            {
                file.WriteLine("PLAYER:" + player.GetSymbol() + "," + player.GetPlayerName() + "," + player.GetScore());
            }
            if (winner == null)
                file.WriteLine("GAME: DRAW," + gameOutcome + "," + _iteration);
            else
                file.WriteLine("GAME: " + winner.GetSymbol() + "," + gameOutcome + "," + _iteration);
        }

        private static void RegenerateOpponentIfDead(Point opponentPosition, Maze mazeFromPlayer)
        {
            if (opponentPosition.IsEmpty)
                mazeFromPlayer.SetSymbol(Properties.Settings.Default.MazeCenterX, Properties.Settings.Default.MazeCenterY, Symbols.SYMBOL_PLAYER_B);
        }

    }
}
