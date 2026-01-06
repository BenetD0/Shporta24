using Microsoft.AspNetCore.Identity;

namespace Shporta24.Models
{
    public class ApplicationUser : IdentityUser
    {
        // mund të shtosh fushat extra nëse do
        public string FullName { get; set; }
    }
}
