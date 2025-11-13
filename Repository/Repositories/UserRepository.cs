namespace Repository.Repositories;

using Data;
using Domain.Entities;
using Interfaces;

public class UserRepository : IRepository<User>
{
    private readonly MyDbContext _db;
    public UserRepository(MyDbContext context)
    {
        _db = context;
    }
    
    public IEnumerable<User?> Get()
    {
        try
        {
            return _db.Users.ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public User? GetById(int id)
    {
        try
        {
            return _db.Users.Find(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public bool Create(User user)
    {
        try
        {
            _db.Users.Add(user);
            _db.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public bool Update(User user)
    {
        try
        {
            _db.Users.Update(user);
            _db.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var user = _db.Users.Find(id);
            if (user != null)
                _db.Users.Remove(user);
            _db.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}