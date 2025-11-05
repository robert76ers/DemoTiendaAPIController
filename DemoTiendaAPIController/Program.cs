using DemoTienda.Application.Services;
using DemoTienda.Infrastructure.Extensions;
using DemoTiendaAPIController.Data;
using DemoTiendaAPIController.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
