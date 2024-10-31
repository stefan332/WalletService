namespace Data
{
    public interface IStore
    {
        IQueryable<T> Query<T>() where T : class;
        Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
        void Remove<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void RemoveRange<T>(List<T> entities) where T : class;
    }
}