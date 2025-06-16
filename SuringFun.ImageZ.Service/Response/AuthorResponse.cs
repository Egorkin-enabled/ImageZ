namespace SuringFun.ImageZ.Service.Response;

/// <summary>
/// Represents response for authors.
/// </summary>
public class AuthorResponse
{
    /// <summary>
    /// Identifer of the author.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Avatar photo. May be unspecified.
    /// </summary>
    public AttachmentResponse? Photo { get; set; }

    /// <summary>
    /// Public name, shown to everyone.
    /// </summary>
    public string PublicName { get; set; } = default!;
}

