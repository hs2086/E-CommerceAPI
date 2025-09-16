using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;

namespace E_COMMERCEAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }           
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
