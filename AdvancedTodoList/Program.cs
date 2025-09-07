using AdvancedTodoList.Application.Mapping;
using AdvancedTodoList.WebApp.Extensions.ServiceCollection;
using AdvancedTodoList.WebApp.Extensions.WebAppBuilder;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpoints()
    .AddApplicationSwagger()
    .AddApplicationAntiforgery();

builder.AddAuth();
builder.AddDataAccess();

// Bind options
builder.ConfigureApplicationOptions();

// Register application services
builder.Services
    .AddDataAccessServices()
    .AddApplicationServices();

// Apply mapping settings
MappingGlobalSettings.Apply();

// Add fluent validation
builder.Services
    .AddFluentValidation()
    .AddFluentValidationAutoValidation();

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
