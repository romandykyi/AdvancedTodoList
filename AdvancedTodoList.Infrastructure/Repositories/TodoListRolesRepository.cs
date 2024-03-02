﻿using AdvancedTodoList.Core.Models.TodoLists.Members;
using AdvancedTodoList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AdvancedTodoList.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for CRUD operations on to-do lists roles.
/// </summary>
public class TodoListRolesRepository(ApplicationDbContext dbContext) :
	BaseRepository<TodoListRole, int>(dbContext)
{
}
