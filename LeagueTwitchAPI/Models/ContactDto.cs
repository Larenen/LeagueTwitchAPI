using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueAPI.Models
{
    public class ContactDto
    {
        [Required]
        [MinLength(5,ErrorMessage = "Name must be at least 5 letter long")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(10,ErrorMessage = "Message must be at least 10 letter long")]
        public string Message { get; set; }
    }
}
