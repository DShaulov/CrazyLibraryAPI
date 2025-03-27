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
            // Create an empty result set
            IQueryable<Book> result = _context.Books.Where(b => false);
            // Check if each search parameter is provided and add the query to the result set
            if (!string.IsNullOrEmpty(bookName))
            {
                IQueryable<Book> titleQuery = _context.Books
                    .Include(b => b.Author)
                    .Where(b => b.Title.StartsWith(bookName));
                result = result.Union(titleQuery);
            }
            if (!string.IsNullOrEmpty(authorFirstName))
            {
                IQueryable<Book> authorFirstNameQuery = _context.Books
                    .Include(b => b.Author)
                    .Where(b => b.Author.FirstName.StartsWith(authorFirstName));
                result = result.Union(authorFirstNameQuery);
            }
            if (!string.IsNullOrEmpty(authorLastName))
            {
                IQueryable<Book> authorLastNameQuery = _context.Books
                    .Include(b => b.Author)
                    .Where(b => b.Author.LastName.StartsWith(authorLastName));
                result = result.Union(authorLastNameQuery);
            }
            return await result.ToListAsync();
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
