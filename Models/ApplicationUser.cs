using Microsoft.AspNetCore.Identity;

namespace GiftOfTheGivers.Models
{
    // Extends the built-in Identity user with fields relevant to the Foundation.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
}
