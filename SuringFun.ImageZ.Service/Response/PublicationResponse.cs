namespace SuringFun.ImageZ.Service.Response;

/// <summary>
/// Represents response for publications.
/// </summary>
public class PublicationResponse
{
    /// <summary>
    /// Identifer of the publication.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Author, who created publication.
    /// </summary>
    public AuthorResponse? Author { get; set; }

    /// <summary>
    /// Attachment, which represents publication.
    /// </summary>
    public AttachmentResponse Attachment { get; set; }
        = default!;
}

