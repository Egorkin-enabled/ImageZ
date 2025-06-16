namespace SuringFun.ImageZ.Service.Service;

/// <summary>
/// Represent extensions for `IFileService` to keep interface
/// simple and untouchable. 
/// </summary>
public static class FileServiceExtensions
{
    /// <summary>
    /// Creates unique file with given key prefix from byte 
    /// array.
    /// </summary>
    /// <param name="service">Underhood service used.</param>
    /// <param name="prefix">Prefix to unique key</param>
    /// <param name="content">
    /// Byte content to write into.
    /// </param>
    /// <returns>
    /// Unique key with given prefix generated.
    /// </returns>
    public static string CreateFile(
        this IFileService service,
        string prefix,
        byte[] content
        ) => service.CreateFile(
            prefix,
            new MemoryStream(content) // No need to close
                                      // MemoryStream.
            );

    /// <summary>
    /// Reads full content of given file into byte buffer.
    /// </summary>
    /// <param name="service">Underhood service used.</param>
    /// <param name="key">Key of file to read.</param>
    /// <returns>
    /// Byte array, which is full file contents
    /// </returns>
    public static byte[] ReadFileBytes(
        this IFileService service,
        string key
        )
    {
        using var input = service.ReadFile(key);

        if (input.CanSeek)
        {
            // Can simply create huge buffer to save into.
            int bufferLength;
            checked
            {
                bufferLength = (int)input.Length;
            }

            byte[] outputBuffer = new byte[bufferLength];
            input.ReadExactly(outputBuffer);
            return outputBuffer;
        }

        // We can't seek, so we have to work with expandables.
        MemoryStream outputStream = new();

        byte[] buffer = new byte[1024];

        int _;
        while ((_ = input.Read(buffer)) < 0)
            outputStream.Write(buffer, 0, _);

        return outputStream.ToArray();
    }
}

