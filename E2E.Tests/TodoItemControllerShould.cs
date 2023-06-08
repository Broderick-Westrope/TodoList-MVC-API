using System.Net;
using System.Net.Http.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.MVC.API;
using TodoList.MVC.API.Models;
using TodoList.MVC.API.Requests.TodoItem;
using TodoList.MVC.API.Requests.User;
using TodoList.MVC.API.Responses.TodoItem;
using TodoList.MVC.API.Responses.User;

namespace E2E.Tests;

//TODO: Change to use dbContext for arrange operations
public class TodoItemControllerShould: IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string Url = "api/TodoItem";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Stack<Guid> _userIds = new();
    private readonly TodoContext _todoContext;
    private readonly IServiceScope _scope;

    public TodoItemControllerShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _scope = _factory.Services.CreateScope();
        _todoContext = (TodoContext) _scope.ServiceProvider.GetRequiredService(typeof(TodoContext));
    }

    [Theory]
    [AutoData]
    //? Better to generate the createTodo values so we dont make a redundant userId value?
    public async Task GetTodoItem(CreateUserRequest createUserRequestObj, CreateTodoItemRequest createTodoItemRequestObj)
    {
        // arrange
        _userIds.Push(createTodoItemRequestObj.UserId);
        await _todoContext.AddAsync(new User(createTodoItemRequestObj.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await _todoContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();
        var postResponseMsg =
            await client.PostAsJsonAsync(Url, createTodoItemRequestObj);
        var createTodoItemResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateTodoItemResponse>();

        // act
        var getResponseMsg = await client.GetAsync($"{Url}/{createTodoItemResponseObj.Id}");

        // assert
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var getTodoItemResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetTodoItemResponse>();
        getTodoItemResponseObj
            .Should()
            .NotBeNull();
        getTodoItemResponseObj!
            .Id
            .Should()
            .Be(createTodoItemResponseObj.Id);
        getTodoItemResponseObj
            .Title
            .Should()
            .Be(createTodoItemResponseObj.Title);
        getTodoItemResponseObj
            .Description
            .Should()
            .Be(createTodoItemResponseObj.Description);
        getTodoItemResponseObj
            .DueDate
            .Should()
            .Be(createTodoItemResponseObj.DueDate);
        getTodoItemResponseObj
            .IsCompleted
            .Should()
            .Be(createTodoItemResponseObj.IsCompleted);
        getTodoItemResponseObj
            .UserId
            .Should()
            .Be(createTodoItemResponseObj.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task GetAllTodoItems(CreateUserRequest createUserRequestObj, CreateTodoItemRequest createTodoItemRequestObj)
    {
        // arrange
        _userIds.Push(createTodoItemRequestObj.UserId);
        await _todoContext.AddAsync(new User(createTodoItemRequestObj.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await _todoContext.SaveChangesAsync();

        var client = _factory.CreateClient();
        var postResponseMsg = await client.PostAsJsonAsync(Url, createTodoItemRequestObj);
        var createTodoItemResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateTodoItemResponse>();

        // act
        var getResponseMsg = await client.GetAsync($"{Url}");

        // assert
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        
        var getTodoItemResponseObjs = (await getResponseMsg.Content.ReadFromJsonAsync<GetAllTodoItemsResponse>())?.TodoItems;
        getTodoItemResponseObjs
            .Should()
            .NotBeNull();
        getTodoItemResponseObjs!
            .Count
            .Should()
            .BePositive();
        getTodoItemResponseObjs
            .Should()
            .Contain(x =>
                x.Id == createTodoItemResponseObj.Id && 
                x.Title == createTodoItemResponseObj.Title &&
                x.Description == createTodoItemResponseObj.Description && 
                x.IsCompleted == createTodoItemResponseObj.IsCompleted && 
                x.DueDate == createTodoItemResponseObj.DueDate &&
                x.UserId == createTodoItemResponseObj.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task PostTodoItem(CreateUserRequest createUserRequestObj, CreateTodoItemRequest createTodoItemRequestObj)
    {
        // arrange
        _userIds.Push(createTodoItemRequestObj.UserId);
        await _todoContext.AddAsync(new User(createTodoItemRequestObj.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await _todoContext.SaveChangesAsync();
        
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(Url, createTodoItemRequestObj);

        // assert
        postResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.Created);
        
        var createTodoItemResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateTodoItemResponse>();
        createTodoItemResponseObj
            .Should()
            .NotBeNull();
        createTodoItemResponseObj
            .Id
            .Should()
            .NotBeEmpty();
        createTodoItemResponseObj
            .Title
            .Should()
            .Be(createTodoItemResponseObj.Title);
        createTodoItemResponseObj
            .Description
            .Should()
            .Be(createTodoItemRequestObj.Description);
        createTodoItemResponseObj
            .IsCompleted
            .Should()
            .Be(false);
        createTodoItemResponseObj
            .DueDate
            .Should()
            .Be(createTodoItemRequestObj.DueDate);
        createTodoItemResponseObj
            .UserId
            .Should()
            .Be(createTodoItemRequestObj.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task PutTodoItem(CreateUserRequest createUserRequestObj, CreateTodoItemRequest createTodoItemRequestObj, UpdateTodoItemRequest updateTodoItemRequestObj)
    {
        // arrange
        _userIds.Push(createTodoItemRequestObj.UserId);
        await _todoContext.AddAsync(new User(createTodoItemRequestObj.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await _todoContext.SaveChangesAsync();

        var client = _factory.CreateClient();
        var postResponseMsg = await client.PostAsJsonAsync(Url, createTodoItemRequestObj);
        var createTodoItemResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateTodoItemResponse>();

        // act
        //? Possible to restrict updateTodoItemRequestObj.UserId to being the same as createTodoItemRequestObj.UserId?
        var putResponseMsg = await client.PutAsJsonAsync($"{Url}/{createTodoItemResponseObj!.Id}", updateTodoItemRequestObj with { UserId = createTodoItemRequestObj.UserId});

        // assert
        putResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var getResponseMsg = await client.GetAsync($"{Url}/{createTodoItemResponseObj.Id}");
        var getTodoItemResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetTodoItemResponse>();
        getTodoItemResponseObj
            .Should()
            .NotBeNull();
        getTodoItemResponseObj!
            .Id
            .Should()
            .Be(createTodoItemResponseObj.Id);
        getTodoItemResponseObj
            .Title
            .Should()
            .Be(updateTodoItemRequestObj.Title);
        getTodoItemResponseObj
            .Description
            .Should()
            .Be(updateTodoItemRequestObj.Description);
        getTodoItemResponseObj
            .IsCompleted
            .Should()
            .Be(updateTodoItemRequestObj.IsCompleted);
        getTodoItemResponseObj
            .DueDate
            .Should()
            .Be(updateTodoItemRequestObj.DueDate);
        getTodoItemResponseObj
            .UserId
            .Should()
            .Be(createTodoItemRequestObj.UserId);
    }

    [Theory]
    [AutoData]
    public async Task DeleteTodoItem(CreateUserRequest createUserRequestObj, CreateTodoItemRequest createTodoItemRequestObj)
    {
        // arrange
        //TODO: move to a shared AddUser().
        _userIds.Push(createTodoItemRequestObj.UserId);
        await _todoContext.AddAsync(new User(createTodoItemRequestObj.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await _todoContext.SaveChangesAsync();

        var client = _factory.CreateClient();
        var postResponseMsg =  await client.PostAsJsonAsync(Url, createTodoItemRequestObj);
        var createTodoItemResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateTodoItemResponse>();
        _userIds.Push(createTodoItemResponseObj!.Id);

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{Url}/{createTodoItemResponseObj.Id}");

        // assert
        deleteResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
        
        var getResponseMsg = await client.GetAsync($"{Url}/{createTodoItemResponseObj.Id}");
        getResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }
    
    public async void Dispose()
    {
        while (_userIds.Count > 0)
        {
            var user = await _todoContext.Users.FirstAsync(x=> x!.Id == _userIds.Pop())!;
            _todoContext.Users.Remove(user);
            await _todoContext.SaveChangesAsync();
        }
        _scope.Dispose();
    }
}