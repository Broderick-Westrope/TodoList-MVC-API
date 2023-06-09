using System.Net;
using System.Net.Http.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.MVC.API;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests.Project;
using TodoList.MVC.API.Requests.TodoItem;
using TodoList.MVC.API.Requests.User;
using TodoList.MVC.API.Responses.Project;
using TodoList.MVC.API.Responses.TodoItem;

namespace E2E.Tests;

public class ProjectControllerShould: IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string Url = "api/Project";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Stack<Guid> _userIds = new();
    private readonly TodoContext _todoContext;
    private readonly IServiceScope _scope;

    public ProjectControllerShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _scope = _factory.Services.CreateScope();
        _todoContext = (TodoContext) _scope.ServiceProvider.GetRequiredService(typeof(TodoContext));
    }

    private async Task AddUser(User user)
    {
        _userIds.Push(user.Id);
        await _todoContext.Users.AddAsync(user);
        await _todoContext.SaveChangesAsync();
    }

    private async Task AddProject(Project project)
    {
        await _todoContext.Projects.AddAsync(project);
        await _todoContext.SaveChangesAsync();
    }

    [Theory]
    [AutoData]
    //TODO: lazy loading with proxies vs using a DTO for the project values in this test?
    public async Task GetProject(CreateUserRequest createUserRequestObj, Project project)
    {
        // arrange
        await AddUser(new User(project.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddProject(project);
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{Url}/{project.Id}");

        // assert
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var getProjectResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetProjectResponse>();
        getProjectResponseObj
            .Should()
            .NotBeNull();
        getProjectResponseObj!
            .Id
            .Should()
            .Be(project.Id);
        getProjectResponseObj
            .Title
            .Should()
            .Be(project.Title);
        getProjectResponseObj
            .UserId
            .Should()
            .Be(project.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task GetAllProjects(CreateUserRequest createUserRequestObj, Project project)
    {
        // arrange
        await AddUser(new User(project.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddProject(project);
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{Url}");

        // assert
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        
        var getProjectResponseObjs = (await getResponseMsg.Content.ReadFromJsonAsync<GetAllProjectsResponse>())?.Projects;
        getProjectResponseObjs
            .Should()
            .NotBeNull();
        getProjectResponseObjs!
            .Count
            .Should()
            .BePositive();
        getProjectResponseObjs
            .Should()
            .Contain(x =>
                x.Id == project.Id && 
                x.Title == project.Title &&
                x.UserId == project.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task PostProject(CreateUserRequest createUserRequestObj, CreateProjectRequest createProjectRequestObj)
    {
        // arrange
        await AddUser(new User(createProjectRequestObj.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(Url, createProjectRequestObj);

        // assert
        postResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.Created);
        
        var createProjectResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateProjectResponse>();
        createProjectResponseObj
            .Should()
            .NotBeNull();
        createProjectResponseObj!
            .Id
            .Should()
            .NotBeEmpty();
        createProjectResponseObj
            .Title
            .Should()
            .Be(createProjectResponseObj.Title);
        createProjectResponseObj
            .UserId
            .Should()
            .Be(createProjectRequestObj.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task PutProject(CreateUserRequest createUserRequestObj, Project project, UpdateProjectRequest updateProjectRequestObj)
    {
        // arrange
        await AddUser(new User(project.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddProject(project);
        var client = _factory.CreateClient();

        // act
        //? Possible to restrict updateProjectRequestObj.UserId to being the same as createProjectRequestObj.UserId?
        var putResponseMsg = await client.PutAsJsonAsync($"{Url}/{project.Id}", updateProjectRequestObj with { UserId = project.UserId});

        // assert
        putResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var task = _todoContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        task.Result
            .Should()
            .NotBeNull();
        var result = task.Result;
        result!
            .Id
            .Should()
            .Be(project.Id);
        result
            .Title
            .Should()
            .Be(updateProjectRequestObj.Title);
        result
            .UserId
            .Should()
            .Be(updateProjectRequestObj.UserId);
    }

    [Theory]
    [AutoData]
    public async Task DeleteProject(CreateUserRequest createUserRequestObj, Project project)
    {
        // arrange
        await AddUser(new User(project.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddProject(project);
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{Url}/{project.Id}");

        // assert
        deleteResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var result = await _todoContext.Projects.FirstOrDefaultAsync(x=>x.Id == project.Id);
        result
            .Should()
            .BeNull();
    }
    
    public async void Dispose()
    {
        while (_userIds.Count > 0)
        {
            var userId = _userIds.Pop();
            var user = await _todoContext.Users.FirstAsync(x=> x!.Id == userId);
            _todoContext.Users.Remove(user);
            await _todoContext.SaveChangesAsync();
        }
        _scope.Dispose();
    }
}