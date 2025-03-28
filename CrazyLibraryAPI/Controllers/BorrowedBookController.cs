using CrazyLibraryAPI.Models;
using CrazyLibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrazyLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowedBookController : ControllerBase
    {
        private readonly BorrowService _borrowService;
        public BorrowedBookController(BorrowService borrowService)
        {
            _borrowService = borrowService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<BorrowedBookModel>>> Search(string identity)
        {
            if (string.IsNullOrEmpty(identity))
            {
                return BadRequest("Identity cannot be empty");
            }
            var borrowedBooks = await _borrowService.SearchBurrowedBooksAsync(identity);
            if (borrowedBooks == null || !borrowedBooks.Any())
            {
                return NotFound("No borrowed books found for the customer");
            }
            return Ok(borrowedBooks);
        }

        [HttpPost("borrow")]
        public async Task<ActionResult> Borrow(string bookUniqueID, string customerPassport)
        {
            if (string.IsNullOrEmpty(bookUniqueID) || string.IsNullOrEmpty(customerPassport))
            {
                return BadRequest("BookUniqueID and CustomerPassport cannot be empty");
            }
            var result = await _borrowService.BorrowBookAsync(bookUniqueID, customerPassport);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok("Book borrowed successfully");
        }

        [HttpPost("return")]
        public async Task<ActionResult> Return(string bookUniqueID, string customerPassport)
        {
            if (string.IsNullOrEmpty(bookUniqueID) || string.IsNullOrEmpty(customerPassport))
            {
                return BadRequest("BookUniqueID and CustomerPassport cannot be empty");
            }
            var result = await _borrowService.ReturnBookAsync(bookUniqueID, customerPassport);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok("Book returned successfully");
        }
    }
}
