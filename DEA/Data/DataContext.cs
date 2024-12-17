using DEA.Next.Entities;
using Microsoft.EntityFrameworkCore;

namespace DEA.Next.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<CustomerDetails> CustomerDetails { get; init; }
    public DbSet<FtpDetails> FtpDetails { get; init; }
    public DbSet<EmailDetails> EmailDetails { get; init; }
    
    // Building the relationships between tables.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerDetails>()
            .HasOne(cd => cd.FtpDetails)
            .WithOne(fd => fd.CustomerDetails)
            .HasForeignKey<FtpDetails>(fd => fd.CustomerDetailsId);

        modelBuilder.Entity<CustomerDetails>()
            .HasOne(cd => cd.EmailDetails)
            .WithOne(ed => ed.CustomerDetails)
            .HasForeignKey<EmailDetails>(ed => ed.CustomerDetailsId);
    }
}