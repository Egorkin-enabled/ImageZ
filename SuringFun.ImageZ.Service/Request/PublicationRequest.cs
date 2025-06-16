using System.ComponentModel.DataAnnotations;

namespace SuringFun.ImageZ.Service.Request;

/// <summary>
/// Represents body for `POST /publications`. 
/// </summary>
public class PublicationRequest
{
    /// <summary>
    /// Byte data of content.
    /// </summary>
    [Required]
    public byte[] Content { get; set; } = default!;

    /// <summary>
    /// Byte data of preview.
    /// </summary>
    public byte[]? Preview { get; set; }

    /// <summary>
    /// Description in `MarkDown` format.
    /// </summary>
    /// <remarks>
    /// Extracts name of publication from `# ` at the beggining
    /// of the file. If not found, publication is unnamed.
    /// </remarks>
    public string? Description { get; set; }
}
