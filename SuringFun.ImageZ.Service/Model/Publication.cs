namespace SuringFun.ImageZ.Service.Model;

/// <summary>
/// Represents publication in the system.
/// </summary>
public class Publication
{
    /// <summary>
    /// Primary Key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Author of the publication. May be `null` if author was
    /// removed, for example.
    /// </summary>
    public Author? Author { get; set; }

    /// <summary>
    /// Content of the publication.
    /// </summary>
    public Attachment Attachment { get; set; } = default!;
}

