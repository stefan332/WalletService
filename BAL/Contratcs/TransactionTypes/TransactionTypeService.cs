using Data;
using Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BAL.Contratcs.TransactionTypes
{
    public class TransactionTypeService : ITransactionTypeService
    {
        private readonly IStore _store;
        private readonly ILogger<TransactionTypeService> _logger;
        public TransactionTypeService(IStore store, ILogger<TransactionTypeService> logger)
        {
            _store = store;
        }
        public async Task<TransactionType> CreateAsync(TransactionType transactionType)
        {
            try
            {
                await _store.AddAsync(transactionType);
                return transactionType;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                throw;
            }                  
        }

        public async Task<IEnumerable<TransactionType>> GetAllAsync()
        {
            try
            {
                return await _store.Query<TransactionType>().ToListAsync();

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                throw;
            }
       
           
        }

        public async Task<TransactionType?> GetByIdAsync(int id)
        {
            try
            {
                var trasactionType = await _store.Query<TransactionType>().FirstOrDefaultAsync(x => x.TransactionTypeId == id);
                if (trasactionType == null) return null;
                return trasactionType;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                throw;
            }

        }
    }
}
