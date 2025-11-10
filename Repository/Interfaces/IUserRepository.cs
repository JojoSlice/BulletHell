namespace Repository.Interfaces;

public interface IUserRepository
{
    public List<User> Get();
    public User GetById();
    public bool Create();
    public bool Update();
    public bool Delete();
}