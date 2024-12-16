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
    public int FtpPort { get; set; }
    public required bool FtpFolderLoop { get; set; }
    public required bool FtpMoveToSubFolder { get; set; }
    [MaxLength(200)]
    public required string FtpMainFolder { get; set; }
    [MaxLength(200)]
    public required string FtpSubFolder { get; set; }
    public required bool FtpRemoveFiles { get; set; }
    
    // Navigation properties
    public Guid CustomerDetailsId { get; set; }
    public CustomerDetails CustomerDetails { get; set; } = null!;
}