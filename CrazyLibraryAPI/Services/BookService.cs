using CrazyLibraryAPI.Data;
using CrazyLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrazyLibraryAPI.Services
{
    public class BookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string bookName, string authorFirstName, string authorLastName)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b =>
                    (!string.IsNullOrEmpty(bookName) && b.Title.StartsWith(bookName)) ||
                    (!string.IsNullOrEmpty(authorFirstName) && b.Author.FirstName.StartsWith(authorFirstName)) ||
                    (!string.IsNullOrEmpty(authorLastName) && b.Author.LastName.StartsWith(authorLastName))
                )
                .ToListAsync();
            foreach(Book book in books)
            {
                book.BorrowCount = await CountBookBorrowsAsync(book.UniqueID);
            }
            return books;
        }

        public async Task<Book?> GetBookByIdAsync(string id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.UniqueID.Equals(id));
        }

        public async Task<int> CountBookBorrowsAsync(string bookUniqueId)
        {
            return await _context.BookHistories
                .CountAsync(bh => bh.BookUniqueID == bookUniqueId && bh.Action == "Borrow");
        }
    }
}
