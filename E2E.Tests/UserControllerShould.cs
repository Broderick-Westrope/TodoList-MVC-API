using System.Net;
using System.Net.Http.Json;
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
    private readonly User _userDetails = new(Guid.Empty, "Heidi_Cormier91@hotmail.com", "_k7qNLNjR0szIh1");


    public UserControllerShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async void Dispose()
    {
        await _factory.CreateClient().DeleteAsync($"{Url}/{_userDetails.Id}");
    }

    [Fact]
    public async Task GetUser()
    {
        // arrange
        var client = _factory.CreateClient();
        var createdUser = await client.PostAsJsonAsync(Url,
            new CreateUserRequest(_userDetails.Email, _userDetails.Password));
        var createUserResponse = await createdUser.Content.ReadFromJsonAsync<CreateUserResponse>();
        _userDetails.Id = createUserResponse!.Id;

        // act
        var result = await client.GetAsync($"{Url}/{createUserResponse.Id}");

        // assert
        result
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        var getUserResponse = await result.Content.ReadFromJsonAsync<GetUserResponse>();
        getUserResponse
            .Should()
            .NotBeNull();
        getUserResponse!
            .Id
            .Should()
            .Be(createUserResponse.Id);
        getUserResponse
            .Email
            .Should()
            .Be(createUserResponse.Email);
        getUserResponse
            .Password
            .Should()
            .Be(createUserResponse.Password);
    }

    [Fact]
    public async Task GetAllUsers()
    {
        // arrange
        var client = _factory.CreateClient();
        var createdUser = await client.PostAsJsonAsync(Url,
            new CreateUserRequest(_userDetails.Email, _userDetails.Password));
        var createUserResponse = await createdUser.Content.ReadFromJsonAsync<CreateUserResponse>();
        _userDetails.Id = createUserResponse.Id;

        // act
        var result = await client.GetAsync($"{Url}");

        // assert
        result
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        var getUserResponse = (await result.Content.ReadFromJsonAsync<GetAllUsersResponse>())?.Users;
        getUserResponse
            .Should()
            .NotBeNull();
        getUserResponse!
            .Count
            .Should()
            .BePositive();
        getUserResponse
            .Should()
            .Contain(x =>
                x.Id == createUserResponse!.Id && x.Email == createUserResponse.Email &&
                x.Password == createUserResponse.Password);
    }

    [Fact]
    public async Task PostUser()
    {
        // arrange
        var client = _factory.CreateClient();
        var request = new CreateUserRequest(_userDetails.Email, _userDetails.Password);

        // act
        var result = await client.PostAsJsonAsync(Url, request);

        // assert
        result
            .StatusCode
            .Should()
            .Be(HttpStatusCode.Created);
        var response = await result.Content.ReadFromJsonAsync<CreateUserResponse>();
        response
            .Should()
            .NotBeNull();
        _userDetails.Id = response!.Id;
        response
            .Id
            .Should()
            .NotBeEmpty();
        response
            .Email
            .Should()
            .Be(request.Email);
        response
            .Password
            .Should()
            .Be(request.Password);
    }

    [Fact]
    public async Task PutUser()
    {
        // arrange
        var client = _factory.CreateClient();
        var createdUser = await client.PostAsJsonAsync(Url,
            new CreateUserRequest(_userDetails.Email, _userDetails.Password));
        var createUserResponse = await createdUser.Content.ReadFromJsonAsync<CreateUserResponse>();
        _userDetails.Id = createUserResponse!.Id;
        var updatedUser = new UpdateUserRequest("Albert.Marquardt@gmail.com", "21_2I8CDiGRmrCg");

        // act
        var result = await client.PutAsJsonAsync($"{Url}/{createUserResponse.Id}", updatedUser);

        // assert
        result
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
        var getUserResponse = await (await client.GetAsync($"{Url}/{createUserResponse.Id}")).Content
            .ReadFromJsonAsync<GetUserResponse>();
        getUserResponse
            .Should()
            .NotBeNull();
        getUserResponse!
            .Email
            .Should()
            .Be(updatedUser.Email);
        getUserResponse
            .Password
            .Should()
            .Be(updatedUser.Password);
    }

    [Fact]
    public async Task DeleteUser()
    {
        // arrange
        var client = _factory.CreateClient();
        var request = new CreateUserRequest(_userDetails.Email, _userDetails.Password);
        var response =
            await (await client.PostAsJsonAsync(Url, request)).Content.ReadFromJsonAsync<CreateUserResponse>();

        // act
        await client.DeleteAsync($"{Url}/{response!.Id}");

        // assert
        var result = await client.GetAsync($"{Url}/{response.Id}");
        result
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }
}