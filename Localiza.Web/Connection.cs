using System.Data;
using Npgsql;

namespace Localiza.DAL;
public delegate IDbConnection CreateConnectionFactory();
public static class Connection
{
    public static CreateConnectionFactory Factory(string connectionString){
        
        return () => {
            Console.WriteLine("connStr:",connectionString);
            return new NpgsqlConnection(connectionString);
        };
    }
}