using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using TodoList.MVC.API.DbModels;

namespace UnitTests;

public class UserAggregateShould
{
    private readonly Fixture _fixture = new();

    [Theory]
    [AutoData]
    public void AddTodoItem(UserAggregate user, TodoItem todoItem)
    {
        //act
        user.AddTodoItem(todoItem);
        
        //assert
        user.TodoItems.Should().Contain(todoItem);
    }

    [Theory]
    [AutoData]
    public void DeleteTodoItem(UserAggregate user, TodoItem todoItem)
    {
        //arrange
        user.AddTodoItem(todoItem);
        Assert.Contains(todoItem, user.TodoItems);
        
        // act
        user.DeleteTodoItem(todoItem.Id);
        
        //assert
        user.TodoItems.Should().NotContain(todoItem);
    }
    
    [Theory]
    [AutoData]
    public void AddProject(UserAggregate user, Project project)
    {
        //act
        user.AddProject(project);
        
        //assert
        user.Projects.Should().Contain(project);
    }

    [Theory]
    [AutoData]
    public void DeleteProject(UserAggregate user, Project project)
    {
        //arrange
        user.AddProject(project);
        Assert.Contains(project, user.Projects);
        
        // act
        user.DeleteProject(project.Id);
        
        //assert
        user.Projects.Should().NotContain(project);
    }
    
    
}