namespace SuringFun.ImageZ.Service.Response;

/// <summary>
/// Represents attachment response.
/// </summary>
public class AttachmentResponse
{
    /// <summary>
    /// S3 key for the actual content.
    /// </summary>
    public string ContentKey { get; set; } = default!;

    /// <summary>
    /// S3 key for simplified preview content. May be missing.
    /// </summary>
    public string? PreviewKey { get; set; }

    /// <summary>
    /// Description of the attachment.
    /// </summary>
    public string? Description { get; set; }
}

