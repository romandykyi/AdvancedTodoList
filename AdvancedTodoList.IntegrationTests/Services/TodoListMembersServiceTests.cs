//using AdvancedTodoList.Core.Dtos;
//using AdvancedTodoList.Core.Models.TodoLists.Members;
//using AdvancedTodoList.Core.Pagination;
//using AdvancedTodoList.Core.Services;
//using AdvancedTodoList.Core.Specifications;
//using AdvancedTodoList.Infrastructure.Specifications;
//using AdvancedTodoList.IntegrationTests.Fixtures;
//using NUnit.Framework.Internal;

//namespace AdvancedTodoList.IntegrationTests.Services;

//[TestFixture]
//public class TodoListMembersServiceTests : BusinessLogicFixture
//{
//	private ITodoListMembersService _service;
//	private readonly TodoListContext TestContext = new("TestTodoListId", "TestUserId");

//	private void RoleExists(int roleId, string todoListId)
//	{
//		WebApplicationFactory.TodoListRolesRepository
//			.GetByIdAsync(roleId)
//			.Returns(new TodoListRole() { Id = roleId, TodoListId = todoListId, Name = "Ok", Priority = 1 });
//	}

//	[SetUp]
//	public void SetUp()
//	{
//		_service = ServiceScope.ServiceProvider.GetService<ITodoListMembersService>()!;
//	}

//	[Test]
//	public async Task GetMembersAsync_ListExists_AppliesTodoListMembersSpecification()
//	{
//		// Arrange
//		PaginationParameters parameters = new(2, 5);
//		Page<TodoListMemberPreviewDto> testPage = new([], 1, 1, 1);
//		WebApplicationFactory.TodoMembersHelperService
//			.GetPageAsync<TodoListMemberPreviewDto>(TestContext, Arg.Any<ISpecification<TodoListMember>>(), Arg.Any<PaginationParameters>())
//			.Returns(new ServiceResponse<Page<TodoListMemberPreviewDto>>(ServiceResponseStatus.Success, testPage));

//		// Act
//		var result = await _service.GetMembersAsync(TestContext, parameters);

//		// Assert
//		Assert.That(result, Is.Not.Null);
//		await WebApplicationFactory.TodoMembersHelperService
//			.Received()
//			.GetPageAsync<TodoListMemberPreviewDto>(TestContext,
//			Arg.Is<TodoListMembersSpecification>(x => x.TodoListId == TestContext.TodoListId),
//			Arg.Any<PaginationParameters>());
//	}

//	[Test]
//	public async Task AddMemberAsync_MemberDoesNotExist_IndicatesSuccess()
//	{
//		// Arrange
//		string todoListId = "Id";
//		string userId = "UserId";
//		TodoListMemberAddDto inputDto = new(userId);
//		TodoListMemberMinimalViewDto outputDto = new(500, userId, todoListId, null);
//		WebApplicationFactory.TodoListMembersRepository
//			.FindAsync(todoListId, userId)
//			.ReturnsNull();
//		WebApplicationFactory.TodoMembersHelperService
//			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(
//				TestContext, inputDto, Arg.Any<Func<RolePermissions, bool>>())
//			.Returns(new ServiceResponse<TodoListMemberMinimalViewDto>(
//				ServiceResponseStatus.Success, outputDto));

//		// Act
//		var result = await _service.AddMemberAsync(TestContext, inputDto);

//		// Assert
//		Assert.Multiple(() =>
//		{
//			Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.Success));
//			Assert.That(result.Dto, Is.EqualTo(outputDto));
//		});
//		// Assert that create async was called with valid permission accessor
//		// (should return true if AddMembers is true)
//		RolePermissions addMembers = new(AddMembers: true);
//		await WebApplicationFactory.TodoMembersHelperService
//			.Received()
//			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(
//			TestContext, inputDto, Arg.Is<Func<RolePermissions, bool>>(x => x(addMembers)));
//	}

//	[Test]
//	public async Task AddMemberAsync_MemberExists_ReturnsUserAlreadyAddedStatus()
//	{
//		// Arrange
//		string todoListId = "Id";
//		string userId = "UserId";
//		TodoListMemberAddDto inputDto = new(userId);
//		WebApplicationFactory.TodoListMembersRepository
//			.FindAsync(todoListId, userId)
//			.Returns(new TodoListMember()
//			{
//				TodoListId = todoListId,
//				UserId = userId
//			});

//		// Act
//		var result = await _service.AddMemberAsync(TestContext, inputDto);

//		// Assert
//		Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.UserAlreadyAdded));
//	}

//	[Test]
//	public async Task AddMemberAsync_TodoListDoesNotExist_ReturnsNotFoundStatus()
//	{
//		// Arrange
//		string todoListId = "Id";
//		string userId = "UserId";
//		TodoListMemberAddDto inputDto = new(userId);
//		WebApplicationFactory.TodoListMembersRepository
//			.FindAsync(todoListId, userId)
//			.ReturnsNull();
//		WebApplicationFactory.TodoMembersHelperService
//			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(TestContext, inputDto,
//			Arg.Any<Func<RolePermissions, bool>>())
//			.Returns(new ServiceResponse<TodoListMemberMinimalViewDto>(ServiceResponseStatus.NotFound));

//		// Act
//		var result = await _service.AddMemberAsync(TestContext, inputDto);

//		// Assert
//		Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.NotFound));
//	}

//	[Test]
//	public async Task AddMemberAsync_UserHasNoPermission_ReturnsForbiddenStatus()
//	{
//		// Arrange
//		string todoListId = "Id";
//		string userId = "UserId";
//		TodoListMemberAddDto inputDto = new(userId);
//		WebApplicationFactory.TodoListMembersRepository
//			.FindAsync(todoListId, userId)
//			.ReturnsNull();
//		WebApplicationFactory.TodoMembersHelperService
//			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(TestContext, inputDto,
//			Arg.Any<Func<RolePermissions, bool>>())
//			.Returns(new ServiceResponse<TodoListMemberMinimalViewDto>(ServiceResponseStatus.Forbidden));

//		// Act
//		var result = await _service.AddMemberAsync(TestContext, inputDto);

//		// Assert
//		Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.Forbidden));
//	}

//	[Test]
//	public async Task UpdateMemberRoleAsync_Success_ReturnsSuccessStatus()
//	{
//		// Arrange
//		int memberId = 121;
//		int roleId = 555;
//		TodoListMemberUpdateRoleDto dto = new(roleId);
//		RoleExists(roleId, TestContext.TodoListId);
//		WebApplicationFactory.TodoMembersHelperService
//			.UpdateAsync(TestContext, memberId, dto, Arg.Any<Func<RolePermissions, bool>>())
//			.Returns(ServiceResponseStatus.Success);

//		// Act
//		var result = await _service.UpdateMemberRoleAsync(TestContext, memberId, dto);

//		// Assert
//		Assert.That(result, Is.EqualTo(TodoListMemberServiceResultStatus.Success));
//		// Assert that create async was called with valid permission accessor
//		// (should return true if AssignMembers is true)
//		RolePermissions assignRoles = new(AssignRoles: true);
//		await WebApplicationFactory.TodoMembersHelperService
//			.Received()
//			.UpdateAsync(TestContext, memberId, dto,
//			Arg.Is<Func<RolePermissions, bool>>(x => x(assignRoles)));
//	}

//	[Test]
//	public async Task UpdateMemberRoleAsync_MemberDoesNotExist_ReturnsNotFoundStatus()
//	{
//		// Arrange
//		int memberId = 121;
//		int roleId = 777;
//		TodoListMemberUpdateRoleDto dto = new(roleId);
//		RoleExists(roleId, TestContext.TodoListId);
//		WebApplicationFactory.TodoMembersHelperService
//			.UpdateAsync(TestContext, memberId, dto, Arg.Any<Func<RolePermissions, bool>>())
//			.Returns(ServiceResponseStatus.NotFound);

//		// Act
//		var result = await _service.UpdateMemberRoleAsync(TestContext, memberId, dto);

//		// Assert
//		Assert.That(result, Is.EqualTo(TodoListMemberServiceResultStatus.NotFound));
//	}

//	[Test]
//	public async Task UpdateMemberRoleAsync_UserHasNoPermission_ReturnsForbiddenStatus()
//	{
//		// Arrange
//		int memberId = 121;
//		int roleId = 777;
//		TodoListMemberUpdateRoleDto dto = new(roleId);
//		RoleExists(roleId, TestContext.TodoListId);
//		WebApplicationFactory.TodoMembersHelperService
//			.UpdateAsync(TestContext, memberId, dto, Arg.Any<Func<RolePermissions, bool>>())
//			.Returns(ServiceResponseStatus.Forbidden);

//		// Act
//		var result = await _service.UpdateMemberRoleAsync(TestContext, memberId, dto);

//		// Assert
//		Assert.That(result, Is.EqualTo(TodoListMemberServiceResultStatus.Forbidden));
//	}

//	[Test]
//	public async Task UpdateMemberRoleAsync_RoleDoesNotExist_ReturnsInvalidRoleIdStatus()
//	{
//		// Arrange
//		int memberId = 121;
//		int roleId = 777;
//		TodoListMemberUpdateRoleDto dto = new(roleId);
//		WebApplicationFactory.TodoListRolesRepository
//			.GetByIdAsync(roleId)
//			.ReturnsNull();

//		// Act
//		var result = await _service.UpdateMemberRoleAsync(TestContext, memberId, dto);

//		// Assert
//		Assert.That(result, Is.EqualTo(TodoListMemberServiceResultStatus.InvalidRoleId));
//	}

//	[Test]
//	public async Task UpdateMemberRoleAsync_InvalidRoleId_ReturnsInvalidRoleIdStatus()
//	{
//		// Arrange
//		int memberId = 121;
//		int roleId = 777;
//		TodoListMemberUpdateRoleDto dto = new(roleId);
//		RoleExists(roleId, "InvalidTodoId");

//		// Act
//		var result = await _service.UpdateMemberRoleAsync(TestContext, memberId, dto);

//		// Assert
//		Assert.That(result, Is.EqualTo(TodoListMemberServiceResultStatus.InvalidRoleId));
//	}

//	// Tests for other methods are useless, because they are just wrappers.
//}
