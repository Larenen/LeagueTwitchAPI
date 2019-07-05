using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LeagueAPI.Models;
using LeagueAPI.Models.ChampionsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LeagueAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerStatsController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _config;

        public PlayerStatsController(IMemoryCache memoryCache,IConfiguration config)
        {
            _memoryCache = memoryCache;
            _config = config;
        }

        [HttpGet("[action]/{playerName}")]
        public async Task<ActionResult<string>> Divisions(string playerName, string server, string cultureName)
        {
            var apiKey = _config.GetValue("ApiKey", "null");
             
            ResourceManager rm =
                new ResourceManager("LeagueAPI.Resources.Resource", typeof(PlayerStatsController).Assembly);
            CultureInfo culture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var playerId = await PlayerApiHelper.FetchPlayerId(playerName, server,apiKey);
            if (playerId == null)
                return NotFound(rm.GetString("PlayerNotFound"));

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string riotApiUrlPlayerInfo = PlayerApiHelper.NameToAddress(server);
                    if (riotApiUrlPlayerInfo == null)
                        return NotFound(rm.GetString("RegionNotFound"));

                    riotApiUrlPlayerInfo += "/lol/league/v4/entries/by-summoner/" + playerId +
                                            "?api_key=" + apiKey;

                    using (HttpResponseMessage response = await client.GetAsync(riotApiUrlPlayerInfo))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return NotFound(rm.GetString("ErrorPlayerInfo"));

                        var playerInfoData = await response.Content.ReadAsStringAsync();
                        LeagueEntryDTO[] leagueEntry = JsonConvert.DeserializeObject<LeagueEntryDTO[]>(playerInfoData);

                        if (leagueEntry.Length == 0)
                            return BadRequest(rm.GetString("NoDivision"));

                        string divisions = null;
                        foreach (var leagueEntryDto in leagueEntry)
                        {
                            if (leagueEntryDto.QueueType == "RANKED_SOLO_5x5")
                                divisions += string.Format(rm.GetString("SoloDivision"), leagueEntryDto.Tier,
                                    leagueEntryDto.Rank, leagueEntryDto.LeaguePoints);
                            else if (leagueEntryDto.QueueType == "RANKED_FLEX_SR")
                                divisions += string.Format(rm.GetString("FlexSRDivision"), leagueEntryDto.Tier,
                                    leagueEntryDto.Rank, leagueEntryDto.LeaguePoints);
                            else
                                divisions += string.Format(rm.GetString("FlexTTDivision"), leagueEntryDto.Tier,
                                    leagueEntryDto.Rank, leagueEntryDto.LeaguePoints);

                            if (leagueEntryDto.MiniSeries != null)
                            {
                                divisions += " " + rm.GetString("Series") + " " + leagueEntryDto.MiniSeries.Progress
                                                 .Replace('W', '✓')
                                                 .Replace('N', '-').Replace('L', 'X');
                            }

                            divisions += Environment.NewLine;
                        }

                        return divisions;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("------------Exception------------");
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, rm.GetString("Server500"));
            }
        }

        [HttpGet("[action]/{playerName}/{championName}")]
        public async Task<ActionResult<string>> Mastery(string playerName, string championName, string server,
            string cultureName)
        {
            var apiKey = _config.GetValue("ApiKey", "null");

            ResourceManager rm =
                new ResourceManager("LeagueAPI.Resources.Resource", typeof(PlayerStatsController).Assembly);
            CultureInfo culture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var playerId = await PlayerApiHelper.FetchPlayerId(playerName, server,apiKey);
            if (playerId == null)
                return NotFound(rm.GetString("PlayerNotFound"));

            var championsList = await ReturnChampionList();
            if (championsList == null)
                return NotFound(rm.GetString("ChampsError"));

            var champion = championsList.FirstOrDefault(n => string.Equals(n.Name.ToUpper(), championName.ToUpper(),
                StringComparison.CurrentCultureIgnoreCase));

            if (champion == null)
                return NotFound(rm.GetString("NoChamp"));

            string riotApiChampionMastery = PlayerApiHelper.NameToAddress(server);
            if (riotApiChampionMastery == null)
                return NotFound(rm.GetString("RegionNotFound"));

            riotApiChampionMastery += "/lol/champion-mastery/v4/champion-masteries/by-summoner/" + playerId +
                                      "/by-champion/" + champion.Id +
                                      "?api_key=" + apiKey;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(riotApiChampionMastery))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return Ok(string.Format(rm.GetString("ChampLVL"), 0, champion.Name, 0));

                        var playerMasteryData = await response.Content.ReadAsStringAsync();
                        var playerMastery = JsonConvert.DeserializeObject<ChampionMasteryDTO>(playerMasteryData);

                        if (playerMastery == null)
                            return BadRequest(rm.GetString("DessError"));

                        return string.Format(rm.GetString("ChampLVL"), playerMastery.ChampionLevel, champion.Name,
                            playerMastery.ChampionPoints);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("------------Exception------------");
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, rm.GetString("Server500"));
            }
        }

        [HttpGet("[action]/{playerName}")]
        public async Task<ActionResult<string>> WinRate(string playerName, string server, string cultureName)
        {
            var apiKey = _config.GetValue("ApiKey", "null");

            ResourceManager rm =
                new ResourceManager("LeagueAPI.Resources.Resource", typeof(PlayerStatsController).Assembly);
            CultureInfo culture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var playerId = await PlayerApiHelper.FetchPlayerId(playerName, server,apiKey);
            if (playerId == null)
                return NotFound("Player not found");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string riotApiUrlPlayerInfo = PlayerApiHelper.NameToAddress(server);
                    if (riotApiUrlPlayerInfo == null)
                        return NotFound(rm.GetString("RegionNotFound"));

                    riotApiUrlPlayerInfo += "/lol/league/v4/entries/by-summoner/" + playerId +
                                            "?api_key=" + apiKey;

                    using (HttpResponseMessage response = await client.GetAsync(riotApiUrlPlayerInfo))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return NotFound(rm.GetString("RegionNotFound"));

                        var playerInfoData = await response.Content.ReadAsStringAsync();
                        LeagueEntryDTO[] leagueEntry = JsonConvert.DeserializeObject<LeagueEntryDTO[]>(playerInfoData);

                        if (leagueEntry.Length == 0)
                            return BadRequest(rm.GetString("NoDivision"));

                        string winRate = null;
                        foreach (var leagueEntryDto in leagueEntry)
                        {
                            int winRatio = 100 * leagueEntryDto.Wins / (leagueEntryDto.Wins + leagueEntryDto.Losses);

                            if (leagueEntryDto.QueueType == "RANKED_SOLO_5x5")
                                winRate += string.Format(rm.GetString("SoloWinRatio"), winRatio);
                            else if (leagueEntryDto.QueueType == "RANKED_FLEX_SR")
                                winRate += string.Format(rm.GetString("FlexSRWinRatio"), winRatio);
                            else
                                winRate += string.Format(rm.GetString("FlexTTWinRatio"), winRatio);

                            winRate += Environment.NewLine;
                        }

                        return winRate;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("------------Exception------------");
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, rm.GetString("Server500"));
            }
        }

        private async Task<ChampionDTO[]> ReturnChampionList()
        {
            string riotApiChampionsURL =
                "http://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json";
            ChampionDTO[] championsList = null;

            if (!_memoryCache.TryGetValue("ChampionList", out championsList))
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(riotApiChampionsURL))
                    {
                        if (res.StatusCode != HttpStatusCode.OK)
                            return null;

                        var championsData = await res.Content.ReadAsStringAsync();
                        championsList = JsonConvert.DeserializeObject<ChampionDTO[]>(championsData);

                        var cacheEntryOptions =
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(7));
                        _memoryCache.Set("ChampionList", championsList, cacheEntryOptions);
                    }
                }
            }

            return championsList;
        }
    }
}