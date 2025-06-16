namespace SuringFun.ImageZ.Service.Service;

/// <summary>
/// Represents key-value access to file contents. Abstraction
/// over S3 service.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Creates unique file with given prefix in the key. 
    /// </summary>
    /// <param name="prefix">Prefix to add to key.</param>
    /// <param name="content">Stream to copy from.</param>
    /// <returns>Generated unique key.</returns>
    string CreateFile(string prefix, Stream content);

    /// <summary>
    /// Reads file with given key.
    /// </summary>
    /// <param name="key">Key to the file.</param>
    /// <returns>Stream allows to read file.</returns>
    Stream ReadFile(string key);

    /// <summary>
    /// Writes contents to the file with given key.
    /// </summary>
    /// <param name="key">Key to write file for.</param>
    /// <param name="content">Stream to copy from.</param>
    void WriteFile(string key, Stream content);

    /// <summary>
    /// Deletes file at the given key.
    /// </summary>
    /// <param name="key">Key to find file.</param>
    void DeleteFile(string key);
}

