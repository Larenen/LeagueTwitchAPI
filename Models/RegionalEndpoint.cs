using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeagueAPI.Exceptions;

namespace LeagueAPI.Models
{
    public static class RegionalEndpoint
    {
        public static string Eune => "https://eun1.api.riotgames.com";
        public static string Euw => "https://euw1.api.riotgames.com";
        public static string Na => "https://na1.api.riotgames.com";

        public static string ServerNameToUrl(string region)
        {
            switch (region)
            {
                case "euw":
                    return Euw;
                case "eune":
                    return Eune;
                case "na":
                    return Na;
                default:
                    throw new RegionNotFoundException();
            }
        }
    }
}
