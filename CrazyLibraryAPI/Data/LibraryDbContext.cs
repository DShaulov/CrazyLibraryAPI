using CrazyLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CrazyLibraryAPI.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookHistory> BookHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Passport);

            modelBuilder.Entity<Book>()
                .HasKey(b => b.UniqueID);

            modelBuilder.Entity<Author>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<BookHistory>()
                .HasKey(bh => bh.Id);

            // Configure BookHistory-Book relationship
            modelBuilder.Entity<BookHistory>()
                .HasOne(bh => bh.Book)
                .WithMany()
                .HasForeignKey(bh => bh.BookUniqueID);

            // Configure BookHistory-Customer relationship
            modelBuilder.Entity<BookHistory>()
                .HasOne(bh => bh.Customer)
                .WithMany()
                .HasForeignKey(bh => bh.CustomerPassport);

            // Add index to improve query performance for common scenarios
            modelBuilder.Entity<BookHistory>()
                .HasIndex(bh => bh.BookUniqueID);

            modelBuilder.Entity<BookHistory>()
                .HasIndex(bh => bh.CustomerPassport);

            modelBuilder.Entity<BookHistory>()
                .HasIndex(bh => bh.DateTime);

            modelBuilder.Entity<BookHistory>()
                .HasIndex(bh => bh.Action);
        }
    }
}
