using Microsoft.AspNetCore.Identity;

namespace VirtualEventTicketing.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
}