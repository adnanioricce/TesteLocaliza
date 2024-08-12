namespace Localiza.Web.Services;

public interface IUserService
{
    Task<Usuario?> Authenticate(LoginRequest request);
}