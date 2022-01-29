using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.DTOs.Author
{
    public class BookCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [Range(1800, 2022)]
        public int Year { get; set; }

        [Required]
        public string Isbn { get; set; }

        [Required]
        [StringLength(250, MinimumLength =10)]
        public string Summary { get; set; }

        [MaxLength(200)]
        public string Image { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public double Price { get; set; }
    }
}
