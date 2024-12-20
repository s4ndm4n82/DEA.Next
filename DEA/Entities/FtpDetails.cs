using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

public class FtpDetails
{
    public Guid Id { get; set; }
    [MaxLength(50)]
    public required string FtpType { get; init; }
    [MaxLength(50)]
    public required string FtpProfile { get; init; }
    [MaxLength(50)]
    public required string FtpHost { get; init; }
    [MaxLength(50)]
    public required string FtpUser { get; init; }
    [MaxLength(50)]
    public required string FtpPassword { get; init; }
    public int FtpPort { get; init; }
    public required bool FtpFolderLoop { get; init; }
    public required bool FtpMoveToSubFolder { get; init; }
    [MaxLength(200)]
    public required string FtpMainFolder { get; init; }
    [MaxLength(200)]
    public required string FtpSubFolder { get; init; }
    public required bool FtpRemoveFiles { get; init; }
    
    // Navigation properties
    public Guid CustomerDetailsId { get; set; }
    public CustomerDetails CustomerDetails { get; init; } = null!;
}