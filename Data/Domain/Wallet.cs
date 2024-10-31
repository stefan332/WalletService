using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Data.Domain
{
    public class Wallet
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }
        public decimal Balance { get; set; }
        [Timestamp]
        public byte[]? RowVersion { get; set; }  // For optimistic concurrency

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

}
