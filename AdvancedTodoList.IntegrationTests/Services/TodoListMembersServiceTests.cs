using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Pagination;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Specifications;
using AdvancedTodoList.Infrastructure.Specifications;
using AdvancedTodoList.IntegrationTests.Fixtures;
using AdvancedTodoList.IntegrationTests.Utils;
using NUnit.Framework.Internal;

namespace AdvancedTodoList.IntegrationTests.Services;

[TestFixture]
public class TodoListMembersServiceTests : BusinessLogicFixture
{
	private ITodoListMembersService _service;
	private readonly TodoListContext TestContext = new("TestTodoListId", "TestUserId");
	private const int DefaultPriority = 1;

	private void RoleExists(int roleId, string todoListId)
	{
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(roleId)
			.Returns(new TodoListRole() { Id = roleId, TodoListId = todoListId, Name = "Ok", Priority = DefaultPriority });
	}

	[SetUp]
	public void SetUp()
	{
		_service = ServiceScope.ServiceProvider.GetService<ITodoListMembersService>()!;
	}

	[Test]
	public async Task GetMembersAsync_ListExists_AppliesTodoListMembersSpecification()
	{
		// Arrange
		PaginationParameters parameters = new(2, 5);
		TodoListMembersFilter filter = new([], "Test");
		Page<TodoListMemberPreviewDto> testPage = new([], 1, 1, 1);
		WebApplicationFactory.TodoMembersHelperService
			.GetPageAsync<TodoListMemberPreviewDto>(TestContext, Arg.Any<ISpecification<TodoListMember>>(), Arg.Any<PaginationParameters>())
			.Returns(new ServiceResponse<Page<TodoListMemberPreviewDto>>(ServiceResponseStatus.Success, testPage));

		// Act
		var result = await _service.GetMembersAsync(TestContext, parameters, filter);

		// Assert
		Assert.That(result, Is.Not.Null);
		await WebApplicationFactory.TodoMembersHelperService
			.Received()
			.GetPageAsync<TodoListMemberPreviewDto>(TestContext,
			Arg.Is<TodoListMembersSpecification>(x =>
			x.TodoListId == TestContext.TodoListId && x.Filter.UserId == filter.UserId),
			Arg.Any<PaginationParameters>());
	}

	[Test]
	public async Task AddMemberAsync_MemberDoesNotExist_IndicatesSuccess()
	{
		// Arrange
		string todoListId = "Id";
		string userId = "UserId";
		TodoListMemberAddDto inputDto = new(userId);
		TodoListMemberMinimalViewDto outputDto = new(500, userId, todoListId, null);
		WebApplicationFactory.TodoListMembersRepository
			.FindAsync(todoListId, userId)
			.ReturnsNull();
		WebApplicationFactory.PermissionsChecker
			.HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);
		WebApplicationFactory.TodoMembersHelperService
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(
				TestContext, inputDto, null)
			.Returns(new ServiceResponse<TodoListMemberMinimalViewDto>(
				ServiceResponseStatus.Success, outputDto));

		// Act
		var result = await _service.AddMemberAsync(TestContext, inputDto);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.Success));
			Assert.That(result.Dto, Is.EqualTo(outputDto));
		});
		// Assert that create async was called with valid permission accessor
		// (should return true if AddMembers is true)
		RolePermissions addMembers = new(AddMembers: true);
		await WebApplicationFactory.TodoMembersHelperService
			.Received()
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(
			TestContext, inputDto, null);
		await WebApplicationFactory.PermissionsChecker
			.Received()
			.HasPermissionAsync(TestContext, Arg.Is<Func<RolePermissions, bool>>(x => x(addMembers)));
	}

	[Test]
	public async Task AddMemberAsync_MemberExists_ReturnsUserAlreadyAddedStatus()
	{
		// Arrange
		string userId = "UserId";
		TodoListMemberAddDto inputDto = new(userId);
		WebApplicationFactory.TodoListMembersRepository
			.FindAsync(TestContext.TodoListId, userId)
			.Returns(new TodoListMember()
			{
				TodoListId = TestContext.TodoListId,
				UserId = userId
			});
		WebApplicationFactory.PermissionsChecker
			.HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);

		// Act
		var result = await _service.AddMemberAsync(TestContext, inputDto);

		// Assert
		Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.UserAlreadyAdded));
	}

	[Test]
	public async Task AddMemberAsync_TodoListDoesNotExist_ReturnsNotFoundStatus()
	{
		// Arrange
		string todoListId = "Id";
		string userId = "UserId";
		TodoListMemberAddDto inputDto = new(userId);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(true);
		WebApplicationFactory.TodoListMembersRepository
			.FindAsync(todoListId, userId)
			.ReturnsNull();
		WebApplicationFactory.TodoMembersHelperService
			.CreateAsync<TodoListMemberAddDto, TodoListMemberMinimalViewDto>(TestContext, inputDto, null)
			.Returns(new ServiceResponse<TodoListMemberMinimalViewDto>(ServiceResponseStatus.NotFound));

		// Act
		var result = await _service.AddMemberAsync(TestContext, inputDto);

		// Assert
		Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.NotFound));
	}

	[Test]
	public async Task AddMemberAsync_UserHasNoPermission_ReturnsForbiddenStatus()
	{
		// Arrange
		string userId = "UserId";
		TodoListMemberAddDto inputDto = new(userId);
		WebApplicationFactory.PermissionsChecker
			.HasPermissionAsync(TestContext, Arg.Any<Func<RolePermissions, bool>>())
			.Returns(false);

		// Act
		var result = await _service.AddMemberAsync(TestContext, inputDto);

		// Assert
		Assert.That(result.Status, Is.EqualTo(TodoListMemberServiceResultStatus.Forbidden));
	}

	private ISpecification<TodoListMember> ValidSpecification =>
		Arg.Is<MemberPermissionsSpecification>(x => x.UserId == TestContext.CallerId && x.TodoListId == TestContext.TodoListId);

	private const int TestCallerPriority = 3;
	private readonly TodoListMemberUpdateRoleDto TestUpdateRoleDto = new(321);
	private readonly PermissionsAggregate ValidPermissions = new(
		new(DefaultPriority, new RolePermissions(AssignRoles: true)));

	private static IEnumerable<PermissionsAggregate?> CallerHasNoAssignRolePermissionCases =>
		[
			null,
			new PermissionsAggregate(null),
			new PermissionsAggregate(new RoleEssentials(TestCallerPriority, new RolePermissions(AssignRoles: false)))
		];

	[Test]
	[TestCaseSource(nameof(CallerHasNoAssignRolePermissionCases))]
	public async Task UpdateMemberRoleAsync_CallerHasNoAssignRolePermission_ReturnsForbidden(PermissionsAggregate? aggregate)
	{
		// Arrange
		const int memberId = 123;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(aggregate);

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.Forbidden));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_NewRoleDoesNotExist_ReturnsInvalidRoleId()
	{
		// Arrange
		const int memberId = 123;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(TestUpdateRoleDto.RoleId!.Value)
			.ReturnsNull();

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.InvalidRoleId));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_NewRoleIsFromAnotherTodoList_ReturnsInvalidRoleId()
	{
		// Arrange
		const int memberId = 123;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(TestUpdateRoleDto.RoleId!.Value)
			.Returns(TestModels.CreateTestRole("Wrong to-do list ID"));

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.InvalidRoleId));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_CallerCannotAssignNewRole_ReturnsForbidden()
	{
		// Arrange
		const int memberId = 123;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		var role = TestModels.CreateTestRole(TestContext.TodoListId);
		role.Priority = 0;
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(TestUpdateRoleDto.RoleId!.Value)
			.Returns(role);

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.Forbidden));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_MemberDoesNotExist_ReturnsNotFound()
	{
		// Arrange
		const int memberId = 123;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(TestUpdateRoleDto.RoleId!.Value)
			.Returns(TestModels.CreateTestRole(TestContext.TodoListId));
		WebApplicationFactory.TodoListMembersRepository
			.GetByIdAsync(memberId)
			.ReturnsNull();

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.NotFound));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_MemberIsFromAnotherTodoList_ReturnsNotFound()
	{
		// Arrange
		const int memberId = 123;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(TestUpdateRoleDto.RoleId!.Value)
			.Returns(TestModels.CreateTestRole(TestContext.TodoListId));
		WebApplicationFactory.TodoListMembersRepository
			.GetByIdAsync(memberId)
			.Returns(new TodoListMember() { TodoListId = "Wrong", UserId = "UserID" });

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.NotFound));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_MemberHasHigherPriorityThanCaller_ReturnsForbidden()
	{
		// Arrange
		const int memberId = 123;
		const int memberRoleId = 777;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(TestUpdateRoleDto.RoleId!.Value)
			.Returns(TestModels.CreateTestRole(TestContext.TodoListId));
		WebApplicationFactory.TodoListMembersRepository
			.GetByIdAsync(memberId)
			.Returns(new TodoListMember() { TodoListId = TestContext.TodoListId, UserId = "UserID", RoleId = memberRoleId });
		var memberRole = TestModels.CreateTestRole(TestContext.TodoListId);
		memberRole.Priority = 0;
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(memberRoleId)
			.Returns(memberRole);

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.Forbidden));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_AssignNullRole_Succeeds()
	{
		// Arrange
		const int memberId = 123;
		const int memberRoleId = 777;
		TodoListMemberUpdateRoleDto dto = new(null);
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		WebApplicationFactory.TodoListMembersRepository
			.GetByIdAsync(memberId)
			.Returns(new TodoListMember() { TodoListId = TestContext.TodoListId, UserId = "UserID", RoleId = memberRoleId });
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(memberRoleId)
			.Returns(TestModels.CreateTestRole(TestContext.TodoListId));

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, dto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.Success));
	}

	[Test]
	public async Task UpdateMemberRoleAsync_AssignToMemberWithNoRole_Succeeds()
	{
		// Arrange
		const int memberId = 123;
		WebApplicationFactory.TodoListMembersRepository
			.GetAggregateAsync<PermissionsAggregate>(ValidSpecification)
			.Returns(ValidPermissions);
		WebApplicationFactory.TodoListRolesRepository
			.GetByIdAsync(TestUpdateRoleDto.RoleId!.Value)
			.Returns(TestModels.CreateTestRole(TestContext.TodoListId));
		WebApplicationFactory.TodoListMembersRepository
			.GetByIdAsync(memberId)
			.Returns(new TodoListMember() { TodoListId = TestContext.TodoListId, UserId = "UserID", RoleId = null });

		// Act
		var response = await _service.UpdateMemberRoleAsync(TestContext, memberId, TestUpdateRoleDto);

		// Assert
		Assert.That(response, Is.EqualTo(TodoListMemberServiceResultStatus.Success));
	}

	// Tests for other methods are useless, because they are just wrappers.
}
