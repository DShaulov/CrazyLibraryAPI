using System.ComponentModel.DataAnnotations;

namespace CrazyLibraryAPI.Models
{
    public class Customer
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string Passport { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(20)]
        public string Zip { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
