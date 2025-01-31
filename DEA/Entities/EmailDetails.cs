using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

public class EmailDetails
{
    public Guid Id { get; init; }

    [MaxLength(50)] public required string Email { get; init; }

    [MaxLength(100)] public required string EmailInboxPath { get; init; }

    public bool SendEmail { get; init; }
    public bool SendSubject { get; init; }

    // Navigation properties
    public Guid CustomerDetailsId { get; init; }
    public CustomerDetails CustomerDetails { get; init; } = null!;
}