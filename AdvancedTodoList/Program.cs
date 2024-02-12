using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Infrastructure.Data;
using AdvancedTodoList.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure antiforgery
builder.Services.AddAntiforgery(o => o.HeaderName = "X-XSRF-Token");

// Configure auth
builder.Services.AddAuthentication()
	.AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

// Get connection string
string? connectionString =
	builder.Configuration.GetConnectionString("DefaultConnection") ??
	throw new InvalidOperationException("Connection string is not specified in 'ConnectionStrings:DefaultConnection'");

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(
	o => o.UseSqlServer(connectionString,
	b => b.MigrationsAssembly("AdvancedTodoList.Infrastructure"))
	);

builder.Services.AddIdentityCore<ApplicationUser>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddApiEndpoints();

// Register application services
builder.Services.AddScoped<ITodoListsService, TodoListsService>();
builder.Services.AddScoped<ITodoItemsService, TodoItemsService>();
builder.Services.AddScoped<IEntityExistenceChecker, EntityExistenceChecker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
