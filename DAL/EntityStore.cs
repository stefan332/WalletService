using Data;
using Microsoft.EntityFrameworkCore;


namespace DAL
{
    internal class EntityStore : IStore
    {
        private readonly IDbContext _dbContext;

        public EntityStore(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            ValidateArgument(entity);
            var set = GetSet<T>();
            await set.AddAsync(entity, cancellationToken);
        }

        private static void ValidateArgument<T>(T entity) where T : class
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return GetSet<T>().AsQueryable();
        }

        public void Remove<T>(T entity) where T : class
        {
            ValidateArgument(entity);
            GetSet<T>().Remove(entity);
        }

        public void RemoveRange<T>(List<T> entities) where T : class
        {

            foreach (var entity in entities)
            {
                ValidateArgument(entity);
                GetSet<T>().Remove(entity);

            }

        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Update<T>(T entity) where T : class
        {
            ValidateArgument(entity);
            GetSet<T>().Update(entity);
        }

        private DbSet<T> GetSet<T>() where T : class
        {
            return _dbContext.Set<T>();
        }
    }
}
