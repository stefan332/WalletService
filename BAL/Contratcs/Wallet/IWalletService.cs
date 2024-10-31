using BAL.Models.Requests;
using Data.Domain;

namespace BAL.Contratcs
{
    public interface IWalletService
    {
        Task<Wallet> GetWalletByUserAndWalletId(AddOrRemoveFundsRequest addOrRemoveFundsRequest);
        Task<decimal> GetBalanceAsync(string userId);
        Task<Wallet> CreateWalletAsync(string userId);
        Task<Wallet> GetWalletAsync(int walletId);
        Task UpdateWallet(Wallet wallet, decimal amount, bool removeFunds);
    }
}
