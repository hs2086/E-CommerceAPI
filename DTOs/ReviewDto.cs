using System.ComponentModel.DataAnnotations;

namespace E_COMMERCEAPI.DTOs
{
    public class ReviewDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }
    }
}
