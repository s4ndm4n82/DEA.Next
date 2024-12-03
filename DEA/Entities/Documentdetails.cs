using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

public class Documentdetails
{
    public Guid Id { get; set; }
    [MaxLength(5)]
    public required string Extension { get; set; }
    
    // Navigation properties
    public Guid CustomerDetailsId { get; set; }
    public CustomerDetails CustomerDetails { get; set; } = null!;
}