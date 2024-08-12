using Localiza.Web.DAL;
using Localiza.Web.Services;

namespace Localiza.Services.Identity;

public class StandardUserService(DbConnectionFactory dbConnection) : IUserService
{    
    private readonly DbConnectionFactory _dbConnection = dbConnection;    
    public async Task<Usuario?> Authenticate(LoginRequest req)
    {
        using var conn = _dbConnection();
        return await conn.AuthenticateUsuarioAsync(req);
    }
}