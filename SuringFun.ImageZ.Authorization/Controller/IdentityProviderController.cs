using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using static SuringFun.ImageZ.Essentials.AuthorizationConsts;
using static SuringFun.ImageZ.Essentials.JwtConsts;

using static SuringFun.ImageZ.Essentials.EnvironmentHelper;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Globalization;

namespace SuringFun.ImageZ.Authorization.Controller;

/// <summary>
/// <para>
/// Incapsulates cross-authorization logic. Converts provider's
/// JWT tokens into internal service's one.
/// </para>
/// <para>
/// Provides it's own Identity Provider logic for the service.
/// </para>
/// </summary>
[Route("/authorization")]
public class IdentityProviderController :
    Microsoft.AspNetCore.Mvc.Controller
{
    // Yeah... We're Identity Boss now ~!

    private readonly string Issuer;
    private readonly string Audience;
    private readonly float DayLifetime;
    private readonly byte[] Secret;

    private readonly SymmetricSecurityKey Key;
    private readonly SigningCredentials Credentials;

    public IdentityProviderController(
        [FromServices] IConfiguration configuration
    )
    {
        Issuer = ChallengeEnv(configuration, EnvJwtIssuer);
        Audience = ChallengeEnv(configuration, EnvJwtAudience);
        DayLifetime = float.Parse(
            ChallengeEnv(
                configuration,
                EnvJwtDayLifetime
            ),
            CultureInfo.InvariantCulture
        );

        string secretStr =
            ChallengeEnv(configuration, EnvJwtSecret);

        Secret = Encoding.UTF8.GetBytes(secretStr);

        Key = new(Secret);

        // TODO: See different crypto algo-s available.
        Credentials = new(Key, SecurityAlgorithms.HmacSha256);
    }

    /// <summary>
    /// Registers user. 
    /// </summary>
    [HttpPost("self")]
    public async Task<IActionResult> CreateUser(
        [FromServices] UserManager<IdentityUser> userManager,
        [FromServices] IdentityDbContext dbContext,
        [FromBody] RegisterRequest request
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // TODO: Add E-mail confirmation logic.

        // XXX: `UserManager` has email unique checking but how
        //      could I reuse internal checking result?
        //      Maybe try-catch, or it would be better to 
        //      dublicate mail checking logic also here, in 
        //      route handler body?

        // TODO: Add E-mail conflit handling (return not 500, 
        //       but significant code in this case).

        var result =
            await userManager.CreateAsync(
                new IdentityUser()
                {
                    Email = request.Email,
                    UserName = request.Email
                },
                request.Password
            );

        if (!result.Succeeded)
            return StatusCode(500, result.Errors);


        return Ok();
    }

    [HttpPost("self/JWT")]
    public async Task<IActionResult> AuthorizeUser(
        [FromServices] UserManager<IdentityUser> userManager,
        [FromBody] AuthorizeRequest authorizeRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        // Let find user by mail and password.
        var user = await userManager.FindByEmailAsync(
            authorizeRequest.Email
        );

        if (user is null)
            return Forbid();

        // Check their pass.
        var passwordFits =
            await userManager.CheckPasswordAsync(
                user, 
                authorizeRequest.Password
                );

        if (!passwordFits)
            return Forbid();

        // Let create claims for user.
            var userEx =
            new InvalidOperationException(
                "User not fullfiled."
                );

        Claim[] claims = [
            new(ClaimEmail, user.Email ?? throw userEx),
            new(ClaimProviderName, "suringfun.imagez.com"),
            new(ClaimProviderKey, user.Id),
            new(ClaimProviderDisplay, "SF ImageZ Identity")
        ];

        // TODO: Add photo handling (!)

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(
                DayLifetime
            ),
            signingCredentials: Credentials
        );

        var securityHandler = new JwtSecurityTokenHandler();
        return Ok(new
        {
            Jwt = securityHandler.WriteToken(token)
        });
    }

    // TODO: Add external authorization support.

    // TODO: Add `POST /authorization/google   `
    // TODO: Add `POST /authorization/microsoft`
    // TODO: Add `POST       ...etc...         `
}
