using Localiza.DAL;
using Localiza.Security;

namespace Localiza.Services;

public class StandardUserService(CreateConnectionFactory _createConnection) : IUserService
{    
    private readonly CreateConnectionFactory _createConnection = _createConnection;    
    public async Task<Usuario?> Authenticate(string username, string password)
    {        
        using var conn = _createConnection();
        return await conn.AuthenticateUsuarioAsync(new LoginRequest(username,password));
    }
}