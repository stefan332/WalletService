using Data;
using Shared;
using Data.Domain;
using BAL.Contratcs.TransactionTypes;
using Microsoft.EntityFrameworkCore;
using BAL.Models.Requests;

namespace BAL.Contratcs.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IWalletService _walletService;
        private readonly IStore _store;
        private readonly ITransactionTypeService _transactionTypeService;

        public TransactionService(IWalletService walletService, IStore store ,ITransactionTypeService transactionTypeService)
        {
            _walletService = walletService;
            _store = store;
            _transactionTypeService = transactionTypeService;   
        }

        public async Task<Data.Domain.Transaction> AddFundsAsync(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            var wallet = await _walletService.GetWalletByUserAndWalletId(addOrRemoveFundsRequest);
            if (wallet == null) throw new NotFoundException("Wallet not found");

            var transaction = new Data.Domain.Transaction
            {
                WalletId = addOrRemoveFundsRequest.WalletID,
                Amount = addOrRemoveFundsRequest.Amount,
                TransactionTypeId = addOrRemoveFundsRequest.TransactionTypeId,
                Date = DateTime.UtcNow
            };
            var transactionType = await _transactionTypeService.GetByIdAsync(addOrRemoveFundsRequest.TransactionTypeId);
            if (transactionType == null)
            {
                transactionType = new TransactionType
                {
                    Description = TransactionTypeEnumHellper.MapDescrition(TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId)),
                    Name = TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId).ToString()
                };
                await _transactionTypeService.CreateAsync(transactionType);
            }

            await _store.AddAsync(transaction);
            await _walletService.UpdateWallet(wallet, addOrRemoveFundsRequest.Amount, false);

            return transaction;
        }

        public async Task<Data.Domain.Transaction> RemoveFundsAsync(AddOrRemoveFundsRequest addOrRemoveFundsRequest)
        {
            var wallet = await _walletService.GetWalletByUserAndWalletId(addOrRemoveFundsRequest);
            if (wallet == null) throw new NotFoundException("Wallet not found");
            if (wallet.Balance < addOrRemoveFundsRequest.Amount) throw new InvalidOperationException("Insufficient funds");

            var existingTransactions = await _store.Query<Data.Domain.Transaction>().Where(x => x.WalletId == addOrRemoveFundsRequest.WalletID).ToListAsync();
            if(existingTransactions.Any())
            {
                bool hasSpentSameAmount = existingTransactions.Any(t => t.Amount == -addOrRemoveFundsRequest.Amount && t.TransactionTypeId == addOrRemoveFundsRequest.TransactionTypeId);
                if (hasSpentSameAmount) throw new AllReadyExistsException("The same funds cannot be spent twice.");
            }

            var transactionType = await _transactionTypeService.GetByIdAsync(addOrRemoveFundsRequest.TransactionTypeId);
            if (transactionType == null)
            {
                transactionType = new TransactionType
                {
                    Description = TransactionTypeEnumHellper.MapDescrition(TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId)),
                    Name = TransactionTypeEnumHellper.GetTransactionTypeById(addOrRemoveFundsRequest.TransactionTypeId).ToString()
                };
                await _transactionTypeService.CreateAsync(transactionType);
            }

            var transaction = new Data.Domain.Transaction
            {
                WalletId = addOrRemoveFundsRequest.WalletID,
                Amount = -addOrRemoveFundsRequest.Amount,  // Negative to indicate deduction
                TransactionTypeId = addOrRemoveFundsRequest.WalletID,
                Date = DateTime.UtcNow
            };
            await _store.AddAsync(transaction);
            await _walletService.UpdateWallet(wallet, addOrRemoveFundsRequest.Amount, true);

            return transaction;
        }
    }
}
