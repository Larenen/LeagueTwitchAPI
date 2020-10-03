using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace LeagueAPI.Models
{
    public class SummonerDTO
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Puuid { get; set; }
        public string Name { get; set; }
        public int ProfileIconId { get; set; }
        public BigInteger RevisionDate { get; set; }
        public int SummonerLevel { get; set; }
    }
}
