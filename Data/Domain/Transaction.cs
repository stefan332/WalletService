
namespace Data.Domain
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int WalletId { get; set; }
        public int TransactionTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Wallet Wallet { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
