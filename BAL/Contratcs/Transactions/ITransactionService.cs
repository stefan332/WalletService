using BAL.Models.Requests;

namespace BAL.Contratcs.Transactions
{
    public interface ITransactionService
    {
        Task<Data.Domain.Transaction> AddFundsAsync(AddOrRemoveFundsRequest addOrRemoveFundsRequest);
        Task<Data.Domain.Transaction> RemoveFundsAsync(AddOrRemoveFundsRequest addOrRemoveFundsRequest);
    }
}
