using System.Text.Json;
using Localiza.Web.DAL;

namespace Localiza.Web.Helpers;

public static class Seeder
{
    public static async Task SeedDatabase(DbConnectionFactory factory, ILogger<EntryPoint> logger)
    {
        using var conn = factory();
        conn.Open();
        var usuarios = new List<Usuario>();
        foreach (var usuario in SeedDataGenerator.GenerateUsuarios(3))
        {
            var id = await conn.CreateUsuarioAsync(usuario, usuario.HashSenha);
            usuarios.Add(usuario with
            {
                Id = id
            });
        }
        logger.LogInformation("created users: {users}", JsonSerializer.Serialize(usuarios));
        var clientes = new List<Cliente>();
        foreach (var cliente in SeedDataGenerator.GenerateClientes(10, usuarios))
        {
            var id = await conn.CreateClienteAsync(cliente);
            clientes.Add(cliente with
            {
                Id = id
            });
        }
        logger.LogInformation("created clientes: {clientes}", JsonSerializer.Serialize(clientes));
        var cobrancas = new List<Cobranca>();
        foreach (var cliente in clientes)
        {
            foreach (var cobranca in SeedDataGenerator.GenerateCobrancas(10, cliente.Id))
            {
                var id = await conn.CreateCobrancaAsync(cobranca);
                cobrancas.Add(cobranca with
                {
                    Id = id
                });
            }
        }
        logger.LogInformation("created clientes: {cobrancas}", JsonSerializer.Serialize(cobrancas));
    
    }
}