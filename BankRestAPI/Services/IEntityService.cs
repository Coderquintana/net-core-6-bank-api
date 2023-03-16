namespace BankRestAPI.Services
{
    public interface IEntityService<T> where T : class
    {
        Task<IEnumerable<T?>> GetAll();
        Task<T?> GetById(Guid id);
        Task<T?> Update(T entity);
        Task Delete(Guid id);
        Task<T?> Create(T entity);
    }
}
