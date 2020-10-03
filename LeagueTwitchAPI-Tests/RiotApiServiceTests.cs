using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using LeagueAPI;
using LeagueAPI.Exceptions;
using LeagueAPI.Models;
using LeagueAPI.Models.ChampionsDTO;
using LeagueAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LeagueTwitchAPI_Tests
{
    public class RiotApiServiceTests
    {
        [Fact]
        public async Task FetchPlayerIdPlayerNotFound()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var player = new SummonerDTO();
                var mockMemoryCache = new Mock<IMemoryCache>();
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(mockMemoryCache.Object, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 404);

                //Act
                await Assert.ThrowsAsync<PlayerNotFoundException>(() => riotApiService.FetchPlayerId(playerName, regionName));

                //Assert
                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{playerName}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);
            }

        }

        [Fact]
        public async Task FetchPlayerIdSucces()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var player = new SummonerDTO
                {
                    Id = Guid.NewGuid().ToString()
                };
                var mockMemoryCache = new Mock<IMemoryCache>();
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(mockMemoryCache.Object, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 200);

                //Act

                var result = await riotApiService.FetchPlayerId(playerName, regionName);

                //Assert
                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{playerName}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);
                Assert.Equal(player.Id, result);
            }

        }

        [Fact]
        public async Task GetDivisionsNull()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var player = new SummonerDTO
                {
                    Id = Guid.NewGuid().ToString()
                };
                var leagueEntryDTOs = new LeagueEntryDTO[0];
                var mockMemoryCache = new Mock<IMemoryCache>();
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(mockMemoryCache.Object, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 200);
                httpTest.RespondWithJson(leagueEntryDTOs, 200);

                //Act

                var result = await riotApiService.GetDivisions(playerName, regionName);

                //Assert
                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{playerName}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/league/v4/entries/by-summoner/{player.Id}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                Assert.Equal("Player got no division on any queue", result);
            }

        }

        [Fact]
        public async Task GetDivisionsSuccess()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var player = new SummonerDTO
                {
                    Id = Guid.NewGuid().ToString()
                };
                var LeagueEntryDTOs = new LeagueEntryDTO[]
                {
                    new LeagueEntryDTO
                    {
                        QueueType = "RANKED_SOLO_5x5",
                        Tier = "Test",
                        Rank = "Test",
                        LeaguePoints = 0
                    }
                };
                var mockMemoryCache = new Mock<IMemoryCache>();
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(mockMemoryCache.Object, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 200);
                httpTest.RespondWithJson(LeagueEntryDTOs, 200);

                //Act

                var result = await riotApiService.GetDivisions(playerName, regionName);

                //Assert
                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{playerName}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/league/v4/entries/by-summoner/{player.Id}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                Assert.Equal("Solo Division: Test Test (0 LP)\r\n", result);
            }

        }

        [Fact]
        public async Task GetMasteryNull()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var championName = "Unexisting Champion";
                var player = new SummonerDTO
                {
                    Id = Guid.NewGuid().ToString()
                };
                var championDTOs = new ChampionDTO[]
                {
                    new ChampionDTO
                    {
                        Name = "Test"
                    }
                };
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(memoryCache, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 200);
                httpTest.RespondWithJson(championDTOs, 200);

                //Act

                await Assert.ThrowsAsync<ChampionNotFoundException>(() => riotApiService.GetMastery(playerName, regionName, championName));

                //Assert
                httpTest.ShouldHaveCalled($"http://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);
            }

        }

        [Fact]
        public async Task GetMasterySuccess()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var championName = "Test";
                var championId = 10;
                var player = new SummonerDTO
                {
                    Id = Guid.NewGuid().ToString()
                };
                var championDTOs = new ChampionDTO[]
                {
                    new ChampionDTO
                    {
                        Name = "Test",
                        Id = championId
                    }
                };
                var championMasteryDTO = new ChampionMasteryDTO
                {
                    ChampionLevel = 1,
                    ChampionPoints = "2000"
                };
                var memoryCache = new MemoryCache(new MemoryCacheOptions());
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(memoryCache, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 200);
                httpTest.RespondWithJson(championDTOs, 200);
                httpTest.RespondWithJson(championMasteryDTO, 200);

                //Act

                var result = await riotApiService.GetMastery(playerName, regionName, championName);

                //Assert
                httpTest.ShouldHaveCalled($"http://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-summoner/{player.Id}/by-champion/{championId}")
                         .WithQueryParams("api_key")
                         .WithVerb(HttpMethod.Get)
                         .Times(1);

                Assert.Equal($"Champion level {championMasteryDTO.ChampionLevel} with {championName} ({championMasteryDTO.ChampionPoints ?? "0"} points)", result);
            }

        }

        [Fact]
        public async Task GetWinRatioNull()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var player = new SummonerDTO
                {
                    Id = Guid.NewGuid().ToString()
                };
                var leagueEntryDTOs = new LeagueEntryDTO[0];
                var mockMemoryCache = new Mock<IMemoryCache>();
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(mockMemoryCache.Object, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 200);
                httpTest.RespondWithJson(leagueEntryDTOs, 200);

                //Act

                var result = await riotApiService.GetWinRatio(playerName, regionName);

                //Assert
                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{playerName}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/league/v4/entries/by-summoner/{player.Id}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                Assert.Equal("Player got no division on any queue", result);
            }

        }

        [Fact]
        public async Task GetWinRatioSuccess()
        {
            using (var httpTest = new HttpTest())
            {
                //Arrange
                var playerName = "TestingPlayer";
                var regionName = "eune";
                var wins = 10;
                var losses = 10;
                var player = new SummonerDTO
                {
                    Id = Guid.NewGuid().ToString()
                };
                var LeagueEntryDTOs = new LeagueEntryDTO[]
                {
                    new LeagueEntryDTO
                    {
                        QueueType = "RANKED_SOLO_5x5",
                        Wins = wins,
                        Losses = losses
                    }
                };
                var mockMemoryCache = new Mock<IMemoryCache>();
                var mockIConfigration = new Mock<IConfiguration>();
                mockIConfigration.Setup(c => c[Constants.RIOT_APIKEY]).Returns("RiotApiKey");
                var riotApiService = new RiotApiService(mockMemoryCache.Object, mockIConfigration.Object);
                httpTest.RespondWithJson(player, 200);
                httpTest.RespondWithJson(LeagueEntryDTOs, 200);

                //Act

                var result = await riotApiService.GetWinRatio(playerName, regionName);

                //Assert
                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{playerName}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                httpTest.ShouldHaveCalled($"https://eun1.api.riotgames.com/lol/league/v4/entries/by-summoner/{player.Id}")
                        .WithQueryParams("api_key")
                        .WithVerb(HttpMethod.Get)
                        .Times(1);

                var winRatio = 100 * wins / (wins + losses);
                Assert.Equal($"Solo Win Ratio: {winRatio}%\r\n", result);
            }
        }
    }
}
