using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeagueAPI.Models;
using Microsoft.Extensions.Configuration;
using LeagueAPI.Services;
using LeagueAPI.Exceptions;
using Flurl;

namespace LeagueAPI.Controllers
{
    public class WebPageController : Controller
    {
        private readonly IRiotApiService riotApiService;
        private readonly ISendgridService sendgridService;

        public WebPageController(IRiotApiService riotApiService, ISendgridService sendgridService)
        {
            this.riotApiService = riotApiService ?? throw new ArgumentNullException(nameof(riotApiService));
            this.sendgridService = sendgridService ?? throw new ArgumentNullException(nameof(sendgridService));
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("generate-link")]
        public IActionResult GenerateLink()
        {
            return View();
        }

        [HttpPost("generate-link")]
        public async Task<IActionResult> GenerateLink(GenerateLinkDto generateLinkViewModel)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                var playerId = await riotApiService.FetchPlayerId(generateLinkViewModel.Nickname, generateLinkViewModel.Server);

            }
            catch (PlayerNotFoundException)
            {
                ModelState.AddModelError("PlayerNotFound", "Player not found try again with different name, or change your server.");
                return View();
            }
            catch (RegionNotFoundException)
            {
                ModelState.AddModelError("RegionNotFound", "Selected region not found.");
                return View();
            }

            string link = $"{Request.Scheme}://{Request.Host}"
                            .AppendPathSegment(generateLinkViewModel.Api)
                            .AppendPathSegment(generateLinkViewModel.Nickname)
                            .AppendPathSegment(generateLinkViewModel.Server);

            if (generateLinkViewModel.Api == "mastery")
                link = link.AppendPathSegment("<replace with bot argument>");

            return RedirectToAction("Link",new LinkViewModel
            {
                Link = link
            });
        }

        [HttpGet("link")]
        public IActionResult Link(LinkViewModel linkViewModel)
        {
            return View(linkViewModel);
        }

        [HttpGet("how-to-setup")]
        public IActionResult HowToSetup()
        {
            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("contact")]
        public async Task<IActionResult> Contact(ContactDto contactViewModel)
        {
            if (!ModelState.IsValid)
                return View(contactViewModel);

            await sendgridService.SendMail(contactViewModel);

            return RedirectToAction("EmailSend");
        }

        [HttpGet("email-send")]
        public IActionResult EmailSend()
        {
            return View();
        }

        [HttpGet("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
