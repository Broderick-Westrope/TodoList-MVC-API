using TodoList.Application.Responses.Project;

namespace TodoList.Application.Responses.User;

public record GetUserProjectsResponse(List<GetProjectResponse> Projects);