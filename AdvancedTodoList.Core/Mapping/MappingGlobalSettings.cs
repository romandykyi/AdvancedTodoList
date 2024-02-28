using AdvancedTodoList.Core.Dtos;
using AdvancedTodoList.Core.Models.TodoLists.Members;
using Mapster;
using System.Text.RegularExpressions;

namespace AdvancedTodoList.Core.Mapping;

/// <summary>
/// Class that defines global mapping settings.
/// </summary>
public static class MappingGlobalSettings
{
	/// <summary>
	/// Apply global mapping settings.
	/// </summary>
	public static void Apply()
	{
		// Ignore null IDs
		TypeAdapterConfig<TodoListMember, TodoListMemberPreviewDto>.NewConfig()
			.IgnoreIf((src, dest) => src.RoleId == null, dest => dest.Role!);

		// Convert null strings into empty strings and trim strings
		TypeAdapterConfig.GlobalSettings.Default
			.AddDestinationTransform((string? dest) => dest != null ? dest.Trim() : string.Empty);
	}
}
