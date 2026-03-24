using WebApplication1.Model;

namespace WebApplication1.Repository;

public interface IAuthRepository
{
    Task<User?> FindByUsernameAsync(string username);
    Task<User?> FindByIdAsync(long userId);
    Task DeleteAsync(User user);
    Task AddAsync(User user);
}