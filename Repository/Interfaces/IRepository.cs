namespace Repository.Interfaces;

public interface IRepository<T>
{
    public List<T> Get();
    public T GetById();
    public bool Create();
    public bool Update();
    public bool Delete();
}