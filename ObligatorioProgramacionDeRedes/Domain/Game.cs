using System;
using System.Collections.Generic;
using CustomExceptions;

namespace Domain
{
    public class Game
    {
        public string Title { get; set; }
        
        public string Genre { get; set; }
        
        public string ScoreAverage { get; set; }
        
        public string Description { get; set; }
        
        //private string Image { get; set; }

        public void ValidGame()
        {
            if (String.IsNullOrEmpty(this.Title) || String.IsNullOrEmpty(this.Genre) ||
                String.IsNullOrEmpty(this.ScoreAverage) ||
                String.IsNullOrEmpty(this.Description))
            {
                throw new InvalidGameException(MessageException.InvalidGameException);
            }
        }

    }
}