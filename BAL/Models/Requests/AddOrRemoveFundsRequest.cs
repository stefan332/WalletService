

using System.ComponentModel.DataAnnotations;

namespace BAL.Models.Requests
{
    public class AddOrRemoveFundsRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int WalletID { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int TransactionTypeId { get; set; }
    }
}
