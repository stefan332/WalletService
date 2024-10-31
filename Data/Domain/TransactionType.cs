

namespace Data.Domain
{
    public class TransactionType
    {
        public int TransactionTypeId { get; set; }
        public string Name { get; set; } // E.g., "Deposit", "Withdrawal"
        public string Description { get; set; } // Optional: additional details

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
