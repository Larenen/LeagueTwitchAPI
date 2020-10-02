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
using LeagueAPI.Exceptions;
using LeagueAPI.Models;
using LeagueAPI.Models.ChampionsDTO;
using LeagueAPI.Services;
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
        private readonly IRiotApiService riotApiService;

        public PlayerStatsController(IRiotApiService riotApiService)
        {
            this.riotApiService = riotApiService;
        }

        [HttpGet("[action]/{playerName}/{region}")]
        public async Task<ActionResult<string>> Divisions(string playerName, string region)
        {
            try
            {
                return await riotApiService.GetDivisions(playerName, region);
            }
            catch (PlayerNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Player not found.");
            }
            catch (RegionNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Region not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error.");
            }
        }

        [HttpGet("[action]/{playerName}/{region}/{championName}")]
        public async Task<ActionResult<string>> Mastery(string playerName, string region, string championName)
        {
            try
            {
                return await riotApiService.GetMastery(playerName, region, championName);
            }
            catch (PlayerNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Player not found.");
            }
            catch (RegionNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Region not found");
            }
            catch (ChampionNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Champion not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error.");
            }
        }

        [HttpGet("[action]/{playerName}/{region}")]
        public async Task<ActionResult<string>> WinRate(string playerName, string region)
        {
            try
            {
                return await riotApiService.GetWinRatio(playerName, region);
            }
            catch (PlayerNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Player not found.");
            }
            catch (RegionNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Region not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error.");
            }
        }
    }
}