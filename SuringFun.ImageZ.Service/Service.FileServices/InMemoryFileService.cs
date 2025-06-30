
using System.Diagnostics.Contracts;
using System.Globalization;

namespace SuringFun.ImageZ.Service.Service.FileServices;

/// <summary>
/// In memory `IFileService` implementation. Thread safe.
/// </summary>
/// <remarks>
/// Not for production!
/// </remarks>
public class InMemoryFileService : IFileService
{
    private Dictionary<string, byte[]> m_data = new();

    private int m_uniqueCounter = 0;

    // Count in multi-threaded access during debugging.
    private object m_lock = new();

    /// <summary>
    /// Creates in-memory file.
    /// </summary>
    /// <param name="prefix">File prefix.</param>
    /// <param name="content">File content.</param>
    /// <returns>Name generated.</returns>
    public string CreateFile(string prefix, Stream content)
    {
        // That's important to clone data given. Won't user 
        // change contents through reference even if contents
        // stream `CanSeek`.
        byte[] contentsFull = ExtractContents(content);

        // Need to write data into our dictionary. Should be
        // thread safe, so, use lock.

        lock (m_lock)
        {
            // Won't key names depend out of current server 
            // culture.
            string givenName = String.Create(
                CultureInfo.InvariantCulture,
                $"{prefix}{m_uniqueCounter}"
            );

            m_data[givenName] = contentsFull;
            ++m_uniqueCounter;

            return givenName;
        }
    }

    public void DeleteFile(string key)
    {
        // Just remove key.
        lock (m_lock)
            m_data.Remove(key);
    }

    public Stream ReadFile(string key)
    {
        lock (m_lock)
        {
            // Using ctor we create read-only stream.
            MemoryStream memory = new(
                m_data[key]
            );

            return memory;
        }
    }

    public void WriteFile(string key, Stream content)
    {
        // We need to replace file contents.

        // That's important to clone data given. Won't user 
        // change contents through reference even if contents
        // stream `CanSeek`.
        byte[] contentsFull = ExtractContents(content);

        lock (m_lock)
        {
            // Emulate real S3 server. Won't create file 
            // accidentely. Check existance.

            if (!m_data.ContainsKey(key))
                throw new FileNotFoundException(
                    "In memory file should be created first!" +
                    $"File name: {key}"
                    );

            m_data[key] = contentsFull;
        }
    }


    // TODO: Move this method into `Essentials` category.
    private static byte[] ExtractContents(Stream content)
    {
        MemoryStream copy = new();

        const int FixedBufferSize = 1024;

        Span<byte> fixedBuffer =
            stackalloc byte[FixedBufferSize];

        int readAmount;

        while ((readAmount = content.Read(fixedBuffer)) > 0)
            copy.Write(fixedBuffer);

        // Data is copied. It's owner's responsibility to close
        // outer `contents` stream.

        byte[] contentsFull = copy.ToArray();
        return contentsFull;
    }
}

/// <summary>
/// Helper functions.
/// </summary>
public static class InMemoryFileServiceExtensions
{
    /// <summary>
    /// Adds in memory file service as `IFileService` 
    /// implementaiton.
    /// </summary>
    /// <param name="services">Services to add into.</param>
    public static void AddInMemoryFileService(
        this IServiceCollection services)
    {
        // Need no additional arguments for the service.
        services.
            AddSingleton<IFileService, InMemoryFileService>();
    }
}
