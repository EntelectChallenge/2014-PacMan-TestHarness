namespace PacManDuel.Shared
{
    internal class Enums
    {

        public enum MazeValidationOutcome
        {
            ValidMaze = 0,
            InvalidMazeTooManyChanges = 1,
            InvalidMazeIllegalMoveMade = 2,
            InvalidMazePillDroppedInRespawnZone = 3
        }

        public enum TurnOutcome
        {
            MoveMade = 0,
            MoveMadeAndPointScored = 1,
            MoveMadeAndKilledOpponent = 2,
            MoveMadeAndDroppedPoisonPill = 3,
            MoveMadeAndDiedFromPoisonPill = 4,
            MoveMadeAndDroppedPoisonPillIllegally = 5,
            MoveMadeAndBonusPointScored = 6,
            MoveMadePointScoredAndDroppedPoisonPill = 7,
            MoveMadeBonusPointScoredAndDroppedPoisonPill = 8,
            MoveMadeDroppedPoisonPillAndDiedFromPoisonPill = 9,
            MoveMadeDroppedPoisonPillAndKilledOpponent = 10
        }

        public enum GameOutcome
        {
            ProceedToNextRound = 0,
            OutOfPills = 1,
            NoScoringMaxed = 2,
            IllegalMovement = 3,
            IllegalMazeState = 4,
            IllegalAction = 5
        }

        public enum GameStatus
        {
            Draw = 0,
            WinnerDeclared = 1
        }

    }
}
