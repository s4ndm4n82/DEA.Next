namespace DEA.Next.Entities;

public class FtpDetails
{
    public Guid Id { get; set; }
    public required string FtpType { get; set; }
    public required string FtpProfile { get; set; }
    public required string FtpHost { get; set; }
    public required string FtpUser { get; set; }
    public required string FtpPassword { get; set; }
    public required string FtpPort { get; set; }
    public required string FtpPath { get; set; }
    public required bool FtpFolderLoop { get; set; }
    
    // Navigation properties
    public Guid CustomerDetailsId { get; set; }
    public CustomerDetails CustomerDetails { get; set; } = null!;
}