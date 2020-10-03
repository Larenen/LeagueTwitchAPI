using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueAPI.Models.ChampionsDTO
{
    public class ChampionMasteryDTO
    {
        public int ChampionId { get; set; }
        public int ChampionLevel { get; set; }
        public string ChampionPoints { get; set; }
        public string LastPlayTime { get; set; }
        public string ChampionPointsSinceLastLevel { get; set; }
        public string ChampionPointsUntilNextLevel { get; set; }
        public string ChestGranted { get; set; }
        public string TokensEarned { get; set; }
        public string SummonerId { get; set; }
        
    }
}
