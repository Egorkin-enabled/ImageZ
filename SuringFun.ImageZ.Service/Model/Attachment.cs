namespace SuringFun.ImageZ.Service.Model;

/// <summary>
/// Represents any file uploaded to th system.
/// </summary>
public class Attachment
{
    /// <summary>
    /// Primary Key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Attachment description in MarkDown format.
    /// </summary>
    /// <remarks>
    /// Uses `# ` header as display name. Should be first line,
    /// otherwise `Attachment` is unnamed.
    /// </remarks>
    public string? Description { get; set; }

    /// <summary>
    /// S3 key of content to find with `IFileService`.
    /// </summary>
    public string ContentKey { get; set; } = default!;

    /// <summary>
    /// S3 key of ligthweight preview to find with 
    /// `IFileService`.
    /// </summary>
    public string? PreviewKey { get; set; }
}

