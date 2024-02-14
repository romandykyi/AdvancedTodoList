using Microsoft.AspNetCore.Identity;

namespace AdvancedTodoList.Core.Models;

public class ApplicationUser : IdentityUser, IEntity<string>
{
}
