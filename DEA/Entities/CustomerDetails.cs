using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

public class CustomerDetails
{
    public Guid Id { get; init; }
    public bool Status { get; init; } // True = Active, False = Inactive Used to be known as CustomerStatus.
    [MaxLength(100)]
    public required string CustomerName { get; init; } // Used to be known as MainCustomerName.
    [MaxLength(100)]
    public required string UserName { get; init; }
    [MaxLength(800)]
    public required string Token { get; init; }
    public int Queue { get; init; }
    [MaxLength(50)]
    public string ProjectId { get; init; } = string.Empty;
    [MaxLength(100)]
    public string TemplateKey { get; init; } = string.Empty;
    [MaxLength(50)]
    public string DocumentId { get; init; } = string.Empty;
    [MaxLength(10)]
    public string DocumentEncoding { get; init; } = "UTF-8";
    public int MaxBatchSize { get; init; } = 1;
    [MaxLength(100)]
    public string FieldOneValue { get; init; } = string.Empty;
    [MaxLength(100)]
    public string FieldOneName { get; init; } = string.Empty;
    [MaxLength(100)]
    public string FieldTwoValue { get; init; } = string.Empty;
    [MaxLength(100)]
    public string FieldTwoName { get; init; } = string.Empty;
    [MaxLength(100)]
    public required string Domain { get; init; }
    [MaxLength(10)]
    public string FileDeliveryMethod { get; init; } = string.Empty;
    public required List<DocumentDetails> DocumentDetails { get; init; } = [];
    public FtpDetails? FtpDetails { get; init; } 
    public EmailDetails? EmailDetails { get; init; } 
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; init; } = DateTime.UtcNow;
}