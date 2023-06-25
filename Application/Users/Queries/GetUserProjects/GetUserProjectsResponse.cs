using TodoList.Application.Projects.Queries.GetProject;

namespace TodoList.Application.Users.Queries.GetUserProjects;

public record GetUserProjectsResponse(List<GetProjectResponse> Projects);