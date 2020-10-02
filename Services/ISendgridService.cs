using System;
using System.Threading.Tasks;
using LeagueAPI.Models;

namespace LeagueAPI.Services
{
    public interface ISendgridService
    {
        Task SendMail(ContactDto contactViewModel);
    }
}
