using System.ComponentModel.DataAnnotations;

namespace DEA.Next.Entities;

/// <summary>
///     Represents the FTP details associated with a customer.
/// </summary>
public class FtpDetails
{
    /// <summary>
    ///     Gets or sets the unique identifier for the FTP details.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Gets or sets the type of the FTP.
    /// </summary>
    [MaxLength(50)]
    public required string FtpType { get; init; }

    /// <summary>
    ///     Gets or sets the profile of the FTP.
    /// </summary>
    [MaxLength(50)]
    public required string FtpProfile { get; init; }

    /// <summary>
    ///     Gets or sets the host of the FTP.
    /// </summary>
    [MaxLength(50)]
    public required string FtpHost { get; init; }

    /// <summary>
    ///     Gets or sets the user of the FTP.
    /// </summary>
    [MaxLength(50)]
    public required string FtpUser { get; init; }

    /// <summary>
    ///     Gets or sets the password of the FTP.
    /// </summary>
    [MaxLength(250)]
    public required string FtpPassword { get; init; }

    /// <summary>
    ///     Gets or sets the port of the FTP.
    /// </summary>
    public required int FtpPort { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether the FTP folder loop is enabled.
    /// </summary>
    public required bool FtpFolderLoop { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether to move files to a subfolder.
    /// </summary>
    public required bool FtpMoveToSubFolder { get; init; }

    /// <summary>
    ///     Gets or sets the main folder of the FTP.
    /// </summary>
    [MaxLength(200)]
    public required string FtpMainFolder { get; init; }

    /// <summary>
    ///     Gets or sets the subfolder of the FTP.
    /// </summary>
    [MaxLength(200)]
    public required string FtpSubFolder { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether to remove files from the FTP.
    /// </summary>
    public required bool FtpRemoveFiles { get; init; }

    // Navigation properties

    /// <summary>
    ///     Gets or sets the unique identifier of the associated customer details.
    /// </summary>
    public Guid CustomerDetailsId { get; init; }

    /// <summary>
    ///     Gets or sets the customer details associated with the FTP details.
    /// </summary>
    public CustomerDetails CustomerDetails { get; init; } = null!;
}