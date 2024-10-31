using Data.Domain;

namespace BAL.Contratcs.TransactionTypes
{
    public interface ITransactionTypeService
    {
        Task<IEnumerable<TransactionType>> GetAllAsync();
        Task<TransactionType?> GetByIdAsync(int id);
        Task<TransactionType> CreateAsync(TransactionType transactionType);
    }
}
