using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs
{
    public class ActorUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Range(1900, 2100)]
        public int BirthYear { get; set; }
    }
}