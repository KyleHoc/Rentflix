using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Rentflix.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Director { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        [MaxLength(500)]
        public string Synopsis { get; set; }
        [Required]
        [DisplayName("Year")]
        public int ReleaseYear { get; set; }
        [DisplayName("Image")]
        [ValidateNever]
        public string? ImageUrl { get; set; }
    }
}
