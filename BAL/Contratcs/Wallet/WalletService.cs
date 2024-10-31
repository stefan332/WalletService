using Data;
using Data.Domain;
using Microsoft.EntityFrameworkCore;
using Shared;
using Microsoft.Extensions.Logging;
using BAL.Models.Requests;

namespace BAL.Contratcs
{
    public class WalletService : IWalletService
    {
        private readonly IStore _store;
        private readonly ILogger<WalletService> _logger;
        public WalletService(IStore store, ILogger<WalletService> logger)
        {
            _store = store;
            _logger = logger;
        }


        public async Task<Wallet> GetWalletByUserAndWalletId(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            var wallet = await _store.Query<Wallet>()
                .FirstOrDefaultAsync(w => w.UserId == addOrRemoveFundsRequest.UserId && w.Id == addOrRemoveFundsRequest.WalletID);
            if (wallet == null) throw new NotFoundException("Wallet for this player was not found");
            return wallet;
        }

        public async Task<Wallet> CreateWalletAsync(string userId)
        {
            var wallet = new Wallet { UserId = userId, Balance = 0 };
            await _store.AddAsync(wallet);
            await _store.SaveChangesAsync();
            return wallet;
        }

        public async Task<decimal> GetBalanceAsync(string userId)
        {
            var wallet = await _store.Query<Wallet>()
                .FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null) throw new NotFoundException("Wallet not found");
            return wallet.Balance;
        }

        public async Task<Wallet> GetWalletAsync(int walletId)
        {
            try
            {
                var wallet = await _store.Query<Wallet>().FirstOrDefaultAsync(w => w.Id == walletId);
                if (wallet == null) throw new NotFoundException("Wallet not found");

                return wallet;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                throw;
            }
        }

        public async Task UpdateWallet(Wallet wallet, decimal amount, bool removeFunds)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            if (!removeFunds)
                wallet.Balance += amount;
            else wallet.Balance -= amount;
            await _store.SaveChangesAsync();
        }

    }
}
