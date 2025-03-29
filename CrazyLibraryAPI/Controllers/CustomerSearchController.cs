using Microsoft.AspNetCore.Mvc;
using CrazyLibraryAPI.Models;
using CrazyLibraryAPI.Services;

namespace CrazyLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerSearchController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerSearchController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Customer>>> Search(
            [FromBody] CustomerSearchModel searchModel)
        {
            // Validate that at least one search parameter is provided
            if (searchModel == null ||
                (string.IsNullOrEmpty(searchModel.FirstName) &&
                string.IsNullOrEmpty(searchModel.LastName) &&
                string.IsNullOrEmpty(searchModel.Phone) &&
                string.IsNullOrEmpty(searchModel.Passport)))
            {
                return BadRequest("At least one search parameter is required.");
            }

            var customers = await _customerService.SearchCustomersAsync(
                searchModel.FirstName,
                searchModel.LastName,
                searchModel.Phone,
                searchModel.Passport);

            if (customers == null || !customers.Any())
            {
                return NotFound("No customers match the search criteria.");
            }

            return Ok(customers);
        }
    }
}