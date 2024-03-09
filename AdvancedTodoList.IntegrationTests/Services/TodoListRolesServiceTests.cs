//using AdvancedTodoList.Core.Dtos;
//using AdvancedTodoList.Core.Models.TodoLists.Members;
//using AdvancedTodoList.Core.Pagination;
//using AdvancedTodoList.Core.Services;
//using AdvancedTodoList.Core.Specifications;
//using AdvancedTodoList.Infrastructure.Specifications;
//using AdvancedTodoList.IntegrationTests.Fixtures;

//namespace AdvancedTodoList.IntegrationTests.Services;

//[TestFixture]
//public class TodoListRolesServiceTests : BusinessLogicFixture
//{
//	private ITodoListRolesService _service;

//	[SetUp]
//	public void SetUp()
//	{
//		_service = ServiceScope.ServiceProvider.GetService<ITodoListRolesService>()!;
//	}

//	[Test]
//	public async Task GetRolesOfListAsync_ListExists_AppliesTodoItemsSpecification()
//	{
//		// Arrange
//		string todoListId = "ID";
//		PaginationParameters parameters = new(2, 5);
//		WebApplicationFactory.TodoRolesHelperService
//			.GetPageAsync<TodoListRolePreviewDto>(todoListId, Arg.Any<ISpecification<TodoListRole>>(), Arg.Any<PaginationParameters>())
//			.Returns(new Page<TodoListRolePreviewDto>([], 1, 1, 1));

//		// Act
//		var result = await _service.GetRolesOfListAsync(todoListId, parameters);

//		// Assert
//		Assert.That(result, Is.Not.Null);
//		await WebApplicationFactory.TodoRolesHelperService
//			.Received()
//			.GetPageAsync<TodoListRolePreviewDto>(todoListId,
//			Arg.Is<TodoListDependantEntitiesSpecification<TodoListRole>>(x => x.TodoListId == todoListId),
//			Arg.Any<PaginationParameters>());
//	}
//}
