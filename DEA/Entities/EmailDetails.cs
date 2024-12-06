using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

public class EmailDetails
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public required string Email { get; set; }
    [MaxLength(100)]
    public required string EmailInboxPath { get; set; }
    
    // Navigation properties
    public Guid CustomerDetailsId { get; set; }
    public CustomerDetails CustomerDetails { get; set; } = null!;
}