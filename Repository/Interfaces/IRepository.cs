namespace Repository.Interfaces;

public interface IRepository<T>
{
    public IEnumerable<T?> Get();
    public T? GetById(int id);
    public bool Create(T entity);
    public bool Update(T entity);
    public bool Delete(int id);
}