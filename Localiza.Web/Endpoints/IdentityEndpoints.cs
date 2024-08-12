using Dapper;
using Localiza.Security;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Threading.Tasks;
using Localiza.Web.DAL;
using Localiza.Web.Security;
using Localiza.Web.Services;

public record LoginRequest(string Username,string Password);
public static class IdentityApiResource
{    
    public static async Task<bool> UpdateUserPasswordAsync(this IDbConnection connection, int userId, string newPlainPassword)
    {
        var hashedPassword = PasswordHasher.HashPassword(newPlainPassword);

        const string sql = @"
            UPDATE Usuarios
            SET HashSenha = @HashSenha
            WHERE Id = @Id;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Id = userId, HashSenha = hashedPassword });
        return affectedRows > 0;
    }
    public static async Task<int> CreateUsuarioAsync(this IDbConnection connection, Usuario usuario, string plainPassword)
    {
        var updatedUser = usuario with {
            HashSenha = PasswordHasher.HashPassword(plainPassword)
        };

        const string sql = @"
            INSERT INTO Usuarios (Nome, Email, HashSenha)
            VALUES (@Nome, @Email, @HashSenha)
            RETURNING Id;";

        return await connection.ExecuteScalarAsync<int>(sql, updatedUser);
    }

    public static async Task<Usuario?> GetUsuarioByIdAsync(this IDbConnection connection, int id)
    {
        const string sql = @"
            SELECT Id, Nome, Email, HashSenha
            FROM Usuarios
            WHERE Id = @Id;";

        return await connection.QuerySingleOrDefaultAsync<Usuario>(sql, new { Id = id });
    }
    public static async Task<Usuario?> AuthenticateUsuarioAsync(this IDbConnection connection,LoginRequest req)
    {
        var passwordHash = PasswordHasher.HashPassword(req.Password);
        const string sql = @"
            SELECT Id, Nome, Email, HashSenha
            FROM Usuarios
            WHERE HashSenha = @HashSenha;";

        return await connection.QuerySingleOrDefaultAsync<Usuario>(sql, new { HashSenha = passwordHash });
    }
    public static async Task<IEnumerable<Usuario>> GetAllUsuariosAsync(this IDbConnection connection)
    {
        const string sql = @"
            SELECT Id, Nome, Email, HashSenha
            FROM Usuarios;";

        return await connection.QueryAsync<Usuario>(sql);
    }

    public static async Task<bool> UpdateUsuarioAsync(this IDbConnection connection, Usuario usuario)
    {
        const string sql = @"
            UPDATE Usuarios
            SET Nome = @Nome,
                Email = @Email,
                HashSenha = @HashSenha
            WHERE Id = @Id;";

        var affectedRows = await connection.ExecuteAsync(sql, usuario);
        return affectedRows > 0;
    }

    public static async Task<bool> DeleteUsuarioAsync(this IDbConnection connection, int id)
    {
        const string sql = @"
            DELETE FROM Usuarios
            WHERE Id = @Id;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }
    public static void RegisterIdentityEndpoints(this WebApplication app)
    {
        app.MapPost("api/auth/login",async (
            [FromBody]LoginRequest request
            , AuthService authService
            , IUserService userService) => {
                var user = await userService.Authenticate(request);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                var token = authService.GenerateToken(user);
                return Results.Ok(new { Token = token });
        });
        app.MapPost("api/usuarios", CreateUsuarioAsyncEndpoint());
        app.MapPut("api/usuarios/{id:int}", UpdateUsuarioAsyncEndpoint());
        app.MapPut("api/usuarios/{id:int}/password", UpdateUsuarioPasswordAsyncEndpoint());
        app.MapDelete("api/usuarios/{id:int}", DeleteUsuarioAsyncEndpoint());
    }

    public static Func<int, DbConnectionFactory, ILogger<CreateUsuarioRequest>, Task<IResult>> DeleteUsuarioAsyncEndpoint()
    {
        return async (int id, [FromServices]DbConnectionFactory factory,[FromServices] ILogger<CreateUsuarioRequest> logger) =>
        {
            try
            {
                using var db = factory();
                if (await db.DeleteUsuarioAsync(id))
                {
                    return Results.Ok();
                }
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError("An error ocurred while trying to delete user -> {ex}", ex);
                return Results.StatusCode(500);
            }
        };
    }

    public static Func<int, string, DbConnectionFactory, Task<IResult>> UpdateUsuarioPasswordAsyncEndpoint()
    {
        return async (int id, string newPassword, [FromServices]DbConnectionFactory factory) =>
        {
            using var db = factory();
            var updated = await db.UpdateUserPasswordAsync(id, newPassword);
            return updated ? Results.Ok() : Results.NotFound();
        };
    }

    public static Func<int, UpdateUsuarioRequest, DbConnectionFactory, Task<IResult>> UpdateUsuarioAsyncEndpoint()
    {
        return async (int id, [FromBody] UpdateUsuarioRequest req, [FromServices]DbConnectionFactory factory) =>
        {
            
            var user = new Usuario()
            {
                Id = req.Id,
                Nome = req.Nome,
                Email = req.Email,
            };
            using var db = factory();
            var updated = await db.UpdateUsuarioAsync(user);
            return updated ? Results.Ok() : Results.NotFound();
        };
    }

    public static Func<CreateUsuarioRequest, DbConnectionFactory, Task<IResult>> CreateUsuarioAsyncEndpoint()
    {
        return async ([FromBody] CreateUsuarioRequest req, [FromServices]DbConnectionFactory factory) =>
        {
            var usuario = new Usuario()
            {
                Nome = req.Nome,
                Email = req.Email
            };
            using var db = factory();
            var id = await db.CreateUsuarioAsync(usuario, req.Senha);
            return Results.Created($"/usuarios/{id}", id);
        };
    }
}
public record CreateUsuarioRequest {    
    public string Nome { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Senha { get; init; } = default!;
}
public record UpdateUsuarioRequest {
    public int Id { get; init; }
    public string Nome { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string HashSenha { get; init; } = default!;
}
public record Usuario
{
    public int Id { get; init; }
    public string Nome { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string HashSenha { get; init; } = default!;
}
