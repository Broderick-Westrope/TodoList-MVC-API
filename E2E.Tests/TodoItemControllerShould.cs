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

    private async Task AddUser(User user)
    {
        _userIds.Push(user.Id);
        await _todoContext.Users.AddAsync(user);
        await _todoContext.SaveChangesAsync();
    }

    private async Task AddTodoItem(TodoItem todoItem)
    {
        await _todoContext.TodoItems.AddAsync(todoItem);
        await _todoContext.SaveChangesAsync();
    }

    [Theory]
    [AutoData]
    //? Better to generate the createTodo values so we dont make a redundant userId value?
    //TODO: Continue from here: lazy loading with proxies vs using a DTO for the todoItem values in this test?
    public async Task GetTodoItem(CreateUserRequest createUserRequestObj, TodoItem todoItem)
    {
        // arrange
        await AddUser(new User(todoItem.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddTodoItem(todoItem);
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{Url}/{todoItem.Id}");

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
            .Be(todoItem.Id);
        getTodoItemResponseObj
            .Title
            .Should()
            .Be(todoItem.Title);
        getTodoItemResponseObj
            .Description
            .Should()
            .Be(todoItem.Description);
        getTodoItemResponseObj
            .DueDate
            .Should()
            .Be(todoItem.DueDate);
        getTodoItemResponseObj
            .IsCompleted
            .Should()
            .Be(todoItem.IsCompleted);
        getTodoItemResponseObj
            .UserId
            .Should()
            .Be(todoItem.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task GetAllTodoItems(CreateUserRequest createUserRequestObj, TodoItem todoItem)
    {
        // arrange
        await AddUser(new User(todoItem.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddTodoItem(todoItem);
        var client = _factory.CreateClient();

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
                x.Id == todoItem.Id && 
                x.Title == todoItem.Title &&
                x.Description == todoItem.Description && 
                x.IsCompleted == todoItem.IsCompleted && 
                x.DueDate == todoItem.DueDate &&
                x.UserId == todoItem.UserId);
    }
    
    [Theory]
    [AutoData]
    public async Task PostTodoItem(CreateUserRequest createUserRequestObj, CreateTodoItemRequest createTodoItemRequestObj)
    {
        // arrange
        await AddUser(new User(createTodoItemRequestObj.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        
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
        createTodoItemResponseObj!
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
    public async Task PutTodoItem(CreateUserRequest createUserRequestObj, TodoItem todoItem, UpdateTodoItemRequest updateTodoItemRequestObj)
    {
        // arrange
        await AddUser(new User(todoItem.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddTodoItem(todoItem);
        var client = _factory.CreateClient();

        // act
        //? Possible to restrict updateTodoItemRequestObj.UserId to being the same as createTodoItemRequestObj.UserId?
        var putResponseMsg = await client.PutAsJsonAsync($"{Url}/{todoItem.Id}", updateTodoItemRequestObj with { UserId = todoItem.UserId});

        // assert
        putResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var getResponseMsg = await client.GetAsync($"{Url}/{todoItem.Id}");
        var getTodoItemResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetTodoItemResponse>();
        getTodoItemResponseObj
            .Should()
            .NotBeNull();
        getTodoItemResponseObj!
            .Id
            .Should()
            .Be(todoItem.Id);
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
            .Be(todoItem.UserId);
    }

    [Theory]
    [AutoData]
    public async Task DeleteTodoItem(CreateUserRequest createUserRequestObj, TodoItem todoItem)
    {
        // arrange
        await AddUser(new User(todoItem.UserId, createUserRequestObj.Email, createUserRequestObj.Password));
        await AddTodoItem(todoItem);
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{Url}/{todoItem.Id}");

        // assert
        deleteResponseMsg
            .StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
        
        var getResponseMsg = await client.GetAsync($"{Url}/{todoItem.Id}");
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