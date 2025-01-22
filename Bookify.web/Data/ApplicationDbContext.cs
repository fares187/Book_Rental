using Bookify.web.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(e => e.CreatedOn)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Book> Books { get; set; }  
        public DbSet<BookCopy> BookCopies { get; set; }  
        public DbSet<BookCategory> BooksCategories { get; set; } 
        public DbSet<Category> Categories { get; set; }   
        public DbSet<Governorate> Governorates { get; set; }   
        public DbSet<Rental> Rentals { get; set; }   
        public DbSet<RentalCopy> RentalCopies { get; set; }   
        public DbSet<Subscriper> Subscripers { get; set; }   
        public DbSet<Subscription> Subscriptions  { get; set; }   

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasSequence<int>("SerialNumber",schema:"shared")
                .StartsAt(1000001);
            builder.Entity<BookCopy>()
                .Property(e => e.SerialNumber)
                .HasDefaultValueSql("nextval('\"shared\".\"SerialNumber\"') ") ;

            var cascadefor = builder.Model.GetEntityTypes()
                .SelectMany(t=>t.GetForeignKeys())
                .Where(t=>t.DeleteBehavior== DeleteBehavior.Cascade && !t.IsOwnership);

            foreach(var fk in cascadefor)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Entity<BookCategory>().HasKey(e => new
            {
                e.BookId,
                e.CategoryId
            });
            builder.Entity<RentalCopy>().HasKey(e => new
            {
                e.RentalId,
                e.CopyBookId
            });

            builder.Entity<Rental>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<RentalCopy>().HasQueryFilter(e => !e.Rental!.IsDeleted);
            base.OnModelCreating(builder);
        }
    }
}