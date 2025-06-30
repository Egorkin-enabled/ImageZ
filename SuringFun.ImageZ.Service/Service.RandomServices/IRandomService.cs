namespace SuringFun.ImageZ.Service.Service.RandomServices;

/// <summary>
/// Service which allows to introduce randomness.
/// </summary>
public interface IRandomService
{
    /// <summary>
    /// Fills given data array.
    /// </summary>
    /// <param name="data">Data array to fill.</param>
    void GetBytes(byte[] outData);
}
