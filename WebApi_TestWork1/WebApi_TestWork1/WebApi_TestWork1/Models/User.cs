using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi_TestWork1.Utils;

namespace WebApi_TestWork1.Models
{
    // The base user class for saving valute to userIdentityToken
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string UserIdentityToken { get; set; }
        [Required]
        public string ValuteName { get; set; }
        [Required]
        public decimal ValuteCount { get; set; }
    }
}
