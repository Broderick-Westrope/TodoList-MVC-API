using System.Reflection;
using API;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTION_STRING");

// Add services to the container.
builder.Services.AddMapster();
builder.Services.RegisterMapsterConfiguration();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.Load("TodoList.Application")));
builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Required outside of dev. env. for Azure API Management
app.UseSwagger();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment()) 
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace API
{
    public abstract class Program
    {
    }
}