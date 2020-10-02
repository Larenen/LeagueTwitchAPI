using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Resources;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using LeagueAPI.Exceptions;
using LeagueAPI.Models;
using LeagueAPI.Models.ChampionsDTO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace LeagueAPI.Services
{
    public class RiotApiService : IRiotApiService
    {
        private readonly string riotApiKey;
        private readonly IMemoryCache memoryCache;

        public RiotApiService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            riotApiKey = configuration[Constants.RIOT_APIKEY] ?? throw new ArgumentNullException("Riot api key missing");
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<string> FetchPlayerId(string playerName, string server)
        {
            var address = RegionalEndpoint.ServerNameToUrl(server);

            var result = await address.AppendPathSegments("lol", "summoner", "v4", "summoners", "by-name", playerName)
                                      .AllowHttpStatus(HttpStatusCode.NotFound)
                                      .SetQueryParam("api_key", riotApiKey)
                                      .GetAsync()
                                      .ReceiveJson<SummonerDTO>();

            if (result.Id == null)
                throw new PlayerNotFoundException();

            return result.Id;
        }

        public async Task<string> GetDivisions(string playerName, string region)
        {
            var playerId = await FetchPlayerId(playerName, region);
            var riotApiAddress = RegionalEndpoint.ServerNameToUrl(region);
            var result = await riotApiAddress.AppendPathSegments("lol", "league", "v4", "entries", "by-summoner", playerId)
                          .SetQueryParam("api_key", riotApiKey)
                          .GetAsync()
                          .ReceiveJson<LeagueEntryDTO[]>();

            string divisions = "";
            if (divisions.Count() < 1)
                divisions += "Player got no division on any queue";

            foreach (var leagueEntry in result)
            {
                if (leagueEntry.QueueType == "RANKED_SOLO_5x5")
                    divisions += $"Solo Division: {leagueEntry.Tier} {leagueEntry.Rank} ({leagueEntry.LeaguePoints} LP)";

                else if (leagueEntry.QueueType == "RANKED_FLEX_SR")
                    divisions += $"Flex Summoner's Rift Division: {leagueEntry.Tier} {leagueEntry.Rank} ({leagueEntry.LeaguePoints} LP)";

                if (leagueEntry.MiniSeries != null)
                {
                    divisions = $"Series: {leagueEntry.MiniSeries.Progress.Replace('W', '✓').Replace('N', '-').Replace('L', 'X')}";
                }

                divisions += "\r\n";
            }

            return divisions;
        }

        public async Task<string> GetMastery(string playerName, string region, string championName)
        {
            var playerId = await FetchPlayerId(playerName, region);
            var champions = await returnChampionList();
            var champion = champions.SingleOrDefault(n => string.Equals(n.Name.ToLower(), championName.ToLower()));
            if (champion == null)
                throw new ChampionNotFoundException();

            var riotApiAddress = RegionalEndpoint.ServerNameToUrl(region);
            var result = await riotApiAddress
                                .AllowHttpStatus(HttpStatusCode.NotFound)
                                .AppendPathSegments("lol", "champion-mastery", "v4", "champion-masteries", "by-summoner", playerId, "by-champion", champion.Id)
                                .SetQueryParam("api_key", riotApiKey)
                                .GetAsync()
                                .ReceiveJson<ChampionMasteryDTO>();

            return $"Champion level {result.ChampionLevel} with {champion.Name} ({result.ChampionPoints ?? "0"} points)";
        }

        public async Task<string> GetWinRatio(string playerName, string region)
        {
            var playerId = await FetchPlayerId(playerName, region);
            var riotApiAddress = RegionalEndpoint.ServerNameToUrl(region);
            var result = await riotApiAddress
                            .AppendPathSegments("lol", "league", "v4", "entries", "by-summoner", playerId)
                            .SetQueryParam("api_key", riotApiKey)
                            .GetAsync()
                            .ReceiveJson<LeagueEntryDTO[]>();

            if (result.Length == 0)
                return "Player got no division on any queue";

            string winRates = "";
            foreach(var leagueEntry in result)
            {
                var winRatio = 100 * leagueEntry.Wins / (leagueEntry.Wins + leagueEntry.Losses);
                if (leagueEntry.QueueType == "RANKED_SOLO_5x5")
                    winRates += $"Solo Win Ratio: {winRatio}%";
                else if (leagueEntry.QueueType == "RANKED_FLEX_SR")
                    winRates += $"Flex Summoner's Rift Win Ratio: {winRatio}%";

                winRates += "\r\n";
            }

            return winRates;
        }

        private async Task<ChampionDTO[]> returnChampionList()
        {
            ChampionDTO[] champions;
            if (!memoryCache.TryGetValue("ChampionList", out champions))
            {
                champions = await "http://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json"
                    .GetAsync()
                    .ReceiveJson<ChampionDTO[]>();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
                memoryCache.Set("ChampionList", champions, cacheEntryOptions);
            }

            return champions;
        }
    }
}
