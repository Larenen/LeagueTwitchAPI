using System;
using System.Threading.Tasks;

namespace LeagueAPI.Services
{
    public interface IRiotApiService
    {
        Task<string> FetchPlayerId(string playerName, string region);
        Task<string> GetDivisions(string playerName, string region);
        Task<string> GetMastery(string playerName, string region, string championName);
        Task<string> GetWinRatio(string playerName, string region);
    }
}
