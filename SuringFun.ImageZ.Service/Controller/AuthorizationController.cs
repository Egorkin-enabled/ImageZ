using System.Buffers.Text;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuringFun.ImageZ.Service.Model;
using SuringFun.ImageZ.Service.Response;
using static SuringFun.ImageZ.Essentials.AuthorizationConsts;

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
    /// Uses JWT token to register an author in the system.
    /// Excludes username and email.
    /// 
    /// Rely on External Identity Provider. 
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RegisterWithJWT(
        // Rely on dependency injection.
        [FromServices] IHttpContextAccessor contextAccess,
        [FromServices] UserManager<Author> manager
    )
    {
        var context = contextAccess.HttpContext!;

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
            // if it has success now. Let check it out.

            // User is not present currently.

            // Create user entity to register.

            byte[] stamp = new byte[128];
            RandomNumberGenerator.Create().GetBytes(stamp);

            author = new Author()
            {
                Email = email,
                UserName = email,
                PublicName = publicName ?? "Alien",
                // SecurityStamp = Convert.ToBase64String(stamp)
            };
            await manager.CreateAsync(author);
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
            else
                return StatusCode(500, result.Errors);

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
    [HttpGet("@me")]
    public async Task<IActionResult> GetAuthorInfo(
        // Rely on dependency injection.
        [FromServices] IHttpContextAccessor contextAccess,
        [FromServices] UserManager<Author> manager
    )
    {
        Author? author =
            await GetSessionAuthor(
                manager,
                contextAccess.HttpContext!
                );

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
