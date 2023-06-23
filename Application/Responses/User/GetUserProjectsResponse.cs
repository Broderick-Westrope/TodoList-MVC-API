namespace TodoList.Application.Responses.Project;

public record GetUserProjectsResponse(List<GetProjectResponse> Projects);