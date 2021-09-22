using System;
using CustomExceptions;
using System.Text.RegularExpressions;

namespace Domain
{
    public class Review
    {
        public string Comment { get; set; }
        public string Score { get; set; }
        public Game Game { get; set; }
        
        public User User { get; set; }

        public void ValidReview()
        {
            if (String.IsNullOrEmpty(this.Comment) || String.IsNullOrEmpty(this.Score) || !ScoreIsNumeric())
            {
                throw new InvalidReviewException(MessageException.InvalidReviewException);
            }
        }

        public bool ScoreIsNumeric()
        {
            Regex onlyNumbers = new Regex("^[0-9]*$");
            return onlyNumbers.IsMatch(this.Score);
        }

    }
}