using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LeagueAPI.Models;
using Newtonsoft.Json;

namespace LeagueAPI
{
    public class PlayerApiHelper
    {
        public static async Task<string> FetchPlayerId(string playerName, string server,string apiKey)
        {
            var address = NameToAddress(server);
            if (address == null)
                return null;

            string riotApiUrlPlayer = address;

            riotApiUrlPlayer += "/lol/summoner/v4/summoners/by-name/" + playerName +
                                "?api_key=" + apiKey;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(riotApiUrlPlayer))
                    {
                        if (res.StatusCode != HttpStatusCode.OK)
                            return null;

                        var playerData = await res.Content.ReadAsStringAsync();
                        var player = JsonConvert.DeserializeObject<SummonerDTO>(playerData);

                        return player.Id;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("------------Exception------------");
                Console.WriteLine(exception);
                return null;
            }
        }

        public static string NameToAddress(string region)
        {
            string regionAddress = null;

            switch (region)
            {
                case "euw":
                    regionAddress += RegionalEndpoints.Euw;
                    break;
                case "eune":
                    regionAddress += RegionalEndpoints.Eune;
                    break;
                case "na":
                    regionAddress += RegionalEndpoints.Na;
                    break;
                case "na1":
                    regionAddress += RegionalEndpoints.Na1;
                    break;
            }

            return regionAddress;
        }
    }
}
