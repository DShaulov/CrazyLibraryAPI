using CrazyLibraryAPI.Data;
using CrazyLibraryAPI.Models;
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
                .ToList();

            var bookHistoryByBookId = allCustomerBookHistory.GroupBy(history => history.BookUniqueID);
            var currentlyBorrowedBooks = new List<BorrowedBookModel>();
            foreach (var bookGroup in bookHistoryByBookId)
            {
                // For each book, get all actions sorted by date
                var bookActions = bookGroup.OrderByDescending(history => history.DateTime).ToList();

                // Check if the most recent action is "Borrowed"
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
    }
}
