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
    private const string TodoItemsUrl = "api/TodoItems";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Fixture _fixture = new();
    private readonly Stack<Guid> _userIds = new();

    public TodoItemControllerShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        while (_userIds.Count > 0)
        {
            var userId = _userIds.Pop();
            var user = context.Users.First(x => x!.Id == userId);
            context.Users.Remove(user);
            context.SaveChanges();
        }
    }

    private async Task AddUserToDb(UserAggregate user)
    {
        using var scope = _factory.Services.CreateScope();
        //? Should I be doing "await using ..." since DbContext is IDisposable?
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        _userIds.Push(user.Id);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    [Theory]
    [AutoData]
    //? Better to generate the createTodo values so we dont make a redundant userId value?
    public async Task GetTodoItem(TodoItem todoItem, UserAggregate user)
    {
        // arrange
        user.AddTodoItem(todoItem);
        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var getResponseMsg = await client.GetAsync($"{TodoItemsUrl}/{todoItem.Id}");

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
    public async Task CreateTodoItem(UserAggregate user)
    {
        // arrange
        var createTodoItemRequestObj = _fixture.Build<CreateTodoItemRequest>()
            .With(x => x.UserId, user.Id).Create();

        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var postResponseMsg = await client.PostAsJsonAsync(TodoItemsUrl, createTodoItemRequestObj);

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
    public async Task UpdateTodoItem(UpdateTodoItemRequest updateTodoItemRequestObj, TodoItem todoItem,
        UserAggregate user)
    {
        // arrange
        user.AddTodoItem(todoItem);
        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var putResponseMsg = await client.PutAsJsonAsync($"{TodoItemsUrl}/{todoItem.Id}", updateTodoItemRequestObj);

        // assert
        putResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result =
            (await context.Users.FirstAsync(x => x.TodoItems.FirstOrDefault(y => y.Id == todoItem.Id) != null))
            .TodoItems.First(x => x.Id == todoItem.Id);

        result.Should().BeEquivalentTo(updateTodoItemRequestObj);
    }

    [Theory]
    [AutoData]
    public async Task DeleteTodoItem(TodoItem todoItem, UserAggregate user)
    {
        // arrange
        user.AddTodoItem(todoItem);
        await AddUserToDb(user);
        var client = _factory.CreateClient();

        // act
        var deleteResponseMsg = await client.DeleteAsync($"{TodoItemsUrl}/{todoItem.Id}");

        // assert
        deleteResponseMsg.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        var result = await context.Users.FirstOrDefaultAsync(x =>
            x.TodoItems != null && x.TodoItems.FirstOrDefault(y => y.Id == todoItem.Id) != null);
        ;
        result.Should().BeNull();
    }
}