using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuringFun.ImageZ.Service.Model;
using SuringFun.ImageZ.Service.Response;

namespace SuringFun.ImageZ.Service.Controller;

/// <summary>
/// Controls authorization with standardized JWT received from
/// `SuringFun.ImageZ.Authorization` microservice.
/// </summary>
[Route("/authors")]
public class AuthorizationController :
    Microsoft.AspNetCore.Mvc.Controller
{
    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
    public const string ClaimEmail
        = "e-mail";

    /// <summary>
    /// Email claim in `Authorization` JWT token.
    /// </summary>
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


    /// <summary>
    /// Uses JWT token to register an author in the system.
    /// Excludes username and email.
    /// 
    /// Rely on External Identity Provider. 
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RegisterWithJWT(
        // Rely on dependency injection.
        [FromServices] HttpContext context,
        [FromServices] UserManager<Author> manager
    )
    {
        // XXX: Should Am I Log this process? Where & how?
        //      Seems through `[FromServices] ILogger logger`.

        // JWT authorization token is already checked by 
        // authorization middleware, so it's safe to use Claims
        // given.

        // Also, for simplicity and standardness, we'll use
        // our custom claims.

        // They are:
        // ...................:............:....................
        // * e-mail           : (required) :
        // * login            : (required) :
        // * public-name      : (optional) :
        // * avatar-url       : (optional) :
        // ...................:............:....................
        // * provider-name    : (required) :
        // * provider-key     : (required) :
        // * provider-display : (required) :
        // ...................:............:....................

        // Get info 'bout user from claims.
        var user = context.User;

        // Info
        var email =
            user.FindFirstValue(ClaimEmail) ??
            throw new InvalidOperationException();

        var login =
            user.FindFirstValue(ClaimLogin) ??
            throw new InvalidOperationException();

        var publicName = user.FindFirstValue(ClaimPublicName);

        var avatarUrl = user.FindFirstValue(ClaimAvatar);

        // Get info about real provider which is required.
        var providerName =
            user.FindFirstValue(ClaimProviderName) ??
            throw new InvalidOperationException();

        var providerKey =
            user.FindFirstValue(ClaimProviderKey) ??
            throw new InvalidOperationException();

        var providerDisplay =
            user.FindFirstValue(ClaimProviderDisplay) ??
            throw new InvalidOperationException();


        // TODO: Download AvatarPhoto by AvatarUrl to save it.
        //       Must be not critial if photo resource is 
        //       unavailable.


        Author? author = await manager.FindByLoginAsync(
            providerName,
            providerKey
        );

        bool processHasSuccess;

        if (author is not null)
            processHasSuccess = true; // We found author.
        else
        {
            // We need to register author. We're not sure 
            // if it has successful now. Let check it out.

            // User is not present currently.

            // Create user entity to register.
            author = new Author()
            {
                Email = email,
                UserName = login,
                PublicName = publicName ?? "Alien"
            };

            var result = await manager.AddLoginAsync(
                author,
                new UserLoginInfo(
                    providerName,
                    providerKey,
                    providerDisplay
                )
            );

            processHasSuccess = result.Succeeded;

            if (processHasSuccess)
            {
                // Refetch author to get they's Id.
                author = await manager.FindByLoginAsync(
                    providerName,
                    providerKey
                );

                Debug.Assert(author is not null);
            }
        }

        if (processHasSuccess)
        {
            // Nice! We've registered new user! Horay!
            return Ok(author.ToResponse());
        }

        // We can't do it...
        return StatusCode(500);
    }

    /// <summary>
    /// Gets info about author. `@me` referneced version. 
    /// Gets info about current authorized author.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="manager"></param>
    [Authorize]
    [HttpGet("/@me")]
    public async Task<IActionResult> GetAuthorInfo(
        // Rely on dependency injection.
        [FromServices] HttpContext context,
        [FromServices] UserManager<Author> manager
    )
    {
        Author? author =
            await GetSessionAuthor(manager, context);

        if (author is null)
            // It seems user forgot to register into system.
            return Forbid();

        return Ok(author.ToResponse());
    }

    /// <summary>
    /// Helper function to find current session author.
    /// </summary>
    /// <returns>Author for session if found.</returns>
    public static Task<Author?> GetSessionAuthor(
        UserManager<Author> userManager,
        HttpContext httpContext
    ) => userManager.FindByEmailAsync(
        httpContext.User.FindFirstValue(ClaimEmail) ??
        throw new InvalidOperationException($"NO {ClaimEmail}")
    );
}
