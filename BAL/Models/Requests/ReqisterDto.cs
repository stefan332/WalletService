
using System.ComponentModel.DataAnnotations;

namespace BAL.Models.Requests
{
    public class ReqisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
