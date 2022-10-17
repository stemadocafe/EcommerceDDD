using MediatR;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EcommerceDDD.IdentityServer.Database;
using EcommerceDDD.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EcommerceDDD.Core.Infrastructure.Identity;
using EcommerceDDD.Core.Infrastructure;
using EcommerceDDD.IdentityServer.Configurations;

var builder = WebApplication.CreateBuilder(args);

// ---- Services
var tokenIssuerSettings = builder.Configuration.GetSection("TokenIssuerSettings");
builder.Services.Configure<TokenIssuerSettings>(tokenIssuerSettings); 
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies()); 
builder.Services.AddScoped<IdentityApplicationDbContext>();
builder.Services.AddScoped<ITokenRequester, TokenRequester>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// ---- AspNet.Core.Identity settings
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Program)
    .GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddDbContext<IdentityApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddIdentityServer(opt =>
    opt.IssuerUri = tokenIssuerSettings.GetValue<string>("Authority"))
        opt.IssuerUri = tokenIssuerSettings.GetValue<string>("Authority"))
    .AddDeveloperSigningCredential() // without a certificate, for dev only
    .AddDeveloperSigningCredential() // without a certificate, for dev only
    .AddAspNetIdentity<ApplicationUser>()
    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
    .AddConfigurationStore(options =>
    .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
    .AddInMemoryClients(IdentityConfiguration.Clients)
    .AddOperationalStore(options =>
    {
    {
        options.ConfigureDbContext = b => 
        options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
            b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
        options.EnableTokenCleanup = true;
    })
    })
    .AddOperationalStore(options =>
    .AddConfigurationStore(options =>
    {
    {
        options.ConfigureDbContext = b => 
        options.ConfigureDbContext = b =>
            b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
        options.EnableTokenCleanup = true;
    })
    });
    .AddAspNetIdentity<ApplicationUser>();

// ---- Cors
builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy", builder => {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200");
    }
));

// ---- App
var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();

app.Run();
