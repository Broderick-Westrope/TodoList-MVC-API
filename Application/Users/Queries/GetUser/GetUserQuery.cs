using MediatR;
using TodoList.Application.Responses.User;

namespace TodoList.Application.Users.Queries;

public record GetUserQuery : IRequest<GetUserResponse?>
{
    public Guid Id { get; set; }
}