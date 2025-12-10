using DemoTienda.Application.Interfaces;
using DemoTienda.Application.Services;
using DemoTienda.Application.Settings;
using DemoTienda.Infrastructure.Auth;
using DemoTienda.Infrastructure.Context;
using DemoTienda.Infrastructure.Extensions;
using DemoTiendaAPIController.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Mapster;
using DemoTienda.Application.Mapping;
using MapsterMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using DemoTienda.Application.Validators;
using DemoTienda.Application.Interfaces;
using DemoTienda.Infrastructure.Configuration;
using DemoTienda.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mapsterConfig = TypeAdapterConfig.GlobalSettings;
MapsterConfiguration.Register(mapsterConfig);

builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, Mapper>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoriaRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoriaRequestValidator>();

builder.Services.Configure<AzureStorageSettings>(
    builder.Configuration.GetSection("AzureStorage"));

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.AddDbContext<DemoTiendaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DemoTienda")));

builder.Services
    .AddOptions<ProductoSettings>()
    .Bind(builder.Configuration.GetSection("ProductoSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddInfrastructure();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "DemoTienda API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese el token generado por /api/auth/login \r\nEjemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DemoTiendaContext>()
    .AddSignInManager<SignInManager<AppUser>>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()!;

var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);
var signingKey = new SymmetricSecurityKey(keyBytes);

builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CatalogRead", policy =>
        policy.RequireAuthenticatedUser());

    options.AddPolicy("CatalogWrite", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("ollama", client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
});
builder.Services.AddScoped<IDescripcionProductoIAService, DescripcionProductoIAService>();


var app = builder.Build();

app.UseMiddleware<DemoTienda.Api.Middlewares.GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoTienda API v1");
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedAsync(userManager, roleManager);
}

app.Run();
