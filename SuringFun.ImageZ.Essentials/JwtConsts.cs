using Microsoft.Extensions.Configuration;

namespace SuringFun.ImageZ.Essentials;

/// <summary>
/// Has consts for database.
/// </summary>
public static class DbConsts
{
    /// <summary>
    /// Name of env to store database connection string.
    /// </summary>
    public const string EnvDbConStr = "SF_IMAGE_Z_DB_CON_CON";
}

public static class EnvironmentHelper
{
    /// <summary>
    /// Challenges environment to extract given variable.
    /// In case if variable is not present, it `goes bankrupt`.
    /// </summary>
    /// <param name="env">Environment's variable name.</param>
    /// <returns>Value.</returns>
    /// <exception cref="InvalidOperationException">
    /// No variable found.
    /// </exception>
    [Obsolete("Deprecated: use overload with IConfiguration")]
    public static string ChallengeEnv(string env)
        => Environment.GetEnvironmentVariable(env) ??
            throw new InvalidOperationException(
                $"No `{env}` environment variable present. " +
                "Please, check if server is configured " +
                "properly."
                );

    public static string ChallengeEnv(
        IConfiguration config, string env
        ) => config[env] ??
         throw new InvalidOperationException(
                $"No `{env}` config variable present. " +
                "Please, check if server is configured " +
                "properly."
                );

}

/// <summary>
/// Constants with JWT consts.
/// </summary>
public static class JwtConsts
{
    /// <summary>
    /// Env which store issuer.
    /// </summary>
    public const string EnvJwtIssuer =
        "SF_IMAGE_Z_JWT_ISSUER";

    /// <summary>
    /// Env which store audience.
    /// </summary>
    public const string EnvJwtAudience =
        "SF_IMAGE_Z_JWT_AUDIENCE";

    /// <summary>
    /// Env, which store secret to encrypt JWT token.
    /// </summary>
    public const string EnvJwtSecret =
        "SF_IMAGE_Z_JWT_SECRET";

    public const string EnvJwtDayLifetime =
        "SF_IMAGE_Z_JWT_DAY_LIFETIME";
}