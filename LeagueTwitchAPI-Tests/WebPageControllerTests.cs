using System;
using System.Threading.Tasks;
using LeagueAPI.Controllers;
using LeagueAPI.Exceptions;
using LeagueAPI.Models;
using LeagueAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LeagueTwitchAPI_Tests
{
    public class WebPageControllerTests
    {
        [Fact]
        public async Task GenerateLinkModelStateInvalid()
        {
            //Arrange
            var generatLinkDto = new GenerateLinkDto();
            var mockRiotService = new Mock<IRiotApiService>();
            var mockSendgridService = new Mock<ISendgridService>();
            var controller = new WebPageController(mockRiotService.Object, mockSendgridService.Object);
            controller.ModelState.AddModelError("Testing Error","Error");

            //Act
            var result = await controller.GenerateLink(generatLinkDto);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.ModelState.TryGetValue("Testing Error", out var error);
            Assert.Equal("Error", error.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task GenerateLinkPlayerNotFound()
        {
            //Arrange
            var playerName = "Test Name";
            var playerRegion = "Test Region";
            var generatLinkDto = new GenerateLinkDto
            {
                Nickname = playerName,
                Server = playerRegion
            };
            var mockRiotService = new Mock<IRiotApiService>();
            mockRiotService.Setup(rs => rs.FetchPlayerId(playerName, playerRegion)).ThrowsAsync(new PlayerNotFoundException());
            var mockSendgridService = new Mock<ISendgridService>();
            var controller = new WebPageController(mockRiotService.Object, mockSendgridService.Object);

            //Act
            var result = await controller.GenerateLink(generatLinkDto);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.ModelState.TryGetValue("PlayerNotFound", out var error);
            Assert.Equal("Player not found try again with different name, or change your server.", error.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task GenerateLinkRegionNotFound()
        {
            //Arrange
            var playerName = "Test Name";
            var playerRegion = "Test Region";
            var generatLinkDto = new GenerateLinkDto
            {
                Nickname = playerName,
                Server = playerRegion
            };
            var mockRiotService = new Mock<IRiotApiService>();
            mockRiotService.Setup(rs => rs.FetchPlayerId(playerName, playerRegion)).ThrowsAsync(new RegionNotFoundException());
            var mockSendgridService = new Mock<ISendgridService>();
            var controller = new WebPageController(mockRiotService.Object, mockSendgridService.Object);

            //Act
            var result = await controller.GenerateLink(generatLinkDto);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.ModelState.TryGetValue("RegionNotFound", out var error);
            Assert.Equal("Selected region not found.", error.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task GenerateLinkSuccess()
        {
            //Arrange
            var playerName = "Test Name";
            var playerRegion = "Test Region";
            var generatLinkDto = new GenerateLinkDto
            {
                Api = "Test Api",
                Nickname = playerName,
                Server = playerRegion
            };
            var mockRiotService = new Mock<IRiotApiService>();
            mockRiotService.Setup(rs => rs.FetchPlayerId(playerName, playerRegion)).ReturnsAsync("ID");
            var mockSendgridService = new Mock<ISendgridService>();
            var controller = new WebPageController(mockRiotService.Object, mockSendgridService.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            //Act
            var result = await controller.GenerateLink(generatLinkDto);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Link", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ContactModelStateInvalid()
        {
            //Arrange
            var contactDto = new ContactDto();
            var mockRiotService = new Mock<IRiotApiService>();
            var mockSendgridService = new Mock<ISendgridService>();
            var controller = new WebPageController(mockRiotService.Object, mockSendgridService.Object);
            controller.ModelState.AddModelError("Testing Error", "Error");

            //Act
            var result = await controller.Contact(contactDto);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.ModelState.TryGetValue("Testing Error", out var error);
            Assert.Equal("Error", error.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task ContactSucces()
        {
            //Arrange
            var contactDto = new ContactDto
            {
                Email = "Test@test.pl",
                Message = "Test Message",
                Name = "Test Name"
            };
            var mockRiotService = new Mock<IRiotApiService>();
            var mockSendgridService = new Mock<ISendgridService>();
            mockSendgridService.Setup(s => s.SendMail(contactDto)).Verifiable();
            var controller = new WebPageController(mockRiotService.Object, mockSendgridService.Object);

            //Act
            var result = await controller.Contact(contactDto);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            mockSendgridService.Verify();
            Assert.Equal("EmailSend", redirectToActionResult.ActionName);
        }
    }
}
