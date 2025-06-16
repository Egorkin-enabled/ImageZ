using SuringFun.ImageZ.Service.Model;

namespace SuringFun.ImageZ.Service.Response;

/// <summary>
/// Represents response for emotions.
/// </summary>
public class EmotionResponse
{
    /// <summary>
    /// Author, who's express emotion.
    /// </summary>
    public AuthorResponse Source { get; set; } = default!;

    /// <summary>
    /// `Target` response object. Actual type of it determinated
    /// by `TargetKind` property.
    /// </summary>
    public object Target { get; set; } = default!;

    /// <summary>
    /// Determinates what TargetId is: 
    /// author's id or publication's one.
    /// </summary>
    public Emotion.TargetKind TargetKind { get; set; }

    /// <summary>
    /// Determinates which emotion is expressed.
    /// </summary>
    public Emotion.EmotionKind EmotionKind { get; set; }
}

