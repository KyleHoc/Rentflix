using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentflix.Models
{
    public class Rental
    {  
       public int Id { get; set; }

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        [ValidateNever]
        public Movie Movie { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }


        [Required]
        public DateTime RentDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }
    }
}
