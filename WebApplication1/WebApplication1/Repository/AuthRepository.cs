using WebApplication1.Model;

namespace WebApplication1.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly List<User> _users = new();
    public Task<User?> FindByUsernameAsync(string username)
        => Task.FromResult(_users.FirstOrDefault(u => u.Name == username));

    public Task<User?> FindByIdAsync(long userId)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));

    public Task DeleteAsync(User user)
    {
        _users.Remove(user);
        return Task.CompletedTask;
    }

    public Task AddAsync(User user)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }
}