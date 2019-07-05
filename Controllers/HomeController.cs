using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeagueAPI.Models;
using Microsoft.Extensions.Configuration;

namespace LeagueAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateLink()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateLink(GenerateLinkViewModel generateLinkViewModel)
        {
            var apiKey = _configuration.GetValue("ApiKey", "null");

            if(!ModelState.IsValid)
                return View();

            var playerId = await PlayerApiHelper.FetchPlayerId(generateLinkViewModel.Nickname, generateLinkViewModel.Server,apiKey);

            if (playerId == null)
            {
                ModelState.AddModelError("PlayerNotFound","Player not found try again with different name, or change your server");
                return View();
            }

            if (generateLinkViewModel.Region == "na")
            {
                playerId = await PlayerApiHelper.FetchPlayerId(generateLinkViewModel.Nickname, "na1",apiKey);
                if (playerId == null)
                {
                    ModelState.AddModelError("PlayerNotFound","Player not found try again with different name, or change your server");
                    return View();
                }
            }

            string link = null;

            if (generateLinkViewModel.Api == "mastery")
                link = string.Format(
                    "{0}://{1}/api/PlayerStats/Mastery/{2}/<replace with bot argument>?server={3}&cultureName={4}",
                    Request.Scheme, Request.Host, generateLinkViewModel.Nickname, generateLinkViewModel.Server,
                    generateLinkViewModel.Region);
            else
                link = string.Format(
                    "{0}://{1}/api/PlayerStats/{2}/{3}?server={4}&cultureName={5}", Request.Scheme, Request.Host,
                    generateLinkViewModel.Api, generateLinkViewModel.Nickname, generateLinkViewModel.Server,
                    generateLinkViewModel.Region);

            return RedirectToAction("Link",new LinkViewModel
            {
                Link = link
            });
        }

        [HttpGet]
        public IActionResult Link(LinkViewModel linkViewModel)
        {
            return View(linkViewModel);
        }

        public IActionResult HowToSetup()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
