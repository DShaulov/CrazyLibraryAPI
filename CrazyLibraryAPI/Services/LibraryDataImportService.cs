using CrazyLibraryAPI.Data;
using CrazyLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CrazyLibraryAPI.Services
{
    public class LibraryDataImportService
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<LibraryDataImportService> _logger;

        public LibraryDataImportService(LibraryDbContext context, ILogger<LibraryDataImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ImportFromJsonFile(string filePath)
        {
            try
            {
                // Read the JSON file
                string jsonContent = await File.ReadAllTextAsync(filePath);

                // Deserialize the JSON content
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    foreach (JsonElement element in doc.RootElement.EnumerateArray())
                    {
                        await ProcessJsonEntry(element);
                    }
                }

                // Save all changes to the database
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully imported data from {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing data from {filePath}");
                throw;
            }
        }
        private async Task ProcessJsonEntry(JsonElement element)
        {
            try
            {
                // Extract data from the JSON element
                var bookElement = element.GetProperty("Book");
                var customerElement = element.GetProperty("Customer");
                string actionType = element.GetProperty("Type").GetString();
                DateTime actionDateTime = element.GetProperty("ActionDateTime").GetDateTime();

                // Process Book data
                var literaryCreation = bookElement.GetProperty("LiteraryCreation");
                string bookUniqueId = literaryCreation.GetProperty("UniqueID").GetString();
                string title = literaryCreation.GetProperty("Title").GetString();
                string description = literaryCreation.GetProperty("Description").GetString();
                DateTime publicationDate = bookElement.GetProperty("PublicationDate").GetDateTime();
                string imageUrl = bookElement.GetProperty("ImageUrl").GetString();
                string callNumber = bookElement.GetProperty("LibraryCallNumber").GetString();

                // Process Customer data
                string passport = customerElement.GetProperty("Passport").GetString();
                string firstName = customerElement.GetProperty("FirstName").GetString();
                string lastName = customerElement.GetProperty("LastName").GetString();
                string address = customerElement.GetProperty("Address").GetString();
                string city = customerElement.GetProperty("City").GetString();
                string email = customerElement.GetProperty("Email").GetString();
                string phoneNumber = customerElement.GetProperty("PhoneNumber").GetString();
                string zip = customerElement.GetProperty("Zip").GetString();
                DateTime birthDate = customerElement.GetProperty("BirthDate").GetDateTime();

                // Process author data
                Author author = null;
                if (literaryCreation.TryGetProperty("Author", out var authorElement) &&
                    authorElement.TryGetProperty("Name", out var authorNameElement) &&
                    authorNameElement.ValueKind != JsonValueKind.Null)
                {
                    string authorName = authorNameElement.GetString();
                    string authFirstName = null;
                    string authLastName = null;

                    // Split author name into first and last name
                    if (!string.IsNullOrEmpty(authorName))
                    {
                        var nameParts = authorName.Split(' ', 2);
                        authFirstName = nameParts[0];
                        authLastName = nameParts.Length > 1 ? nameParts[1] : "";
                        author = await GetOrCreateAuthor(authFirstName, authLastName);
                    }
                    else
                    {
                        // Create an author with unknown name
                        author = await GetOrCreateAuthor("Unknown", "");

                    }
                }

                // Get or create the book
                var book = await GetOrCreateBook(bookUniqueId, title, description, author.Id,
                    publicationDate, imageUrl, callNumber);

                // Get or create the customer
                var customer = await GetOrCreateCustomer(passport, firstName, lastName, address,
                    city, email, phoneNumber, zip, birthDate);

                // Get or create the book history entry
                await GetOrCreateBookHistory(bookUniqueId, passport, actionDateTime, actionType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing JSON entry");
                throw;
            }
        }

        private async Task<Author> GetOrCreateAuthor(string firstName, string lastName)
        {
            // Look for existing author with the same name
            var existingAuthor = await _context.Authors
                .FindAsync(firstName, lastName);

            if (existingAuthor != null)
                return existingAuthor;

            // Create a new author
            var author = new Author
            {
                FirstName = firstName,
                LastName = lastName
            };

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            return author;
        }

        private async Task<Customer> GetOrCreateCustomer(string passport, string firstName,
            string lastName, string address, string city, string email, string phoneNumber,
            string zip, DateTime birthDate)
        {
            // Look for existing customer
            var existingCustomer = await _context.Customers.FindAsync(passport);

            if (existingCustomer != null)
                return existingCustomer;

            // Create a new customer
            var customer = new Customer
            {
                Passport = passport,
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                City = city,
                Email = email,
                PhoneNumber = phoneNumber,
                Zip = zip,
                BirthDate = birthDate
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        private async Task<Book> GetOrCreateBook(string uniqueId, string title, string description,
            int authorId, DateTime publicationDate, string imageUrl, string callNumber)
        {
            // Look for existing book
            var existingBook = await _context.Books.FindAsync(uniqueId);

            if (existingBook != null)
                return existingBook;

            // Create a new book
            var book = new Book
            {
                UniqueID = uniqueId,
                Title = title,
                Description = description,
                AuthorId = authorId,
                PublicationDate = publicationDate,
                Image_url = imageUrl,
                LibraryCallNumber = callNumber
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return book;
        }

        private async Task<BookHistory> GetOrCreateBookHistory(string bookUniqueId, string customerPassport,
            DateTime actionDateTime, string actionType)
        {
            // Check if this exact book history entry already exists
            var existingHistory = await _context.BookHistories
                .FirstOrDefaultAsync(bh =>
                    bh.BookUniqueID == bookUniqueId &&
                    bh.CustomerPassport == customerPassport &&
                    bh.DateTime == actionDateTime &&
                    bh.Action == actionType);

            if (existingHistory != null)
            {
                _logger.LogInformation($"Found existing book history: Book {bookUniqueId}, Customer {customerPassport}, Action {actionType}");
                return existingHistory;
            }

            // Create a new book history entry
            var bookHistory = new BookHistory
            {
                BookUniqueID = bookUniqueId,
                CustomerPassport = customerPassport,
                DateTime = actionDateTime,
                Action = actionType
            };

            _logger.LogInformation($"Creating new book history: Book {bookUniqueId}, Customer {customerPassport}, Action {actionType}");
            await _context.BookHistories.AddAsync(bookHistory);

            return bookHistory;
        }
    }
}
