using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Localiza.DAL;
using Localiza.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
static async Task SeedDatabase(CreateConnectionFactory factory, ILogger<EntryPoint> logger)
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
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Environment.
var connStr = builder.Configuration.GetConnectionString("Default");
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<CreateConnectionFactory>(sp =>
{
    // var connStr = builder.Configuration.GetConnectionString("Default");        
    // return Connection.Factory(connStr);
    return () => {            
        var conn = new NpgsqlConnection(connStr);
        Console.WriteLine("connStr:{0}",connStr);
        return conn;
    };
});
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<IUserService,StandardUserService>();
// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt => {
    opt.AddPolicy("default",policy => {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            // .WithOrigins("http://localhost:5046","https://localhost:7046","https://localhost:44466")
            .Build();
    });
});
var app = builder.Build();
if(args.Length > 0){      
    if(args.Contains("--seed"))
    {
        var factory = app.Services.GetRequiredService<CreateConnectionFactory>();
        var logger = app.Services.GetRequiredService<ILogger<EntryPoint>>();
        await SeedDatabase(factory, logger);
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("default");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");;

app.RegisterClienteEndpoints();
app.RegisterCobrancaEndpoints();
app.RegisterIdentityEndpoints();

app.Run();
public partial class EntryPoint;

