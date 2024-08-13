using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Localiza.Services;
using Localiza.Services.Identity;
using Localiza.Web.DAL;
using Localiza.Web.Helpers;
using Localiza.Web.Security;
using Localiza.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.
// builder.Environment.
var connStr = builder.Configuration.GetConnectionString("Default");
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DbConnectionFactory>(sp =>
{
    // var connStr = builder.Configuration.GetConnectionString("Default");        
    // return Connection.Factory(connStr);
    return () => {            
        var conn = new NpgsqlConnection(connStr);        
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
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "";
        if(string.IsNullOrWhiteSpace(jwtKey)){
            throw new InvalidConfigurationException("configuration key 'Jwt:Key' is not present or empty");
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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
await Cli.Run(app, args);
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

