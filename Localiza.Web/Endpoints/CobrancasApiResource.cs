using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Localiza.Web.DAL;

public static class CobrancasApiResource
{
    public static async Task<int> CreateCobrancaAsync(this IDbConnection connection, Cobranca cobranca)
    {        
        const string checkClienteSql = "SELECT COUNT(1) FROM Clientes WHERE Id = @ClienteId;";
        var clienteExists = await connection.ExecuteScalarAsync<bool>(checkClienteSql, new { ClienteId = cobranca.ClienteId });

        if (!clienteExists)
        {
            throw new KeyNotFoundException($"Cliente with Id {cobranca.ClienteId} does not exist.");
        }

        const string sql = @"
            INSERT INTO Cobrancas (ClienteId, Valor, DataVencimento, Pago, Descricao)
            VALUES (@ClienteId, @Valor, @DataVencimento, @Pago, @Descricao)
            RETURNING Id;";

        return await connection.ExecuteScalarAsync<int>(sql, cobranca);
    }
    public static async Task<Cobranca?> GetCobrancaByIdAsync(this IDbConnection connection, int id){
        const string sql = @"SELECT * FROM Cobrancas WHERE Id = @Id;";
        return await connection.QueryFirstOrDefaultAsync<Cobranca>(sql,id);
    }
    public static async Task<IEnumerable<Cobranca>> ListCobrancasAsync(
        this IDbConnection connection
        ,int ClienteId){
        const string sql = @"SELECT * FROM Cobrancas WHERE ClienteId = @ClienteId";
        return await connection.QueryAsync<Cobranca>(sql,new { ClienteId });
    }

    public static async Task<Cobranca?> UpdateCobrancaAsync(this IDbConnection conn, Cobranca cobranca)
    {
        var existingCobranca = await conn.GetCobrancaByIdAsync(cobranca.Id);
        if (existingCobranca is null)
        {
            //Nota: lançar exceção?
            return null;
        }
        if (existingCobranca == cobranca)
        {
            return cobranca;
        }
        var updatedCobranca = existingCobranca with
        {
            DataVencimento = cobranca.DataVencimento
            ,Valor = cobranca.Valor
            ,ClienteId = cobranca.ClienteId
            ,Pago = cobranca.Pago
            ,Descricao = cobranca.Descricao
        };
        return updatedCobranca;
    }

    public static async Task<bool> DeleteCobrancaAsync(this IDbConnection conn,int cobrancaId)
    {
        const string sql = "DELETE FROM Cobrancas WHERE Id = @CobrancaId";
        var affectedRows = await conn.ExecuteAsync(sql,new {CobrancaId = cobrancaId});
        return affectedRows > 0;
    }
    public static void RegisterCobrancaEndpoints(this WebApplication app)
    {
        app.MapPost("api/cobrancas", async (
            Cobranca cobranca
            , [FromServices]DbConnectionFactory factory) =>
        {
            try
            {
                using var db = factory();
                var id = await db.CreateCobrancaAsync(cobranca);
                return Results.Created($"api/cobrancas/{id}", id);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        });
        app.MapGet("api/cobrancas/{id:int}",async (
            int id
            ,[FromServices]DbConnectionFactory factory
            ,[FromServices]ILogger<CreateCobrancaRequest> logger) => {
            try
            {
                using var db = factory();
                var cobranca = await db.GetCobrancaByIdAsync(id);
                return Results.Ok(cobranca);
            }
            catch (Exception ex)
            {
                logger.LogError("an error was throwed when trying to consult a cobranca -> {ex}",ex);
                return Results.StatusCode(500);
            }
        });
        app.MapGet("api/clientes/{id:int}/cobrancas",async (
            int id
            ,[FromServices]DbConnectionFactory factory
            ,[FromServices]ILogger<CreateCobrancaRequest> logger) => {
            try
            {
                using var db = factory();
                var cobranca = await db.ListCobrancasAsync(id);
                return Results.Ok(cobranca);
            }
            catch (Exception ex)
            {
                logger.LogError("an error was throwed when trying to consult a cobranca -> {ex}",ex);
                return Results.StatusCode(500);
            }
        });
        app.MapPut("api/cobrancas/{id:int}",async (
            int id
            ,[FromBody]Cobranca cobranca
            ,[FromServices]DbConnectionFactory connectionFactory
            ,[FromServices]ILogger<Cobranca> logger) => {
            try
            {
                if (id != cobranca.Id)
                {
                    return Results.BadRequest(new
                    {
                        Message = "invalid attempt to update a entry of another Id"
                    });
                }
                using var db = connectionFactory();
                var updatedCobranca = await db.UpdateCobrancaAsync(cobranca);
                return Results.Ok(updatedCobranca);
            }
            catch (System.Exception ex)
            {
                logger.LogError("an error was throwed when trying to update a entry in Cobrancas -> {ex}",ex);
                return Results.StatusCode(500);
            }
        });
        app.MapDelete("api/cobrancas/{id:int}", async (
            int id
            , [FromServices] DbConnectionFactory connectionFactory
            , [FromServices] ILogger<Cobranca> logger) =>
        {
            try
            {
                using var db = connectionFactory();
                if (await db.DeleteCobrancaAsync(id))
                {
                    return Results.Ok();    
                }
                return Results.NoContent();
            }
            catch (System.Exception ex)
            {
                logger.LogError("an error was throwed when trying to update a entry in Cobrancas -> {ex}",ex);
                return Results.StatusCode(500);                
            }
        });
    }
}
public record CreateCobrancaRequest {
    public int ClienteId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    // public bool Pago { get; set; }
    public string Descricao { get; set; } = default!;
}
public record Cobranca
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public bool Pago { get; set; }
    public string Descricao { get; set; } = default!;
}
