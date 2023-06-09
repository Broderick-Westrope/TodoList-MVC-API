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
using TodoList.MVC.API.Responses.User;

namespace E2E.Tests;

public class UserControllerShould : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string UserUrl = "api/User";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Stack<Guid> _userIds = new();
    private readonly Fixture _fixture = new();

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
            var user = await context.Users.FirstAsync(x => x!.Id == userId);
            context.Users.Remove(user);
        }
        //? Is it bad practice to save the changes outside of the loop?
        await context.SaveChangesAsync();
    }

    private async Task AddUsersToDb(IEnumerable<User> users)
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

    [Fact]
    public async Task GetUser()
    {
        // arrange
        //TODO: Remove User reference from items & projects, then remove these Without().
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects).Create();

        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{UserUrl}/{user.Id}");

        // assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getUserResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetUserResponse>();
        getUserResponseObj.Should().NotBeNull();
        getUserResponseObj!.Id.Should().Be(user.Id);
        getUserResponseObj.Email.Should().Be(user.Email);
        getUserResponseObj.Password.Should().Be(user.Password);
    }

    [Fact]
    public async Task GetAllUsers()
    {
        // arrange
        //TODO: Remove User reference from items & projects, then remove these Without().
        var users = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects).CreateMany(3).ToList();

        await AddUsersToDb(users);
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{UserUrl}");

        // assert
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var getUserResponseObjs = (await getResponseMsg.Content.ReadFromJsonAsync<GetAllUsersResponse>())?.Users;
        getUserResponseObjs.Should().NotBeNull();
        getUserResponseObjs!.Count.Should().BeGreaterOrEqualTo(users.Count);
        foreach (var user in users)
            getUserResponseObjs.Should().Contain(x =>
                x.Id == user.Id &&
                x.Email == user.Email &&
                x.Password == user.Password);
    }

    [Theory]
    [AutoData]
    public async Task PostUser(CreateUserRequest createUserRequestObj)
    {
        // arrange
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(UserUrl, createUserRequestObj);

        // assert
        postResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var createUserResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateUserResponse>();
        createUserResponseObj
            .Should()
            .NotBeNull();
        createUserResponseObj!
            .Id
            .Should()
            .NotBeEmpty();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(u => u!.Id == createUserResponseObj.Id);

        result.Should().NotBeNull();
        createUserResponseObj.Id.Should().Be(result!.Id);
        _userIds.Push(createUserResponseObj.Id);

        createUserResponseObj.Email.Should().Be(createUserRequestObj.Email);
        createUserResponseObj.Email.Should().Be(result.Email);
        createUserResponseObj.Password.Should().Be(createUserRequestObj.Password);
        createUserResponseObj.Password.Should().Be(result.Password);
        result.TodoItems.Count.Should().Be(0);
        result.Projects.Count.Should().Be(0);
    }

    [Theory]
    [AutoData]
    public async Task PutUser(UpdateUserRequest updateUserRequestObj)
    {
        // arrange
        //TODO: Remove User reference from items & projects, then remove these Without().
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects).Create();

        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        // act
        var putResponseMsg = await client.PutAsJsonAsync($"{UserUrl}/{user.Id}", updateUserRequestObj);

        // assert
        putResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(u => u!.Id == user.Id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(updateUserRequestObj.Email);
        result.Password.Should().Be(updateUserRequestObj.Password);
        result.TodoItems.Count.Should().Be(0);
        result.Projects.Count.Should().Be(0);
    }

    [Fact]
    public async Task DeleteUser()
    {
        // arrange
        var user = _fixture.Build<User>()
            .Without(x => x.TodoItems)
            .Without(x => x.Projects).Create();

        await AddUsersToDb(new[] { user });
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{UserUrl}/{user.Id}");

        // assert
        deleteResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(x => x!.Id == user.Id);
        result
            .Should()
            .BeNull();
    }
}