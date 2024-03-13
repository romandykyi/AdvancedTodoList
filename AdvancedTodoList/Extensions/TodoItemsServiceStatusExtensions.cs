using AdvancedTodoList.Core.Services;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdvancedTodoList.Extensions;

public static class TodoItemsServiceStatusExtensions
{
	public static IActionResult ToActionResult(this TodoItemsServiceStatus status)
	{
		switch (status)
		{
			case TodoItemsServiceStatus.Success:
				return new NoContentResult();

			case TodoItemsServiceStatus.NotFound:
				return new NotFoundResult();

			case TodoItemsServiceStatus.Forbidden:
				return new ForbidResult();

			case TodoItemsServiceStatus.InvalidCategoryId:
				ModelStateDictionary modelState = new();
				modelState.AddModelError("CategoryId", "Category ID is invalid.");
				return new BadRequestObjectResult(modelState);
		}
		throw new ArgumentException("Invalid to-do items service response status", nameof(status));
	}
}
