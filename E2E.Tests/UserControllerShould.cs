using System.Net;
using System.Net.Http.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.MVC.API;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests;
using TodoList.MVC.API.Requests.User;
using TodoList.MVC.API.Responses.User;

namespace E2E.Tests;

//TODO: Change CreateUserRequest & Guid pairs to use a User option instead (UserAggregate, Fixture, etc?).
public class UserControllerShould : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string Url = "api/User";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Stack<Guid> _userIds = new();
    private readonly TodoContext _todoContext;
    private readonly IServiceScope _scope;
    
    public UserControllerShould(WebApplicationFactory<Program> factory)
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

    [Theory]
    [AutoData]
    public async Task GetUser(CreateUserRequest createUserRequestObj, Guid userId)
    {
        // arrange
        await AddUser(new User(userId, createUserRequestObj.Email, createUserRequestObj.Password));
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{Url}/{userId}");

        // assert
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var getUserResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetUserResponse>();
        getUserResponseObj
            .Should()
            .NotBeNull();
        getUserResponseObj!
            .Id
            .Should()
            .Be(userId);
        getUserResponseObj
            .Email
            .Should()
            .Be(createUserRequestObj.Email);
        getUserResponseObj
            .Password
            .Should()
            .Be(createUserRequestObj.Password);
    }

    [Theory]
    [AutoData]
    public async Task GetAllUsers(CreateUserRequest createUserRequestObj, Guid userId)
    {
        // arrange
        await AddUser(new User(userId, createUserRequestObj.Email, createUserRequestObj.Password));
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{Url}");

        // assert
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var getUserResponseObjs = (await getResponseMsg.Content.ReadFromJsonAsync<GetAllUsersResponse>())?.Users;
        getUserResponseObjs
            .Should()
            .NotBeNull();
        getUserResponseObjs!
            .Count
            .Should()
            .BePositive();
        getUserResponseObjs
            .Should()
            .Contain(x =>
                x.Id == userId && 
                x.Email == createUserRequestObj.Email && 
                x.Password == createUserRequestObj.Password);
    }

    [Theory]
    [AutoData]
    public async Task PostUser(CreateUserRequest createUserRequestObj)
    {
        // arrange
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(Url, createUserRequestObj);

        // assert
        postResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var createUserResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateUserResponse>();
        createUserResponseObj
            .Should()
            .NotBeNull();
        _userIds.Push(createUserResponseObj!.Id);
        createUserResponseObj
            .Id
            .Should()
            .NotBeEmpty();
        createUserResponseObj
            .Email
            .Should()
            .Be(createUserRequestObj.Email);
        createUserResponseObj
            .Password
            .Should()
            .Be(createUserRequestObj.Password);
    }

    [Theory]
    [AutoData]
    public async Task PutUser(CreateUserRequest createUserRequestObj, Guid userId, UpdateUserRequest updateUserRequestObj)
    {
        // arrange
        await AddUser(new User(userId, createUserRequestObj.Email, createUserRequestObj.Password));
        var client = _factory.CreateClient();

        // act
        var putResponseMsg = await client.PutAsJsonAsync($"{Url}/{userId}", updateUserRequestObj);

        // assert
        putResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
        
        var result = await _todoContext.Users.FirstOrDefaultAsync(u => u!.Id == userId);
        result
            .Should()
            .NotBeNull();
        result!
            .Id
            .Should()
            .Be(userId);
        result
            .Email
            .Should()
            .Be(updateUserRequestObj.Email);
        result
            .Password
            .Should()
            .Be(updateUserRequestObj.Password);
        //TODO: Add check for todo items and projects once user fixture is made.
    }

    [Theory]
    [AutoData]
    public async Task DeleteUser(CreateUserRequest createUserRequestObj, Guid userId)
    {
        // arrange
        await AddUser(new User(userId, createUserRequestObj.Email, createUserRequestObj.Password));
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{Url}/{userId}");

        // assert
        deleteResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
        
        var result = await _todoContext.Users.FirstOrDefaultAsync(x=>x.Id == userId);
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