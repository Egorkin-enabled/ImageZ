using System.Security.Cryptography;

namespace SuringFun.ImageZ.Service.Service.RandomServices;

/// <summary>
/// Crypto implementation of `IRandomService`. 
/// </summary>
/// <remarks>
/// Adapter for `RandomNumberGenerator` instance.
/// </remarks>
public class CryptoRandomService : IRandomService
{
    // Just adapt system component.
    private RandomNumberGenerator m_random =
        RandomNumberGenerator.Create();

    public void GetBytes(byte[] outData)
        => m_random.GetBytes(outData);
}
