using System;
namespace CustomExceptions
{
    public class InvalidReviewException : Exception
    {
        public InvalidReviewException(String message) : base(message){ }
    }
}