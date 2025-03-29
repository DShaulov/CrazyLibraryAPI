using CrazyLibraryAPI.Data;
using CrazyLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrazyLibraryAPI.Services
{
    public class CustomerService
    {
        private readonly LibraryDbContext _context;

        public CustomerService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string firstName, string lastName, string phone, string identity)
        {
            return await _context.Customers
                .Where(c =>
                    (!string.IsNullOrEmpty(firstName) && c.FirstName.StartsWith(firstName)) ||
                    (!string.IsNullOrEmpty(lastName) && c.LastName.StartsWith(lastName)) ||
                    (!string.IsNullOrEmpty(phone) && c.PhoneNumber.StartsWith(phone)) ||
                    (!string.IsNullOrEmpty(identity) && c.Passport.StartsWith(identity))
                )
                .ToListAsync();
        }
    }
}