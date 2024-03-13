using AdvancedTodoList.Core.Mapping;
using AdvancedTodoList.Core.Models.Auth;
using AdvancedTodoList.Core.Models.TodoLists;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Core.Options;
using AdvancedTodoList.Core.Repositories;
using AdvancedTodoList.Core.Services;
using AdvancedTodoList.Core.Services.Auth;
using AdvancedTodoList.Core.Validation;
using AdvancedTodoList.Infrastructure.Data;
using AdvancedTodoList.Infrastructure.Repositories;
using AdvancedTodoList.Infrastructure.Services;
using AdvancedTodoList.Infrastructure.Services.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter access token (JWT)",
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey
	});

	var scheme = new OpenApiSecurityScheme
	{
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = "Bearer"
		}
	};

	options.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });

	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Configure antiforgery
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-Token");

// Configure auth
string? jwtSecret = builder.Configuration["Auth:AccessToken:SecretKey"] ??
	throw new InvalidOperationException("JWT secret is not configured");
builder.Services.AddAuthentication()
	.AddJwtBearer(options =>
	{
		options.SaveToken = true;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ClockSkew = TimeSpan.FromSeconds(5),
			ValidIssuer = builder.Configuration["Auth:AccessToken:ValidIssuer"],
			ValidAudience = builder.Configuration["Auth:AccessToken:ValidAudience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
		};
	});
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

// Bind options
builder.Services.Configure<AccessTokenOptions>(
	builder.Configuration.GetSection("Auth:AccessToken"));
builder.Services.Configure<RefreshTokenOptions>(
	builder.Configuration.GetSection("Auth:RefreshToken"));

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionsChecker, PermissionsChecker>();
builder.Services.AddScoped<ITodoListsService, TodoListsService>();
builder.Services.AddScoped<ITodoItemsService, TodoItemsService>();
builder.Services.AddScoped<ITodoItemCategoriesService, TodoItemCategoriesService>();
builder.Services.AddScoped<ITodoListMembersService, TodoListMembersService>();
builder.Services.AddScoped<ITodoListRolesService, TodoListRolesService>();
builder.Services.AddScoped<IEntityExistenceChecker, EntityExistenceChecker>();
builder.Services.AddSingleton<IAccessTokensService, AccessTokensService>();
builder.Services.AddScoped<IRefreshTokensService, RefreshTokensService>();
builder.Services.AddScoped(typeof(ITodoListDependantEntitiesService<,>),
	typeof(TodoListDependantEntitiesService<,>));
// Register unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Register application repositories
builder.Services.AddScoped<IRepository<TodoList, string>, TodoListRepository>();
builder.Services.AddScoped<IRepository<TodoItem, int>, TodoItemsRepository>();
builder.Services.AddScoped<IRepository<TodoItemCategory, int>, TodoItemCategoriesRepository>();
builder.Services.AddScoped<ITodoListMembersRepository, TodoListMembersRepository>();
builder.Services.AddScoped<IRepository<TodoListMember, int>, TodoListMembersRepository>();
builder.Services.AddScoped<IRepository<TodoListRole, int>, TodoListRolesRepository>();
builder.Services.AddScoped<IRepository<UserRefreshToken, int>, UserRefreshTokensRepository>();
builder.Services.AddScoped<IUserRefreshTokensRepository, UserRefreshTokensRepository>();

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
