using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain;
using TodoList.Persistence;

namespace API;

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

    public static IServiceCollection AddMapster(this IServiceCollection services,
        Action<TypeAdapterConfig>? options = null)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.Load("TodoList.Application"));

        options?.Invoke(config);

        services.AddSingleton(config);
        services.AddScoped<IMapper, Mapper>();

        return services;
    }
}