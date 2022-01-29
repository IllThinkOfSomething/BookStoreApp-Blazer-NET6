using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs.Author
{
    public class CreateBookDto : Profile
    {
        [Required]
        [MaxLength(35)]
        public string Title { get; set; }

        [Required]
        [MaxLength(4)]
        [MinLength(4)]
        public int Year { get; set; }

        [MaxLength(4)]
        public string Summary { get; set; }

        [MaxLength(200)]
        public string Image { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public double Price { get; set; }
    }
}
