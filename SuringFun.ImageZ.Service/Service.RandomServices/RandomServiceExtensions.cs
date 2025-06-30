namespace SuringFun.ImageZ.Service.Service.RandomServices;

/// <summary>
/// Extensions for random services.
/// </summary>
public static class RandomServiceExtensions
{
    /// <summary>
    /// Adds `CryptoRandomService` instance as `IRamdomService`
    /// implementation.
    /// </summary>
    /// <param name="services"></param>
    public static void AddCryptoRandom(
        this IServiceCollection services)
    {
        services.
            AddSingleton<
                IRandomService,
                CryptoRandomService>();
    }
}