using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

/// <summary>
///     Represents the details of a document associated with a customer.
/// </summary>
public class DocumentDetails
{
    /// <summary>
    ///     Gets or sets the unique identifier for the document.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the file extension of the document.
    /// </summary>
    [MaxLength(10)]
    public required string Extension { get; set; }

    // Navigation properties

    /// <summary>
    ///     Gets or sets the unique identifier of the associated customer details.
    /// </summary>
    public Guid CustomerDetailsId { get; set; }

    /// <summary>
    ///     Gets or sets the customer details associated with the document.
    /// </summary>
    public CustomerDetails CustomerDetails { get; set; } = null!;
}