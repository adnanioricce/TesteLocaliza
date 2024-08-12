using Microsoft.AspNetCore.Mvc;
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
public interface IUserService
{
    Task<Usuario?> Authenticate(string username,string password);
}
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IUserService _userService; // Your user service for authentication

    public AuthController(AuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userService.Authenticate(model.Username, model.Password); // Your user authentication logic
        if (user == null)
        {
            return Unauthorized();
        }

        var token = _authService.GenerateToken(user);
        return Ok(new { Token = token });
    }
}
