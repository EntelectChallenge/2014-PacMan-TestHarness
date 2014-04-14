using System;

namespace PacManDuel.Exceptions
{
    class UnreadableMazeException : Exception
    {

        public UnreadableMazeException(string message) 
            : base(message)
        {
            Console.WriteLine(message);
        }

        public UnreadableMazeException(string message, Exception inner)
            : base(message, inner)
        {
            Console.WriteLine(inner.Message);
        }

    }
}
