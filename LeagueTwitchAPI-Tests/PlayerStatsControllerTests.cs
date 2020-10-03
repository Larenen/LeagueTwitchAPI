using System;
using System.Threading.Tasks;
using LeagueAPI.Controllers;
using LeagueAPI.Exceptions;
using LeagueAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LeagueTwitchAPI_Tests
{
    public class PlayerStatsControllerTests
    {
        [Fact]
        public async Task DivisionsSuccess()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Divisions";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetDivisions(playerName, playerRegion)).ReturnsAsync(expected);
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Divisions(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var returnValue = Assert.IsType<string>(actionResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task DivisionsPlayerNotFound()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Player not found";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetDivisions(playerName, playerRegion)).ThrowsAsync(new PlayerNotFoundException());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Divisions(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(400, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task DivisionsRegionNotFound()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Region not found";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetDivisions(playerName, playerRegion)).ThrowsAsync(new RegionNotFoundException());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Divisions(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(400, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task DivisionsGenericError()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Server error";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetDivisions(playerName, playerRegion)).ThrowsAsync(new Exception());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Divisions(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task MasterySuccess()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var championName = "Testing Champion";
            var expected = "Mastery";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetMastery(playerName, playerRegion, championName)).ReturnsAsync(expected);
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Mastery(playerName, playerRegion, championName);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var returnValue = Assert.IsType<string>(actionResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task MasteryPlayerNotFound()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var championName = "Testing Champion";
            var expected = "Player not found";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetMastery(playerName, playerRegion, championName)).ThrowsAsync(new PlayerNotFoundException());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Mastery(playerName, playerRegion, championName);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(400, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task MasteryRegionNotFound()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var championName = "Testing Champion";
            var expected = "Region not found";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetMastery(playerName, playerRegion, championName)).ThrowsAsync(new RegionNotFoundException());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Mastery(playerName, playerRegion, championName);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(400, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task MasteryChampionNotFound()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var championName = "Testing Champion";
            var expected = "Champion not found";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetMastery(playerName, playerRegion, championName)).ThrowsAsync(new ChampionNotFoundException());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Mastery(playerName, playerRegion, championName);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(400, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task MasteryGenericError()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var championName = "Testing Champion";
            var expected = "Server error";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetMastery(playerName, playerRegion, championName)).ThrowsAsync(new Exception());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Mastery(playerName, playerRegion, championName);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task WinRateSuccess()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Win Ratio";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetWinRatio(playerName, playerRegion)).ReturnsAsync(expected);
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.WinRate(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var returnValue = Assert.IsType<string>(actionResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task WinRatePlayerNotFound()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Player not found";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetWinRatio(playerName, playerRegion)).ThrowsAsync(new PlayerNotFoundException());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.WinRate(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(400, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task WinRateRegionNotFound()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Region not found";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetWinRatio(playerName, playerRegion)).ThrowsAsync(new RegionNotFoundException());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.WinRate(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(400, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task WinRateGenericError()
        {
            //Arrange
            var playerName = "Testing Name";
            var playerRegion = "Testing Region";
            var expected = "Server error";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetWinRatio(playerName, playerRegion)).ThrowsAsync(new Exception());
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.WinRate(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, objectResult.StatusCode);
            var returnValue = Assert.IsType<string>(objectResult.Value);
            Assert.Equal(expected, returnValue);
        }
    }
}
