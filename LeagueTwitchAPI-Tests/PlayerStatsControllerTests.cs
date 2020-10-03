using System;
using System.Threading.Tasks;
using LeagueAPI.Controllers;
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
            var playerDivisions = "Divisions";
            var mockRiotApiService = new Mock<IRiotApiService>();
            mockRiotApiService.Setup(r => r.GetDivisions(playerName, playerRegion)).ReturnsAsync(playerDivisions);
            var controller = new PlayerStatsController(mockRiotApiService.Object);

            //Act
            var result = await controller.Divisions(playerName, playerRegion);

            //Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var returnValue = Assert.IsType<string>(actionResult.Value);
            Assert.Equal(playerDivisions, returnValue);
        }
    }
}
