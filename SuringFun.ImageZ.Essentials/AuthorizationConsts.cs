namespace SuringFun.ImageZ.Essentials;

/// <summary>
/// Represents collection of constants that are used both by 
/// `Authorization` and `Service` microservices.
/// </summary>
public static class AuthorizationConsts
{
    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    public const string ClaimEmail
        = "e-mail";

    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    [Obsolete("Deprecated: Not used!")]
    public const string ClaimLogin
        = "login";

    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    public const string ClaimPublicName
        = "public-name";

    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    public const string ClaimAvatar
        = "avatar-url";

    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    public const string ClaimProviderName
        = "provider-name";

    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    public const string ClaimProviderKey
        = "provider-key";

    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    public const string ClaimProviderDisplay
        = "provider-display";

}
