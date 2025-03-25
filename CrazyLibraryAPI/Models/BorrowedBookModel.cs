namespace CrazyLibraryAPI.Models
{
    public class BorrowedBookModel
    {
        public string BookUID { get; set; }
        public string Author { get; set; }
        public string BookName { get; set; }
        public DateTime BorrowedOn { get; set; }
    }
}
