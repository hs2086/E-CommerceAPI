using System.ComponentModel.DataAnnotations;

namespace E_COMMERCEAPI.DTOs
{
    public class AddRoleDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
