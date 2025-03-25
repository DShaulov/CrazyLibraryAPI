using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrazyLibraryAPI.Models
{
    public class Book
    {
        [Key]
        [Required]
        [StringLength(50)]
        public string UniqueID { get; set; }

        [StringLength(300)]
        public string Title { get; set; }

        [StringLength(5000)]
        public string Description { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public Author Author { get; set; }

        public DateTime PublicationDate { get; set; }

        [StringLength(200)]
        public string Image_url { get; set; }

        [StringLength(100)]
        public string LibraryCallNumber { get; set; }

        public int TotalCopies { get; set; }

        public int CopiesAvailable { get; set; }
    }
}
