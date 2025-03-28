using CrazyLibraryAPI.Data;
using CrazyLibraryAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrazyLibraryAPI.Services
{
    public class BorrowService
    {
        private readonly LibraryDbContext _context;
        
        public BorrowService(LibraryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Performs the following:
        ///     1. Gets all history related to a customer
        ///     2. Groups the history by bookId
        ///     3. Sorts each history group by date time
        ///     4. Checks if the most recent action is "Borrow" and if it is the book is added to the list.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BorrowedBookModel>> SearchBurrowedBooksAsync(string identity)
        {
            var allCustomerBookHistory = _context.BookHistories
                .Where(bh => bh.CustomerPassport == identity)
                .Include(bh => bh.Book)
                .ThenInclude(b => b.Author) // Nested include - include the books author
                .ToList();

            var bookHistoryByBookId = allCustomerBookHistory.GroupBy(history => history.BookUniqueID);
            var currentlyBorrowedBooks = new List<BorrowedBookModel>();
            foreach (var bookGroup in bookHistoryByBookId)
            {
                // For each book, get all actions sorted by date
                var bookActions = bookGroup.OrderByDescending(history => history.DateTime).ToList();

                // Check if the most recent action is "Borrow"
                var mostRecentAction = bookActions.First();
                bool isCurrentlyBorrowed = mostRecentAction.Action == "Borrow";

                if (isCurrentlyBorrowed)
                {
                    string authorName = "";
                    if (mostRecentAction.Book.Author != null)
                    {
                        authorName = mostRecentAction.Book.Author.FirstName + mostRecentAction.Book.Author.LastName;
                    }
                    currentlyBorrowedBooks.Add(new BorrowedBookModel
                    {
                        BookUID = mostRecentAction.BookUniqueID,
                        Author = authorName,
                        BookName = mostRecentAction.Book.Title,
                        BorrowedOn = mostRecentAction.DateTime
                    });
                }
            }

            return currentlyBorrowedBooks;
        }

        public async Task<(bool Success, string Message)> BorrowBookAsync(string bookUniqueID, string customerPassport)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.UniqueID == bookUniqueID);

            if (book == null)
            {
                return (false, "Book not found.");
            }

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Passport == customerPassport);

            if (!customerExists)
            {
                return (false, "Customer not found.");
            }

            if (book.CopiesAvailable <= 0)
            {
                return (false, "No copies available for borrowing.");
            }

            book.CopiesAvailable--;

            // Create book history table entry
            var bookHistory = new BookHistory
            {
                BookUniqueID = bookUniqueID,
                CustomerPassport = customerPassport,
                DateTime = DateTime.UtcNow,
                Action = "Borrow"
            };

            _context.BookHistories.Add(bookHistory);

            await _context.SaveChangesAsync();

            return (true, "Book borrowed successfully.");
        }


        public async Task<(bool Success, string Message)> ReturnBookAsync(string bookUniqueID, string customerPassport)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.UniqueID == bookUniqueID);

            if (book == null)
            {
                return (false, "Book not found.");
            }

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Passport == customerPassport);

            if (!customerExists)
            {
                return (false, "Customer not found.");
            }

            // Check if the customer has actually borrowed this book
            var latestHistory = await _context.BookHistories
                .Where(bh => bh.BookUniqueID == bookUniqueID && bh.CustomerPassport == customerPassport)
                .OrderByDescending(bh => bh.DateTime)
                .FirstOrDefaultAsync();

            if (latestHistory == null || latestHistory.Action != "Borrow")
            {
                return (false, "This book is not currently borrowed by this customer.");
            }

            book.CopiesAvailable++;

            // Create book history entry for return
            var bookHistory = new BookHistory
            {
                BookUniqueID = bookUniqueID,
                CustomerPassport = customerPassport,
                DateTime = DateTime.UtcNow,
                Action = "Return"
            };

            _context.BookHistories.Add(bookHistory);

            await _context.SaveChangesAsync();

            return (true, "Book returned successfully.");
        }
    }
}
