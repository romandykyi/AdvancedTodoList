using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.IntegrationTests.Services;

namespace AdvancedTodoList.IntegrationTests.Utils;

/// <summary>
/// A class that contains test models and IDs of entities which can be used
/// as dependencies.
/// </summary>
[SetUpFixture]
public static class TestModels
{
    /// <summary>
    /// Creates and returns a valid model of a to-do list which can be added to the DB. 
    /// </summary>
    public static TodoList CreateTestTodoList(string? ownerId = null) => new()
    {
        Name = "Name",
        Description = "Description",
        OwnerId = ownerId
    };
    /// <summary>
    /// Creates and returns a valid model of a to-do list item which can be added to the DB. 
    /// </summary>
    public static TodoItem CreateTestTodoItem(string todoListId, string? ownerId = null, int? categoryId = null) => new()
    {
        Name = "Name",
        Description = "Description",
        OwnerId = ownerId,
        CategoryId = categoryId,
        Priority = 5,
        State = TodoItemState.Completed,
        DeadlineDate = DateTime.UtcNow.AddDays(365),
        TodoListId = todoListId
    };
    /// <summary>
    /// Creates and returns a valid model of a to-do list item category which can be added to the DB. 
    /// </summary>
    public static TodoItemCategory CreateTestTodoItemCategory(string todoListId) => new()
    {
        Name = "Category",
        TodoListId = todoListId
    };
    /// <summary>
    /// Creates and returns a valid model of a test to-do list dependant entity. 
    /// </summary>
    public static TestTodoListDependantEntity CreateTestTodoListDependantEntity(string todoListId) => new()
    {
        TestProperty = "Test",
        TodoListId = todoListId
    };
    /// <summary>
    /// Creates and returns a valid model of an application user.
    /// </summary>
    public static ApplicationUser CreateTestUser()
    {
        string userName = Guid.NewGuid().ToString();
        return new()
        {
            UserName = userName,
            FirstName = "Test",
            LastName = "User",
            Email = $"{userName}@example.com"
        };
    }
    /// <summary>
    /// Creates and returns a valid model of a refresh token.
    /// </summary>
    public static UserRefreshToken CreateTestUserRefreshToken(string userId) => new()
    {
        UserId = userId,
        Token = "TestToken",
        ValidTo = DateTime.UtcNow.AddDays(180)
    };
    /// <summary>
    /// Creates and returns a valid model of a todo-list role.
    /// </summary>
    public static TodoListRole CreateTestRole(string todoListId) => new()
    {
        Name = "Role1",
        Priority = 5,
        TodoListId = todoListId
    };

    /// <summary>
    /// Creates and returns a valid model of an invitation link.
    /// </summary>
    public static InvitationLink CreateTestInvitationLink(string todoListId, DateTime? validTo = null) => new()
    {
        Value = Guid.NewGuid().ToString(),
        TodoListId = todoListId,
        ValidTo = validTo ?? DateTime.Now.AddDays(5)
    };
}
