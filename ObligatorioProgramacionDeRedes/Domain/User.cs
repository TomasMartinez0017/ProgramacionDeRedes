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
        
    }
}