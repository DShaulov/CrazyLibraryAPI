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
            // Start with all customers
            IQueryable<Customer> query = _context.Customers.Where(c => true);

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(c => c.FirstName.StartsWith(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(c => c.LastName.StartsWith(lastName));
            }

            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(c => c.PhoneNumber.StartsWith(phone));
            }

            if (!string.IsNullOrEmpty(identity))
            {
                query = query.Where(c => c.Passport.StartsWith(identity));
            }

            return await query.ToListAsync();
        }
    }
}