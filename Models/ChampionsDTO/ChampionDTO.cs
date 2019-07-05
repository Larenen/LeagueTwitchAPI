using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueAPI.Models
{
    public class ChampionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string SquarePortraitPath { get; set; }
        public string[] Roles { get; set; }
    }
}
