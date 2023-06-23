using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain;

namespace TodoList.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AZURE_SQL_CONNECTION_STRING");

        services.AddDbContext<TodoContext>(
            opt => opt
                .UseSqlServer(connectionString, options => options.EnableRetryOnFailure()));
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}