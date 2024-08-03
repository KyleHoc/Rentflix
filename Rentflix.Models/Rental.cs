using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentflix.Models
{
    public class Rental
    {
        [Key][Column(Order = 0)]
        public int MovieId { get; set; }

        [Key][Column(Order = 1)]
        public int UserId { get; set; }
        [Required]
        [Column(Order = 2)]
        public DateTime RentDate { get; set; }
        [Column(Order = 3)]
        public DateTime ReturnDate { get; set; }
    }
}
