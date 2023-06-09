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
    private const string ProjectUrl = "api/Project";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Fixture _fixture = new();
    private readonly Stack<Guid> _userIds = new();

    public ProjectControllerShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        while (_userIds.Count > 0)
        {
            var userId = _userIds.Pop();
            var user = await context.Users.FirstAsync(x => x!.Id == userId);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    private async Task AddUserToDb(User user)
    {
        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        _userIds.Push(user.Id);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    private async Task AddProjectsToDb(IEnumerable<Project> projects)
    {
        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        foreach (var project in projects) await context.Projects.AddAsync(project);

        //? Is it bad practice to save the changes outside of the loop?
        await context.SaveChangesAsync();
    }

    [Theory]
    [AutoData]
    //TODO: lazy loading with proxies vs using a DTO for the project values in this test?
    public async Task GetProject(Project project)
    {
        // arrange
        //? A way to streamline user creation by making a method that returns the created fixture using the given userId? Worth it?
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects)
            .With(x => x.Id, project.UserId).Create();

        await AddUserToDb(user);
        await AddProjectsToDb(new[] { project });
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{ProjectUrl}/{project.Id}");

        // assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getProjectResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetProjectResponse>();
        getProjectResponseObj.Should().NotBeNull();
        getProjectResponseObj!.Id.Should().Be(project.Id);
        getProjectResponseObj.Title.Should().Be(project.Title);
        getProjectResponseObj.UserId.Should().Be(project.UserId);
    }

    [Theory]
    [AutoData]
    public async Task GetAllProjects(Project project)
    {
        // arrange
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects)
            .With(x => x.Id, project.UserId).Create();

        await AddUserToDb(user);
        await AddProjectsToDb(new[] { project });
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{ProjectUrl}");

        // assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getProjectResponseObjs =
            (await getResponseMsg.Content.ReadFromJsonAsync<GetAllProjectsResponse>())?.Projects;
        getProjectResponseObjs.Should().NotBeNull();
        getProjectResponseObjs!.Count.Should().BePositive();
        getProjectResponseObjs.Should().Contain(x =>
            x.Id == project.Id &&
            x.Title == project.Title &&
            x.UserId == project.UserId);
    }

    [Theory]
    [AutoData]
    public async Task PostProject(CreateProjectRequest createProjectRequestObj)
    {
        // arrange
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects)
            .With(x => x.Id, createProjectRequestObj.UserId).Create();

        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(ProjectUrl, createProjectRequestObj);

        // assert
        postResponseMsg.StatusCode.Should().Be(HttpStatusCode.Created);

        var createProjectResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateProjectResponse>();
        createProjectResponseObj.Should().NotBeNull();
        createProjectResponseObj!.Id.Should().NotBeEmpty();
        createProjectResponseObj.Title.Should().Be(createProjectResponseObj.Title);
        createProjectResponseObj.UserId.Should().Be(createProjectRequestObj.UserId);
    }

    [Theory]
    [AutoData]
    public async Task PutProject(UpdateProjectRequest updateProjectRequestObj)
    {
        // arrange
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects)
            .With(x => x.Id, updateProjectRequestObj.UserId).Create();
        var project = _fixture.Build<Project>()
            .With(x => x.UserId, updateProjectRequestObj.UserId).Create();

        await AddUserToDb(user);
        await AddProjectsToDb(new[] { project });
        var client = _factory.CreateClient();

        // act
        //? Possible to restrict updateProjectRequestObj.UserId to being the same as createProjectRequestObj.UserId?
        var putResponseMsg = await client.PutAsJsonAsync($"{ProjectUrl}/{project.Id}",
            updateProjectRequestObj with { UserId = project.UserId });

        // assert
        putResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        result!.Id.Should().Be(project.Id);
        result.Title.Should().Be(updateProjectRequestObj.Title);
        result.UserId.Should().Be(updateProjectRequestObj.UserId);
    }

    [Theory]
    [AutoData]
    public async Task DeleteProject(Project project)
    {
        // arrange
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects)
            .With(x => x.Id, project.UserId).Create();

        await AddUserToDb(user);
        await AddProjectsToDb(new[] { project });
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{ProjectUrl}/{project.Id}");

        // assert
        deleteResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Projects.FirstOrDefaultAsync(x => x.Id == project.Id);
        result.Should().BeNull();
    }
}