using AdvancedTodoList.Core.Mapping;
using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Validation;
using AdvancedTodoList.Infrastructure.Data;
using AdvancedTodoList.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
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

// Apply mapping settings
MappingGlobalSettings.Apply();

// Add fluent validation
ValidatorOptions.Global.LanguageManager.Enabled = false; // Disable localization
builder.Services.AddValidatorsFromAssemblyContaining<TodoItemCreateDtoValidator>();

// Enable auto validation by SharpGrip
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
	// Disable the built-in .NET model (data annotations) validation.
	configuration.DisableBuiltInModelValidation = true;
	// Enable validation for parameters bound from `BindingSource.Body` binding sources.
	configuration.EnableBodyBindingSourceAutomaticValidation = true;
	// Enable validation for parameters bound from `BindingSource.Query` binding sources.
	configuration.EnableQueryBindingSourceAutomaticValidation = true;
});


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
