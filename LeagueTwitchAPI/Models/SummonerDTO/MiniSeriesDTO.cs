using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueAPI.Models
{
    public class MiniSeriesDTO
    {
        public int Target { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public string Progress { get; set; }
    }
}
