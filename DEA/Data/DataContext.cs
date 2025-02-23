using DEA.Next.Entities;
using DEA.Next.Models;
using Microsoft.EntityFrameworkCore;

namespace DEA.Next.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    // DbSet for CustomerDetails entity
    public DbSet<CustomerDetails> CustomerDetails { get; init; }

    // DbSet for FtpDetails entity
    public DbSet<FtpDetails> FtpDetails { get; init; }

    // DbSet for EmailDetails entity
    public DbSet<EmailDetails> EmailDetails { get; init; }

    /// <summary>
    ///     Configures the relationships and properties of the entities.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the entities.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configuration for CustomerDetails entity
        modelBuilder.ApplyConfiguration(new CustomerDetailsModel());

        // Apply configuration for FtpDetails entity
        modelBuilder.ApplyConfiguration(new FtpDetailsModel());

        // Apply configuration for EmailDetails entity
        modelBuilder.ApplyConfiguration(new EmailDetailsModel());

        // Apply configuration for DocumentDetails entity
        modelBuilder.ApplyConfiguration(new DocumentDetailsModel());

        // Configure primary key for CustomerDetails
        modelBuilder.Entity<CustomerDetails>()
            .HasKey(cd => cd.Id);

        // Configure ID property to be generated on add for CustomerDetails
        modelBuilder.Entity<CustomerDetails>()
            .Property(cd => cd.Id)
            .ValueGeneratedOnAdd();

        // Configure one-to-one relationship between CustomerDetails and FtpDetails
        modelBuilder.Entity<CustomerDetails>()
            .HasOne(cd => cd.FtpDetails)
            .WithOne(fd => fd.CustomerDetails)
            .HasForeignKey<FtpDetails>(fd => fd.CustomerDetailsId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-one relationship between CustomerDetails and EmailDetails
        modelBuilder.Entity<CustomerDetails>()
            .HasOne(cd => cd.EmailDetails)
            .WithOne(ed => ed.CustomerDetails)
            .HasForeignKey<EmailDetails>(ed => ed.CustomerDetailsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}