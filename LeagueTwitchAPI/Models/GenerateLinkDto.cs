using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LeagueAPI.Attributes;

namespace LeagueAPI.Models
{
    public class GenerateLinkDto
    {
        [Required]
        [MaxLength(20)]
        [StringInArrayValidation(Values = new string[] { "divisions", "mastery", "winRate" })]
        public string Api { get; set; }

        [Required]
        [StringInArrayValidation(Values = new string[] { "euw", "eune", "na" })]
        public string Server { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nickname { get; set; }
    }
}
