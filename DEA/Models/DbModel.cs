using DEA.Next.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DEA.Next.Models;

/// <summary>
///     Defines the configuration for the CustomerDetails entity.
/// </summary>
public class CustomerDetailsModel : IEntityTypeConfiguration<CustomerDetails>
{
    /// <summary>
    ///     Configures the CustomerDetails entity.
    /// </summary>
    /// <param name="builder">The EntityTypeBuilder instance for CustomerDetails.</param>
    public void Configure(EntityTypeBuilder<CustomerDetails> builder)
    {
        // Set the default value for the ID property to a generated UUID.
        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");
    }
}

/// <summary>
///     Defines the configuration for the FtpDetails entity.
/// </summary>
public class FtpDetailsModel : IEntityTypeConfiguration<FtpDetails>
{
    /// <summary>
    ///     Configures the FtpDetails entity.
    /// </summary>
    /// <param name="builder">The EntityTypeBuilder instance for FtpDetails.</param>
    public void Configure(EntityTypeBuilder<FtpDetails> builder)
    {
        // Set the default value for the ID property to a generated UUID.
        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");
    }
}

/// <summary>
///     Defines the configuration for the EmailDetails entity.
/// </summary>
public class EmailDetailsModel : IEntityTypeConfiguration<EmailDetails>
{
    /// <summary>
    ///     Configures the EmailDetails entity.
    /// </summary>
    /// <param name="builder">The EntityTypeBuilder instance for EmailDetails.</param>
    public void Configure(EntityTypeBuilder<EmailDetails> builder)
    {
        // Set the default value for the ID property to a generated UUID.
        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");
    }
}

/// <summary>
///     Defines the configuration for the DocumentDetails entity.
/// </summary>
public class DocumentDetailsModel : IEntityTypeConfiguration<DocumentDetails>
{
    /// <summary>
    ///     Configures the DocumentDetails entity.
    /// </summary>
    /// <param name="builder">The EntityTypeBuilder instance for DocumentDetails.</param>
    public void Configure(EntityTypeBuilder<DocumentDetails> builder)
    {
        // Set the default value for the ID property to a generated UUID.
        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");
    }
}