using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdvancedTodoList.Core.Models.Auth;

public class ApplicationUser : IdentityUser, IEntity<string>
{
	[MaxLength(MaxNameLength)]
	public required string FirstName { get; set; }
	[MaxLength(MaxNameLength)]
	public required string LastName { get; set; }

	/// <summary>
	/// Maximum length for properties <see cref="FirstName" /> and <see cref="LastName"/>.
	/// </summary>
	public const int MaxNameLength = 100;
}
