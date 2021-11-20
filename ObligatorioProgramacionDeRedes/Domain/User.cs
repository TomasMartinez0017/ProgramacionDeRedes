using System;
using System.Collections.Generic;

namespace Domain
{
    public class User
    {
        public string Username { get; set; }

        public List<Game> Games { get; set; }

        public User()
        {
            this.Games = new List<Game>();
        }

        public bool HasGame(string gameName)
        {
            foreach (Game game in Games)
            {
                if (game.Title.Equals(gameName))
                {
                    return true;
                }
            }

            return false;
        }
        
        public override bool Equals(object? obj)
        {
            return this.Username == ((User)obj).Username;
        }
        
    }
}