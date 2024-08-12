using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Localiza.Web.DAL;
using System.Collections.Immutable;

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
        const string queryCobrancasSql = @"
            SELECT * FROM Cobrancas WHERE Id = @Id
        ";
        var cliente = await connection.QuerySingleOrDefaultAsync<Cliente>(sql, new { Id = id });
        if(cliente is null){
            return null;
        }
        var cobrancas = await connection.QueryAsync<Cobranca>(queryCobrancasSql,new { Id = id });        
        return cliente with {
            Cobrancas = cobrancas.ToImmutableList()
        };
    }

    public static async Task<IEnumerable<Cliente>> GetAllClientesAsync(this IDbConnection connection,int UsuarioId)
    {
        const string sql = @"
            SELECT Id, UsuarioId, Documento, Nome, Telefone, Endereco
            FROM Clientes WHERE UsuarioId = @UsuarioId;
            select c.id,cl.usuarioid ,c.clienteid,c.valor,c.datavencimento,c.pago,c.descricao from cobrancas c
	            join clientes cl on cl.id = c.clienteid 		
	            join usuarios usr on usr.id  = cl.usuarioid 
            where cl.usuarioid = @UsuarioId";        
        using var resultSet = await connection.QueryMultipleAsync(sql);
        var clientes = await resultSet.ReadAsync<Cliente>();
        if(clientes is null){
            return [];
        }
        var cobrancas = await resultSet.ReadAsync<Cobranca>();
        if(cobrancas is null){
            return clientes;
        }
        return clientes.Select(cliente => cliente with {
            Cobrancas = cobrancas.Where(cobranca => cobranca.ClienteId == cliente.Id).ToImmutableList()
        });        
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

public record Cliente()
{
    public int Id { get; init; }
    public int UsuarioId { get; init; }
    public string Documento { get; init; } = default!;
    public string Nome { get; init; } = default!;
    public string Telefone { get; init; } = default!;
    public string Endereco { get; init; } = default!;
    public IReadOnlyList<Cobranca> Cobrancas { get; init; } = default!;
}
