using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Models.TodoLists;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
	IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<TodoList> TodoLists { get; set; }
	public DbSet<TodoItem> TodoItems { get; set; }
}
