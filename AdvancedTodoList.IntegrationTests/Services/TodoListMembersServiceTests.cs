using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListMembersServiceTests : BusinessLogicFixture
{
	private ITodoListMembersService _service;

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoListMembersService>()!;
	}

	[Test]
	public async Task GetMembersAsync_ListExists_AppliesTodoListMembersSpecification()
	{
		// Arrange
		string todoListId = "ID";
		PaginationParameters parameters = new(2, 5);
		WebApplicationFactory.TodoMembersHelperService
			.GetPageAsync<TodoListMemberPreviewDto>(todoListId, Arg.Any<ISpecification<TodoListMember>>(), Arg.Any<PaginationParameters>())
			.Returns(new Page<TodoListMemberPreviewDto>([], 1, 1, 1));

		// Act
		var result = await _service.GetMembersAsync(todoListId, parameters);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoMembersHelperService
			.Received()
			.GetPageAsync<TodoListMemberPreviewDto>(todoListId,
			Arg.Is<TodoListMembersSpecification>(x => x.TodoListId == todoListId),
			Arg.Any<PaginationParameters>());
	}

	[Test]
	public async Task AddMemberAsync_MemberDoesNotExist_IndicatesSuccess()
	{
		// Arrange
		string todoListId = "Id";
		string userId = "UserId";
		TodoListMemberAddDto inputDto = new(userId, null);
		TodoListMemberMinimalViewDto outputDto = new(500, userId, todoListId, null);
		WebApplicationFactory.TodoListMembersRepository
			.FindAsync(todoListId, userId)
			.ReturnsNull();
		WebApplicationFactory.TodoMembersHelperService
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(todoListId, inputDto)
			.Returns(outputDto);

		// Act
		var result = await _service.AddMemberAsync(todoListId, inputDto);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Status, Is.EqualTo(AddTodoListMemberResultStatus.Success));
			Assert.That(result.Dto, Is.EqualTo(outputDto));
		});
		await WebApplicationFactory.TodoMembersHelperService
			.Received()
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(todoListId, inputDto);
	}

	[Test]
	public async Task AddMemberAsync_MemberExists_ReturnsUserAlreadyAddedStatus()
	{
		// Arrange
		string todoListId = "Id";
		string userId = "UserId";
		TodoListMemberAddDto inputDto = new(userId, null);
		WebApplicationFactory.TodoListMembersRepository
			.FindAsync(todoListId, userId)
			.Returns(new TodoListMember()
			{
				TodoListId = todoListId,
				UserId = userId
			});

		// Act
		var result = await _service.AddMemberAsync(todoListId, inputDto);

		// Assert
		Assert.That(result.Status, Is.EqualTo(AddTodoListMemberResultStatus.UserAlreadyAdded));
	}

	[Test]
	public async Task AddMemberAsync_TodoListDoesNotExist_ReturnsNotFoundStatus()
	{
		// Arrange
		string todoListId = "Id";
		string userId = "UserId";
		TodoListMemberAddDto inputDto = new(userId, null);
		WebApplicationFactory.TodoListMembersRepository
			.FindAsync(todoListId, userId)
			.ReturnsNull();
		WebApplicationFactory.TodoMembersHelperService
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(todoListId, inputDto)
			.ReturnsNull();

		// Act
		var result = await _service.AddMemberAsync(todoListId, inputDto);

		// Assert
		Assert.That(result.Status, Is.EqualTo(AddTodoListMemberResultStatus.NotFound));
	}

	// Tests for other methods are useless, because they are just wrappers.
}
