using System;
using System.Threading.Tasks;
using LeagueAPI.Exceptions;
using LeagueAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeagueAPI.Controllers
{
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
                return StatusCode(StatusCodes.Status400BadRequest, "Player not found");
            }
            catch (RegionNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Region not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
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
                return StatusCode(StatusCodes.Status400BadRequest, "Player not found");
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
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
                return StatusCode(StatusCodes.Status400BadRequest, "Player not found");
            }
            catch (RegionNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Region not found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }
    }
}