using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectClient.Models
{
    class Games
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Player { get; set; }

        public DateTime StartDate { get; set; }

        public int GameLength { get; set; }
    }
}
