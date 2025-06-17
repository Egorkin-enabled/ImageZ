using System.ComponentModel.DataAnnotations;
using SuringFun.ImageZ.Service.Model;

namespace SuringFun.ImageZ.Service.Request;

/// <summary>
/// Represent body for `POST /relations`.
/// </summary>
public class EmotionRequest
{
    /// <summary>
    /// Represents kind of target.
    /// </summary>

    /// <summary>
    /// Id of the target.
    /// </summary>
    [Required]
    public int TargetId { get; set; }

    /// <summary>
    /// Determinates what TargetId is: 
    /// author's id or publication's one.
    /// </summary>
    [Required]
    public Emotion.TargetKind TargetKind { get; set; }

    /// <summary>
    /// Determinates which emotion to express.
    /// </summary>
    [Required]
    public Emotion.EmotionKind EmotionKind { get; set; }
}