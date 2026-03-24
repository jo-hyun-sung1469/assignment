using WebApplication1.Model;
using WebApplication1.Repository;

namespace WebApplication1.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public User? GetById(long id)
        => _userRepository.GetById(id);

    public void Create(User user)
        =>  _userRepository.Create(user);
}