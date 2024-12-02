namespace DEA.Next.Entities;

public class Emaildetails
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string EmailInboxPath { get; set; }
    
    // Navigation properties
    public Guid CustomerDetailsId { get; set; }
    public CustomerDetails CustomerDetails { get; set; } = null!;
}