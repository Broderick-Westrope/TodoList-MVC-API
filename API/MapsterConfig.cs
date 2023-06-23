using Mapster;
using TodoList.Application.Responses.User;
using TodoList.Domain.Entities;

namespace API;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<UserAggregate, GetUserResponse>
            .NewConfig()
            .Map(dest => dest.TodoItemIds, src => from todoItem in src.TodoItems select todoItem.Id)
            .Map(dest => dest.ProjectIds, src => from project in src.Projects select project.Id);
    }
}