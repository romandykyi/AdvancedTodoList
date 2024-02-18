using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
	IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<TodoList> TodoLists { get; set; }
	public DbSet<TodoItem> TodoItems { get; set; }
	public DbSet<TodoListMember> TodoListsMembers { get; set; }
	public DbSet<TodoListMemberRole> TodoListMemberRoles { get; set; }
	public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
}
