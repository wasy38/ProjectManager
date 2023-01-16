namespace ProjectManager.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<T> GetRepository<T>() where T : class;
        public void Save();
        public Task SaveChangesAsync();
    }
}
