using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

/// <summary>
///     Represents the details of a customer.
/// </summary>
public class CustomerDetails
{
    /// <summary>
    ///     Gets or sets the unique identifier for the customer.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the customer is active or inactive.
    ///     True = Active, False = Inactive.
    /// </summary>
    public required bool Status { get; init; } // True = Active, False = Inactive Used to be known as CustomerStatus.

    /// <summary>
    ///     Gets the name of the customer.
    /// </summary>
    [MaxLength(100)]
    public required string CustomerName { get; init; } // Used to be known as MainCustomerName.

    /// <summary>
    ///     Gets the username associated with the customer.
    /// </summary>
    [MaxLength(100)]
    public required string UserName { get; init; }

    /// <summary>
    ///     Gets the token associated with the customer.
    /// </summary>
    [MaxLength(1000)]
    public required string Token { get; init; }

    /// <summary>
    ///     Gets the queue number associated with the customer.
    /// </summary>
    public required int Queue { get; init; }

    /// <summary>
    ///     Gets the project ID associated with the customer.
    /// </summary>
    [MaxLength(50)]
    public string ProjectId { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the template key associated with the customer.
    /// </summary>
    [MaxLength(100)]
    public string TemplateKey { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the document ID associated with the customer.
    /// </summary>
    [MaxLength(50)]
    public string DocumentId { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the document encoding associated with the customer.
    /// </summary>
    [MaxLength(20)]
    public string DocumentEncoding { get; init; } = "UTF-8";

    /// <summary>
    ///     Gets the maximum batch size for the customer.
    /// </summary>
    public required int MaxBatchSize { get; init; } = 1;

    /// <summary>
    ///     Gets the value of the first custom field for the customer.
    /// </summary>
    [MaxLength(100)]
    public string FieldOneValue { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the name of the first custom field for the customer.
    /// </summary>
    [MaxLength(100)]
    public string FieldOneName { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the value of the second custom field for the customer.
    /// </summary>
    [MaxLength(100)]
    public string FieldTwoValue { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the name of the second custom field for the customer.
    /// </summary>
    [MaxLength(100)]
    public string FieldTwoName { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the domain associated with the customer.
    /// </summary>
    [MaxLength(100)]
    public required string Domain { get; init; }

    /// <summary>
    ///     Gets the file delivery method for the customer.
    /// </summary>
    [MaxLength(20)]
    public required string FileDeliveryMethod { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the list of document details associated with the customer.
    /// </summary>
    public required List<DocumentDetails> DocumentDetails { get; set; } = [];

    /// <summary>
    ///     Gets the FTP details associated with the customer, if any.
    /// </summary>
    public FtpDetails? FtpDetails { get; init; }

    /// <summary>
    ///     Gets the email details associated with the customer, if any.
    /// </summary>
    public EmailDetails? EmailDetails { get; init; }

    /// <summary>
    ///     Gets the date and time when the customer was created.
    /// </summary>
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    ///     Gets the date and time when the customer was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; init; } = DateTime.UtcNow;
}