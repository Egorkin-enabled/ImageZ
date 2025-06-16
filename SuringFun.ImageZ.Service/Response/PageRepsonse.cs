namespace SuringFun.ImageZ.Service.Response;

/// <summary>
/// Represents generalized response for page. 
/// </summary>
/// <typeparam name="TItem">List of sub-responses.</typeparam>
public class PageRepsonse<TItem>
{
    /// <summary>
    /// How much items could be loaded at once.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// How much items to skip in the list.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Items loaded into page.
    /// </summary>
    /// <remarks>
    /// Note: `Items.Length` may be less than `Limit`.
    /// </remarks>
    public TItem[] Items { get; set; } = default!;
}

