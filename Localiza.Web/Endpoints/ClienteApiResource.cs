using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Localiza.Web.DAL;

public static class ClienteApiResource
{    
    public static async Task<int> CreateClienteAsync(this IDbConnection connection, Cliente cliente)
    {
        const string sql = @"
            INSERT INTO Clientes (UsuarioId, Documento, Nome, Telefone, Endereco)
            VALUES (@UsuarioId, @Documento, @Nome, @Telefone, @Endereco)
            RETURNING Id;";

        return await connection.ExecuteScalarAsync<int>(sql, cliente);
    }

    public static async Task<Cliente?> GetClienteByIdAsync(this IDbConnection connection, int id)
    {
        const string sql = @"
            SELECT Id, UsuarioId, Documento, Nome, Telefone, Endereco
            FROM Clientes
            WHERE Id = @Id;";

        return await connection.QuerySingleOrDefaultAsync<Cliente>(sql, new { Id = id });
    }

    public static async Task<IEnumerable<Cliente>> GetAllClientesAsync(this IDbConnection connection,int UsuarioId)
    {
        const string sql = @"
            SELECT Id, UsuarioId, Documento, Nome, Telefone, Endereco
            FROM Clientes WHERE UsuarioId = @UsuarioId;";

        return await connection.QueryAsync<Cliente>(sql,new {UsuarioId});
    }

    public static async Task<bool> UpdateClienteAsync(this IDbConnection connection, Cliente cliente)
    {
        const string sql = @"
            UPDATE Clientes
            SET UsuarioId = @UsuarioId,
                Documento = @Documento,
                Nome = @Nome,
                Telefone = @Telefone,
                Endereco = @Endereco
            WHERE Id = @Id;";

        var affectedRows = await connection.ExecuteAsync(sql, cliente);
        return affectedRows > 0;
    }

    public static async Task<bool> DeleteClienteAsync(this IDbConnection connection, int id)
    {
        const string sql = @"
            DELETE FROM Cobrancas WHERE ClienteId = @Id;
            DELETE FROM Clientes
            WHERE Id = @Id;";

        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }
    public static void RegisterClienteEndpoints(this WebApplication app)
    {
        app.MapPost("api/clientes", CreateClienteAsyncEndpoint());

        app.MapGet("api/clientes/{id:int}", GetClienteByIdAsyncEndpoint());

        app.MapGet("api/clientes/{id:int}/list", ListClientesAsyncEndpoint());

        app.MapPut("api/clientes/{id:int}", UpdateClienteAsyncEndpoint());

        app.MapDelete("api/clientes/{id:int}", DeleteClienteAsyncEndpoint());
    }

    private static Func<int, DbConnectionFactory, Task<IResult>> DeleteClienteAsyncEndpoint()
    {
        return async (int id, [FromServices]DbConnectionFactory factory) =>
        {
            using var db = factory();
            var deleted = await db.DeleteClienteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        };
    }

    private static Func<int, Cliente, DbConnectionFactory, Task<IResult>> UpdateClienteAsyncEndpoint()
    {
        return async (int id, Cliente cliente, [FromServices]DbConnectionFactory factory) =>
        {
            using var db = factory();
            if (id != cliente.Id)
            {
                return Results.BadRequest();
            }

            var updated = await db.UpdateClienteAsync(cliente);
            return updated ? Results.Ok(cliente) : Results.NotFound();
        };
    }

    private static Func<int,DbConnectionFactory,Task<IResult>> ListClientesAsyncEndpoint()
    {
        return async (int id,[FromServices]DbConnectionFactory factory) =>
        {
            using var db = factory();
            var clientes = await db.GetAllClientesAsync(id);
            return Results.Ok(clientes);
        };
    }

    private static Func<int, DbConnectionFactory, Task<IResult>> GetClienteByIdAsyncEndpoint()
    {
        return async (int id, [FromServices]DbConnectionFactory factory) =>
        {
            using var db = factory();
            var cliente = await db.GetClienteByIdAsync(id);
            return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
        };
    }

    private static Func<Cliente, DbConnectionFactory, Task<IResult>> CreateClienteAsyncEndpoint()
    {
        return async (Cliente cliente, [FromServices]DbConnectionFactory factory) =>
        {
            using var db = factory();
            var id = await db.CreateClienteAsync(cliente);
            return Results.Created($"/clientes/{id}", id);
        };
    }
}

public record Cliente
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Documento { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public string Telefone { get; set; } = default!;
    public string Endereco { get; set; } = default!;
}
