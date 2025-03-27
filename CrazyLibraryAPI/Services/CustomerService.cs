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
            // Create an empty result set
            IQueryable<Customer> result = _context.Customers.Where(c => false);

            // Check if each search parameter is provided and add the query to the result set
            if (!string.IsNullOrEmpty(firstName))
            {
                IQueryable<Customer> firstNameQuery = _context.Customers.Where(c => c.FirstName.StartsWith(firstName));
                result = result.Union(firstNameQuery);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                IQueryable<Customer> lastNameQuery = _context.Customers.Where(c => c.LastName.StartsWith(lastName));
                result = result.Union(lastNameQuery);
            }

            if (!string.IsNullOrEmpty(phone))
            {
                IQueryable<Customer> phoneQuery = _context.Customers.Where(c => c.PhoneNumber.StartsWith(phone));
                result = result.Union(phoneQuery);
            }

            if (!string.IsNullOrEmpty(identity))
            {
                IQueryable<Customer> identityQuery = _context.Customers.Where(c => c.Passport.StartsWith(identity));
                result = result.Union(identityQuery);
            }

            return await result.ToListAsync();

        }
    }
}