using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CrazyLibraryAPI.Models
{
    public class BookHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [ForeignKey("Book")]
        public string BookUniqueID { get; set; }

        public Book Book { get; set; }

        [Required]
        [StringLength(50)]
        [ForeignKey("Customer")]

        public string CustomerPassport { get; set; }

        public Customer Customer { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(20)]
        public string Action { get; set; }  // "Borrow" or "Return"

    }
}
