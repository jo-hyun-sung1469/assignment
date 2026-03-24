using WebApplication1.Model;

namespace WebApplication1.Repository;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = [];
    
    public User? GetById(long id)
        => _users.FirstOrDefault(u => u.Id == id);

    public void Create(User user)
        => _users.Add(user);
}