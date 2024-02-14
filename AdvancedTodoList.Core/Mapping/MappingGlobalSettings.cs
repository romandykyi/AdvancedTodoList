using Mapster;

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
		// Convert null strings into empty strings and trim strings
		TypeAdapterConfig.GlobalSettings.Default
			.AddDestinationTransform((string? dest) => dest != null ? dest.Trim() : string.Empty);
	}
}
