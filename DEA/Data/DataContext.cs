using DEA.Next.Entities;
using DEA.Next.Extensions;
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
            .HasKey(cd => cd.Id);
        
        modelBuilder.Entity<CustomerDetails>()
            .Property(cd => cd.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<CustomerDetails>()
            .HasOne(cd => cd.FtpDetails)
            .WithOne(fd => fd.CustomerDetails)
            .HasForeignKey<FtpDetails>(fd => fd.CustomerDetailsId);
        
        modelBuilder.Entity<CustomerDetails>()
            .HasOne(cd => cd.EmailDetails)
            .WithOne(ed => ed.CustomerDetails)
            .HasForeignKey<EmailDetails>(ed => ed.CustomerDetailsId);
        
        // Seeding data to the database.
        if (!File.Exists("./Config/CustomerConfig.json")) return;

        var customerDetails = ReadDataFromJson.ReadDataFromJsonConfig().Result;

        foreach (var customer in customerDetails)
        {
            customer.Id = Guid.NewGuid();
            modelBuilder.Entity<CustomerDetails>().HasData(new CustomerDetails
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                UserName = customer.UserName,
                Token = customer.Token,
                Queue = customer.Queue,
                ProjectId = customer.ProjectId,
                TemplateKey = customer.TemplateKey,
                DocumentId = customer.DocumentId,
                DocumentEncoding = customer.DocumentEncoding,
                MaxBatchSize = customer.MaxBatchSize,
                FieldOneValue = customer.FieldOneValue,
                FieldOneName = customer.FieldOneName,
                FieldTwoValue = customer.FieldTwoValue,
                FieldTwoName = customer.FieldTwoName,
                Domain = customer.Domain,
                FileDeliveryMethod = customer.FileDeliveryMethod,
                CreatedDate = customer.CreatedDate,
                ModifiedDate = customer.ModifiedDate
            });
            
            if (customer.FtpDetails != null)
            {
                customer.FtpDetails.Id = Guid.NewGuid();
                customer.FtpDetails.CustomerDetailsId = customer.Id;
                modelBuilder.Entity<FtpDetails>().HasData(customer.FtpDetails);
            }
            
            if (customer.EmailDetails != null)
            {
                customer.EmailDetails.Id = Guid.NewGuid();
                customer.EmailDetails.CustomerDetailsId = customer.Id;
                modelBuilder.Entity<EmailDetails>().HasData(customer.EmailDetails);
            }

            foreach (var document in customer.DocumentDetails)
            {
                document.Id = Guid.NewGuid();
                document.CustomerDetailsId = customer.Id;
                modelBuilder.Entity<DocumentDetails>().HasData(new DocumentDetails
                {
                    Id = document.Id,
                    Extension = document.Extension,
                    CustomerDetailsId = document.CustomerDetailsId
                });
            }
        }
    }
}