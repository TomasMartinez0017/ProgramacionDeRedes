using System;
using System.Collections.Generic;

namespace Domain
{
    public class User
    {
        private int Id { get; set; }

        private List<Game> Games { get; set; }
    }
}