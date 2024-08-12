using Localiza.Web.DAL;

namespace Localiza.Web.Helpers;

public static class Cli
{
    public static async Task Run(WebApplication app,params string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        if(args.Contains("--seed"))
        {
            var factory = app.Services.GetRequiredService<DbConnectionFactory>();
            var logger = app.Services.GetRequiredService<ILogger<EntryPoint>>();
            await Seeder.SeedDatabase(factory, logger);
        }
        
    }
}