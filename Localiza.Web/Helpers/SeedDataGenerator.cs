using Bogus;
using Bogus.Extensions.Brazil;
namespace Localiza.Web.Helpers;
public static class SeedDataGenerator
{
    public static IEnumerable<Usuario> GenerateUsuarios(int count)
    {
        var id = 1;
        var faker = new Faker<Usuario>()
            .RuleFor(u => u.Id, _ => id++)
            .RuleFor(u => u.Nome, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.HashSenha, f => f.Internet.Password());

        return faker.Generate(count);
    }

    public static IEnumerable<Cobranca> GenerateCobrancas(int count, int clienteId)
    {
        var id = 1;
        var faker = new Faker<Cobranca>()
            .RuleFor(c => c.Id, _ => id++)
            .RuleFor(c => c.ClienteId, _ => clienteId)
            .RuleFor(c => c.Valor, f => f.Finance.Amount(100))
            .RuleFor(c => c.DataVencimento, f => f.Date.Future())
            .RuleFor(c => c.Pago, f => f.Random.Bool())
            .RuleFor(c => c.Descricao, f => f.Lorem.Sentence());

        return faker.Generate(count);
    }

    public static IEnumerable<Cliente> GenerateClientes(int count, IEnumerable<Usuario> usuarios)
    {
        var id = 1;
        var userList = new List<Usuario>(usuarios);
        var faker = new Faker<Cliente>()
            .RuleFor(c => c.Id, _ => id++)
            .RuleFor(c => c.UsuarioId, f => f.PickRandom(userList).Id)
            .RuleFor(c => c.Documento, f => f.Person.Cpf())
            .RuleFor(c => c.Nome, f => f.Person.FullName)
            .RuleFor(c => c.Telefone, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.Endereco, f => f.Address.FullAddress());

        return faker.Generate(count);
    }
}
