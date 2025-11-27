namespace Repository.Interfaces;

public interface IRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByUsernameAsync(string username);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<string> DeleteAsync(int id);
}
