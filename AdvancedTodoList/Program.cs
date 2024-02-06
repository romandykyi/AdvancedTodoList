using AdvancedTodoList.Core.Models;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-Token");

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
