using CrazyLibraryAPI.Models;
using CrazyLibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrazyLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookSearchController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookSearchController(BookService bookService)
        {
            _bookService = bookService;
        }
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> Search(
            [FromBody] BookSearchModel searchModel)
        {
            if (searchModel == null ||
                (string.IsNullOrEmpty(searchModel.BookName) &&
                string.IsNullOrEmpty(searchModel.AuthorFirstName) &&
                string.IsNullOrEmpty(searchModel.AuthorLastName)))
            {
                return BadRequest("At least one search parameter is required.");
            }

            var books = await _bookService.SearchBooksAsync(
                searchModel.BookName,
                searchModel.AuthorFirstName,
                searchModel.AuthorLastName);

            if (books == null || !books.Any())
            {
                return NotFound("No books match the search criteria.");
            }

            return Ok(books);
        }

        [HttpGet("{id}/stats")]
        public async Task<ActionResult<BookStatModel>> GetBookStats(string id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var borrowCount = await _bookService.CountBookBorrowsAsync(id);
            return new BookStatModel
            {
                Book = book,
                BorrowCount = borrowCount
            };
        }
    }
}
