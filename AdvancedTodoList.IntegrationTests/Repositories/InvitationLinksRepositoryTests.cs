using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.IntegrationTests.Utils;

namespace AdvancedTodoList.IntegrationTests.Repositories;

public class InvitationLinksRepositoryTests : BaseRepositoryTests<InvitationLink, int>
{
    private IInvitationLinksRepository LinksRepository => (IInvitationLinksRepository)Repository;

    protected override int NonExistingId => -1;

    private readonly DateTime UpdatedValidTo = DateTime.UtcNow.AddDays(10);

    protected override void AssertUpdated(InvitationLink updatedEntity)
    {
        Assert.That(updatedEntity.ValidTo, Is.EqualTo(UpdatedValidTo));
    }

    protected override async Task<InvitationLink> CreateTestEntityAsync()
    {
        TodoList todoList = TestModels.CreateTestTodoList();
        DbContext.Add(todoList);
        await DbContext.SaveChangesAsync();

        return TestModels.CreateTestInvitationLink(todoList.Id);
    }

    protected override void UpdateEntity(InvitationLink entity)
    {
        entity.ValidTo = UpdatedValidTo;
    }

    [Test]
    public async Task FindAsync_EntityExists_ReturnsValidLink()
    {
        // Arrange
        var link = await AddTestEntityToDbAsync();

        // Act
        var result = await LinksRepository.FindAsync(link.Value);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(link.Id));
    }

    [Test]
    public async Task FindAsync_EntityDoesNotExist_ReturnsNull()
    {
        // Arrange
        string link = "bad link";

        // Act
        var result = await LinksRepository.FindAsync(link);

        // Assert
        Assert.That(result, Is.Null);
    }
}
