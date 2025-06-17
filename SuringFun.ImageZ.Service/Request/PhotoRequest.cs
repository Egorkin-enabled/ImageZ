using System.ComponentModel.DataAnnotations;

namespace SuringFun.ImageZ.Service.Request;

/// <summary>
/// Represents photo request's post body. 
/// </summary>
public class PhotoRequest
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

}