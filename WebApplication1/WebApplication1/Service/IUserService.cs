using WebApplication1.Model;

namespace WebApplication1.Service;

public interface IUserService
{
    User? GetById(long id);
    void Create(User user);

}