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
using TodoList.MVC.API.Requests.User;
using TodoList.MVC.API.Responses.Project;
using TodoList.MVC.API.Responses.TodoItem;
using TodoList.MVC.API.Responses.User;

namespace E2E.Tests;

public class UserControllerShould : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string UsersUrl = "api/Users";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Fixture _fixture = new();
    private readonly Stack<Guid> _userIds = new();

    public UserControllerShould(WebApplicationFactory<Program> factory)
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
            var user = await context.Users.FirstAsync(x => x.Id == userId);
            context.Users.Remove(user);
        }

        //? Is it bad practice to save the changes outside of the loop?
        await context.SaveChangesAsync();
    }

    private async Task AddUsersToDb(IEnumerable<UserAggregate> users)
    {
        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        foreach (var user in users)
        {
            _userIds.Push(user.Id);
            await context.Users.AddAsync(user);
        }

        //? Is it bad practice to save the changes outside of the loop?
        await context.SaveChangesAsync();
    }

    [Theory]
    [AutoData]
    public async Task GetUser(UserAggregate user)
    {
        // arrange
        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{UsersUrl}/{user.Id}");

        // assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getUserResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetUserResponse>();
        getUserResponseObj.Should().NotBeNull();
        getUserResponseObj!.Id.Should().Be(user.Id);
        getUserResponseObj.Email.Should().Be(user.Email);
        getUserResponseObj.Password.Should().Be(user.Password);
    }

    [Theory]
    [AutoData]
    public async Task GetUserProjects(UserAggregate user)
    {
        //arrange
        var projects = _fixture.Build<Project>().CreateMany(3).ToList();
        user.AddProjects(projects);

        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        //act
        var getResponseMsg = await client.GetAsync($"{UsersUrl}/{user.Id}/Projects");

        //assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getUserProjectsResponseObjs =
            (await getResponseMsg.Content.ReadFromJsonAsync<GetUserProjectsResponse>())?.Projects;
        getUserProjectsResponseObjs.Should().NotBeNull();
        getUserProjectsResponseObjs!.Should().BeEquivalentTo(projects);
    }

    [Theory]
    [AutoData]
    public async Task GetUserTodoItems(UserAggregate user)
    {
        //arrange
        var todoItems = _fixture.Build<TodoItem>().CreateMany(3).ToList();
        user.AddTodoItems(todoItems);

        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        //act
        var getResponseMsg = await client.GetAsync($"{UsersUrl}/{user.Id}/TodoItems");

        //assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getUserProjectsResponseObjs =
            (await getResponseMsg.Content.ReadFromJsonAsync<GetUserTodoItemsResponse>())?.TodoItems;
        getUserProjectsResponseObjs.Should().NotBeNull();
        getUserProjectsResponseObjs!.Should().BeEquivalentTo(todoItems);
    }

    [Theory]
    [AutoData]
    public async Task CreateUser(CreateUserRequest createUserRequestObj)
    {
        // arrange
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(UsersUrl, createUserRequestObj);

        // assert
        postResponseMsg.StatusCode.Should().Be(HttpStatusCode.Created);

        var createUserResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateUserResponse>();
        createUserResponseObj.Should().NotBeNull();
        createUserResponseObj!.Id.Should().NotBeEmpty();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(u => u.Id == createUserResponseObj.Id);

        result.Should().NotBeNull();
        createUserResponseObj.Id.Should().Be(result!.Id);
        _userIds.Push(createUserResponseObj.Id);

        createUserResponseObj.Email.Should().Be(createUserRequestObj.Email);
        createUserResponseObj.Email.Should().Be(result.Email);
        createUserResponseObj.Password.Should().Be(createUserRequestObj.Password);
        createUserResponseObj.Password.Should().Be(result.Password);
        result.TodoItems!.Count.Should().Be(0);
        result.Projects!.Count.Should().Be(0);
    }

    [Theory]
    [AutoData]
    public async Task UpdateUser(UpdateUserRequest updateUserRequestObj, UserAggregate user)
    {
        // arrange
        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        // act
        var putResponseMsg = await client.PutAsJsonAsync($"{UsersUrl}/{user.Id}", updateUserRequestObj);

        // assert
        putResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(updateUserRequestObj.Email);
        result.Password.Should().Be(updateUserRequestObj.Password);
        result.TodoItems!.Count.Should().Be(user.TodoItems!.Count);
        result.Projects!.Count.Should().Be(user.Projects!.Count);
    }

    [Theory]
    [AutoData]
    public async Task DeleteUser(UserAggregate user)
    {
        // arrange
        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{UsersUrl}/{user.Id}");

        // assert
        deleteResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        result.Should().BeNull();
    }
}