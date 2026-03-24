using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Service;

namespace WebApplication1.Controller;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get([FromRoute] long id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return NotFound();
        
        return user;
    }
    
    [HttpPost]
    public ActionResult<User> Create([FromBody] User user)
    {
        _userService.Create(user);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }
    
    // public login
}