using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

public class FtpDetails
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public required string FtpType { get; set; }
    [MaxLength(50)]
    public required string FtpProfile { get; set; }
    [MaxLength(50)]
    public required string FtpHost { get; set; }
    [MaxLength(50)]
    public required string FtpUser { get; set; }
    [MaxLength(50)]
    public required string FtpPassword { get; set; }
    [MaxLength(2)]
    public required string FtpPort { get; set; }
    [MaxLength(100)]
    public required string FtpPath { get; set; }
    public required bool FtpFolderLoop { get; set; }
    
    // Navigation properties
    public Guid CustomerDetailsId { get; set; }
    public CustomerDetails CustomerDetails { get; set; } = null!;
}