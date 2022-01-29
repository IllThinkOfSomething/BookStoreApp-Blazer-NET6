using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs.Author
{
    public class AuthorCreateDto
    {
        // Restrictions on API Level
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(250)]
        public string Bio { get; set; }
    }
}
