
using System.ComponentModel.DataAnnotations;

namespace BAL.Models.Requests
{
    public class CreateWalletDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
