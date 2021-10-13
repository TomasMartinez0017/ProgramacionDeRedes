using System;

namespace CustomExceptions
{
    public class InvalidGameException : Exception
    {
        public InvalidGameException(String message) : base(message){ }
    }
}