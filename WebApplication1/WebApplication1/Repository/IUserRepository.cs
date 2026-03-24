using WebApplication1.Model;

namespace WebApplication1.Repository;

public interface IUserRepository
{
    User? GetById(long id);
    void Create(User user);
}