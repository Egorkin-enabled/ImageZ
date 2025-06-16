namespace SuringFun.ImageZ.Service.Model;

/// <summary>
/// Represents author's emotion to other author or publication.
/// </summary>
public class Emotion
{
    /// <summary>
    /// All possible emotions related to a target in the system.
    /// </summary>
    public enum EmotionKind
    {
        /// <summary>
        /// Agent likes target.
        /// </summary>
        Positive = +1,

        /// <summary>
        /// Agent dislikes target.
        /// </summary>
        Negative = -1
    }

    /// <summary>
    /// Represents target kind.
    /// </summary>
    public enum TargetKind
    {
        Author,
        Publication
    }

    public int Id { get; set; }

    #region UNIQUE COMBINATION

    /// <summary>
    /// Represents source of the emotion.
    /// </summary>
    public Author Source { get; set; } = default!;

    #region ONE OUT OF TWO

    /// <summary>
    /// Represents target of the emotion as if it would be 
    /// another author.
    /// </summary>
    public Author? AuthorTarget { get; set; }

    /// <summary>
    /// Represents target of the emotion as if it would be a
    /// publication.
    /// </summary>
    public Publication? PublicationTarget { get; set; }

    #endregion // ONE OUT OF TWO

    #endregion // UNIQUE COMBINATION

    /// <summary>
    /// Rerpesents kind of current relation.
    /// </summary>
    public EmotionKind Kind { get; set; }
}

