using Microsoft.EntityFrameworkCore;
using TodoList.MVC.API;
using TodoList.MVC.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTION_STRING");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(
    opt => opt.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString);

var app = builder.Build();

// Required outside of dev. env. for Azure API Management
app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();

public partial class Program{}