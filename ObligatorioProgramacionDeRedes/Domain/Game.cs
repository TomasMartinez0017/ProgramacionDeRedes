using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CustomExceptions;

namespace Domain
{
    public class Game
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Rating { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public void ValidGame()
        {
            if (String.IsNullOrEmpty(this.Title) || String.IsNullOrEmpty(this.Genre) ||
                String.IsNullOrEmpty(this.Rating) ||
                String.IsNullOrEmpty(this.Description))
            {
                throw new InvalidGameException(MessageException.InvalidGameException);
            }
        }

        public void SetRating(string ratingInStringFormat)
        {
            int rating = Convert.ToInt32(ratingInStringFormat);
            switch (rating)
            {
                case 1:
                    this.Rating = "Everyone";
                    break;
                case 2:
                    this.Rating = "Teen";
                    break;
                case 3:
                    this.Rating = "Mature";
                    break;
                case 4:
                    this.Rating = "Adults Only";
                    break;
            }
        }

        public string ConvertRating()
        {
            string rating = "";
            switch (this.Rating)
            {
                case "Everyone":
                    rating="1";
                    break;
                case "Teen":
                    rating= "2";
                    break;
                case "Mature":
                    rating= "3";
                    break;
                case "Adults Only":
                    rating= "4";
                    break;
            }

            return rating;
        }
        
        public override bool Equals(object? obj)
        {
            return this.Title == ((Game)obj).Title;
        }

    }
}