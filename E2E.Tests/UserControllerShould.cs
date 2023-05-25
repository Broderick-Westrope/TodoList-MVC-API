using System.Net;
using System.Net.Http.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests;
using TodoList.MVC.API.Requests.User;
using TodoList.MVC.API.Responses.User;

namespace E2E.Tests;

public class UserControllerShould : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string Url = "api/User";
    
    private readonly WebApplicationFactory<Program> _factory;
    private Guid _userId = Guid.Empty;

        
    public UserControllerShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async void Dispose()
    {
        await _factory.CreateClient().DeleteAsync($"{Url}/{_userId}");
    }


    [Theory]
    [AutoData]
    public async Task GetUser(CreateUserRequest createUserRequestObj)
    {
        // arrange
        var client = _factory.CreateClient();
        var postResponseMsg = await client.PostAsJsonAsync(Url, createUserRequestObj);
        var createUserResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateUserResponse>();
        _userId = createUserResponseObj!.Id;

        // act
        var getResponseMsg = await client.GetAsync($"{Url}/{createUserResponseObj.Id}");

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
            .Be(createUserResponseObj.Id);
        getUserResponseObj
            .Email
            .Should()
            .Be(createUserResponseObj.Email);
        getUserResponseObj
            .Password
            .Should()
            .Be(createUserResponseObj.Password);
    }

    [Theory]
    [AutoData]
    public async Task GetAllUsers(CreateUserRequest createUserRequestObj)
    {
        // arrange
        var client = _factory.CreateClient();
        var postResponseMsg = await client.PostAsJsonAsync(Url, createUserRequestObj);
        var createUserResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateUserResponse>();
        _userId = createUserResponseObj!.Id;

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
                x.Id == createUserResponseObj.Id && x.Email == createUserResponseObj.Email &&
                x.Password == createUserResponseObj.Password);
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
        _userId = createUserResponseObj!.Id;
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
    public async Task PutUser(CreateUserRequest createUserRequestObj, UpdateUserRequest updateUserRequestObj)
    {
        // arrange
        var client = _factory.CreateClient();
        var postResponseMsg = await client.PostAsJsonAsync(Url, createUserRequestObj);
        var createUserResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateUserResponse>();
        _userId = createUserResponseObj!.Id;

        // act
        var putResponseMsg = await client.PutAsJsonAsync($"{Url}/{createUserResponseObj.Id}", updateUserRequestObj);

        // assert
        putResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var getResponseMsg = await client.GetAsync($"{Url}/{createUserResponseObj.Id}");
        var getUserResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetUserResponse>();
        getUserResponseObj
            .Should()
            .NotBeNull();
        getUserResponseObj!
            .Email
            .Should()
            .Be(updateUserRequestObj.Email);
        getUserResponseObj
            .Password
            .Should()
            .Be(updateUserRequestObj.Password);
    }

    [Theory]
    [AutoData]
    public async Task DeleteUser(CreateUserRequest createUserRequestObj)
    {
        // arrange
        var client = _factory.CreateClient();
        var postResponseMsg =  await client.PostAsJsonAsync(Url, createUserRequestObj);
        var createUserResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateUserResponse>();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{Url}/{createUserResponseObj!.Id}");

        // assert
        deleteResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
        
        var getResponseMsg = await client.GetAsync($"{Url}/{createUserResponseObj.Id}");
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }
}