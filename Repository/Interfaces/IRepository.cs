namespace Repository.Interfaces;

public interface IRepository<T>
{
    public Task<List<T>> GetAllAsync();
    public Task<T?> GetByIdAsync(int id);
    public Task<T> CreateAsync(T entity);
    public Task<T> UpdateAsync(T entity);
    public Task<string> DeleteAsync(int id);
}

