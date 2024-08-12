using System.Data;
using Npgsql;

namespace Localiza.Web.DAL;
public delegate IDbConnection DbConnectionFactory();
public static class Connection
{
    public static DbConnectionFactory Factory(string connectionString)
        => () => new NpgsqlConnection(connectionString);
    
}