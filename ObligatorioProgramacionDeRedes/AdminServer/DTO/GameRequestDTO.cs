using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.DTO
{
    public class GameRequestDTO
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Rating { get; set; }
        public string Description { get; set; }
    }
}
