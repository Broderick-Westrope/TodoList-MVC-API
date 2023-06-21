using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.MVC.API;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests.Project;
using TodoList.MVC.API.Responses.Project;

namespace E2E.Tests;

public class ProjectControllerShould : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string ProjectsUrl = "api/Projects";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Fixture _fixture = new();
    private readonly Stack<Guid> _userIds = new();

    public ProjectControllerShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        while (_userIds.Count > 0)
        {
            var userId = _userIds.Pop();
            var user = context.Users.First(x => x!.Id == userId);
            context.Users.Remove(user);
            context.SaveChanges();
        }
    }

    private async Task AddUserToDb(UserAggregate userAggregate)
    {
        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        _userIds.Push(userAggregate.Id);
        await context.Users.AddAsync(userAggregate);
        await context.SaveChangesAsync();
    }

    [Theory]
    [AutoData]
    public async Task GetProject(UserAggregate user, Project project)
    {
        // arrange
        user.AddProject(project);
        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{ProjectsUrl}/{project.Id}");

        // assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getProjectResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetProjectResponse>();
        getProjectResponseObj.Should().NotBeNull();
        getProjectResponseObj!.Id.Should().Be(project.Id);
        getProjectResponseObj.Title.Should().Be(project.Title);
    }

    [Theory]
    [AutoData]
    public async Task CreateProject(UserAggregate user)
    {
        // arrange
        var createProjectRequestObj = _fixture.Build<CreateProjectRequest>()
            .With(x => x.UserId, user.Id).Create();

        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(ProjectsUrl, createProjectRequestObj);

        // assert
        postResponseMsg.StatusCode.Should().Be(HttpStatusCode.Created);

        var createProjectResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateProjectResponse>();
        createProjectResponseObj.Should().NotBeNull();
        createProjectResponseObj!.Id.Should().NotBeEmpty();
        createProjectResponseObj.Title.Should().Be(createProjectResponseObj.Title);
    }

    [Theory]
    [AutoData]
    public async Task UpdateProject(UpdateProjectRequest updateProjectRequestObj, Project project, UserAggregate user)
    {
        // arrange
        user.AddProject(project);
        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var putResponseMsg = await client.PutAsJsonAsync($"{ProjectsUrl}/{project.Id}", updateProjectRequestObj);

        // assert
        putResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result =
            (await context.Users.FirstAsync(x => x.Projects.FirstOrDefault(y => y.Id == project.Id) != null)).Projects
            .First(x => x.Id == project.Id);

        result.Should().BeEquivalentTo(updateProjectRequestObj);
    }

    [Theory]
    [AutoData]
    public async Task DeleteProject(UserAggregate user, Project project)
    {
        // arrange
        user.AddProject(project);
        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{ProjectsUrl}/{project.Id}");

        // assert
        deleteResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(x =>
            x.Projects != null && x.Projects.FirstOrDefault(y => y.Id == project.Id) != null);
        ;
        result.Should().BeNull();
    }
}