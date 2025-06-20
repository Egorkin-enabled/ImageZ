using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace SuringFun.ImageZ.Authorization.Controller;

/// <summary>
/// Represents request to authorization.
/// </summary>
public class AuthorizeRequest
{
    // Currently dublicates register request, but it was made
    // to simplify differnece introducing in authorization &
    // registration process.

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}

/// <summary>
/// Represents request to registation.
/// </summary>
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    public byte[]? PhotoContents { get; set; }
}