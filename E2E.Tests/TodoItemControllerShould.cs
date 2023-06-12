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
using TodoList.MVC.API.Requests.TodoItem;
using TodoList.MVC.API.Responses.TodoItem;

namespace E2E.Tests;

//TODO: Add cancellation tokens to tests
public class TodoItemControllerShould : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private const string TodoItemUrl = "api/TodoItem";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Fixture _fixture = new();
    private readonly Stack<Guid> _userIds = new();

    public TodoItemControllerShould(WebApplicationFactory<Program> factory)
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

    private async Task AddUserToDb(UserAggregateRoot userAggregateRoot)
    {
        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        _userIds.Push(userAggregateRoot.Id);
        await context.Users.AddAsync(userAggregateRoot);
        await context.SaveChangesAsync();
    }

    private async Task AddTodoItemsToDb(IEnumerable<TodoItem> todoItems)
    {
        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        foreach (var todoItem in todoItems) await context.TodoItems.AddAsync(todoItem);

        //? Is it bad practice to save the changes outside of the loop?
        await context.SaveChangesAsync();
    }

    [Theory]
    [AutoData]
    //? Better to generate the createTodo values so we dont make a redundant userId value?
    public async Task GetTodoItem(TodoItem todoItem, UserAggregateRoot user)
    {
        // arrange
        await AddUserToDb(user);
        await AddTodoItemsToDb(new[] { todoItem });
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{TodoItemUrl}/{todoItem.Id}");

        // assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getTodoItemResponseObj = await getResponseMsg.Content.ReadFromJsonAsync<GetTodoItemResponse>();
        getTodoItemResponseObj.Should().NotBeNull();
        getTodoItemResponseObj!.Id.Should().Be(todoItem.Id);
        getTodoItemResponseObj.Title.Should().Be(todoItem.Title);
        getTodoItemResponseObj.Description.Should().Be(todoItem.Description);
        getTodoItemResponseObj.DueDate.Should().Be(todoItem.DueDate);
        getTodoItemResponseObj.IsCompleted.Should().Be(todoItem.IsCompleted);
    }

    [Theory]
    [AutoData]
    public async Task GetAllTodoItems(TodoItem todoItem, UserAggregateRoot user)
    {
        // arrange
        await AddUserToDb(user);
        await AddTodoItemsToDb(new[] { todoItem });
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{TodoItemUrl}");

        // assert
        getResponseMsg.StatusCode.Should().Be(HttpStatusCode.OK);

        var getTodoItemResponseObjs =
            (await getResponseMsg.Content.ReadFromJsonAsync<GetAllTodoItemsResponse>())?.TodoItems;
        getTodoItemResponseObjs.Should().NotBeNull();
        getTodoItemResponseObjs!.Count.Should().BePositive();
        getTodoItemResponseObjs.Should().Contain(x =>
                x.Id == todoItem.Id &&
                x.Title == todoItem.Title &&
                x.Description == todoItem.Description &&
                x.IsCompleted == todoItem.IsCompleted &&
                x.DueDate == todoItem.DueDate);
    }

    [Theory]
    [AutoData]
    public async Task PostTodoItem(CreateTodoItemRequest createTodoItemRequestObj, UserAggregateRoot user)
    {
        // arrange
        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(TodoItemUrl, createTodoItemRequestObj);

        // assert
        postResponseMsg.StatusCode.Should().Be(HttpStatusCode.Created);

        var createTodoItemResponseObj = await postResponseMsg.Content.ReadFromJsonAsync<CreateTodoItemResponse>();
        createTodoItemResponseObj.Should().NotBeNull();
        createTodoItemResponseObj!.Id.Should().NotBeEmpty();
        createTodoItemResponseObj.Title.Should().Be(createTodoItemResponseObj.Title);
        createTodoItemResponseObj.Description.Should().Be(createTodoItemRequestObj.Description);
        createTodoItemResponseObj.IsCompleted.Should().Be(false);
        createTodoItemResponseObj.DueDate.Should().Be(createTodoItemRequestObj.DueDate);
    }

    [Theory]
    [AutoData]
    public async Task PutTodoItem(UpdateTodoItemRequest updateTodoItemRequestObj, TodoItem todoItem, UserAggregateRoot user)
    {
        // arrange
        await AddUserToDb(user);
        await AddTodoItemsToDb(new[] { todoItem });
        var client = _factory.CreateClient();

        // act
        var putResponseMsg = await client.PutAsJsonAsync($"{TodoItemUrl}/{todoItem.Id}", updateTodoItemRequestObj);

        // assert
        putResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.TodoItems.FirstOrDefaultAsync(t => t.Id == todoItem.Id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(todoItem.Id);
        result.Title.Should().Be(updateTodoItemRequestObj.Title);
        result.Description.Should().Be(updateTodoItemRequestObj.Description);
        result.IsCompleted.Should().Be(updateTodoItemRequestObj.IsCompleted);
        result.DueDate.Should().Be(updateTodoItemRequestObj.DueDate);
    }

    [Theory]
    [AutoData]
    public async Task DeleteTodoItem(TodoItem todoItem, UserAggregateRoot user)
    {
        // arrange
        await AddUserToDb(user);
        await AddTodoItemsToDb(new[] { todoItem });
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{TodoItemUrl}/{todoItem.Id}");

        // assert
        deleteResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.TodoItems.FirstOrDefaultAsync(x => x.Id == todoItem.Id);
        result.Should().BeNull();
    }
}