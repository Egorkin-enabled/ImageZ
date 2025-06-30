using Microsoft.AspNetCore.Mvc;
using SuringFun.ImageZ.Service.Service.RandomServices;

namespace SuringFun.ImageZ.Service.Service.FileServices;

/// <summary>
/// Represents S3 service for local filesystem implementation.
/// </summary>
public class LocalFileService : IFileService
{
    /// <summary>
    /// Directory, where files are stored in local filesystem.
    /// </summary>
    public string LocalDirectory { get; } = string.Empty;

    private IRandomService m_randomService;

    /// <summary>
    /// Creates Local file service instance. Need for 
    /// `IRandomSerivice` as dependency.
    /// </summary>
    /// <param name="randomService"></param>
    public LocalFileService(
        [FromServices] IRandomService randomService,
        string localDirectoryPath
    )
    {
        m_randomService = randomService;
        LocalDirectory = localDirectoryPath;
    }

    public string CreateFile(string prefix, Stream content)
    {
        // Straightforward approach. Generate name, then check 
        // unique. Keep until unique name found.
        throw new NotImplementedException();
    }

    public void DeleteFile(string key)
    {
        throw new NotImplementedException();
    }

    public Stream ReadFile(string key)
    {
        throw new NotImplementedException();
    }

    public void WriteFile(string key, Stream content)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Extensions to add local file service.
/// </summary>
public static class LocalFileServiceExtensions
{
    /// <summary>
    /// Adds local file service as `IFileService` 
    /// implementation.
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="localDirectory">
    /// Path to local directory
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// No `IRandomSerivce` binded found!
    /// </exception>
    public static void AddLocalFileService(
        this IServiceCollection services, string localDirectory)
    {
        services.AddSingleton<IFileService>(
            serviceProvider => new LocalFileService(
                serviceProvider.GetService<IRandomService>() ??
                throw new InvalidOperationException(
                    $"No `{nameof(IRandomService)}` found."),
                    localDirectory
                )
            );
    }
}