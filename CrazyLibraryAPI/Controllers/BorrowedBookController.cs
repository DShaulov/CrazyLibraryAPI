﻿using CrazyLibraryAPI.Models;
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

        [HttpPost]
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
    }
}
