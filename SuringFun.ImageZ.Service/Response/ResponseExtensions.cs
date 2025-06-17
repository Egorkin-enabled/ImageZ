using System.Runtime.Serialization;
using SuringFun.ImageZ.Service.Model;

namespace SuringFun.ImageZ.Service.Response;

/// <summary>
/// Represents helper functions collection for responese DTO.
/// </summary>
public static class ResponseExtensions
{
    /// <summary>
    /// Converts attachment model to response.
    /// </summary>
    /// <param name="attachment">Model instnace.</param>
    /// <returns>Repsponse instance.</returns>
    public static AttachmentResponse ToResponse(
        this Attachment attachment
        ) => new()
        {
            ContentKey = attachment.ContentKey,
            PreviewKey = attachment.PreviewKey
        };


    /// <summary>
    /// Converts author model to response.
    /// </summary>
    /// <param name="attachment">Model instnace.</param>
    /// <returns>Repsponse instance.</returns>
    public static AuthorResponse ToResponse(
        this Author author
        ) => new()
        {
            Id = author.Id,
            Photo = author.AuthorPhoto?.ToResponse(),
            PublicName = author.PublicName
        };

    /// <summary>
    /// Converts publication model to response.
    /// </summary>
    /// <param name="publication">Model.</param>
    /// <returns>Response.</returns>
    public static PublicationResponse ToResponse(
        this Publication publication
        ) => new()
        {
            Id = publication.Id,
            Author = publication.Author?.ToResponse(),
            Attachment = publication.Attachment.ToResponse(),

            CreationDate = publication.CreationDate.AsUnix()
        };

    /// <summary>
    /// Converts emotion model to emotion response.
    /// </summary>
    /// <param name="emotion">Model.</param>
    /// <returns>Response.</returns>
    public static EmotionResponse ToResponse(
        this Emotion emotion
        ) => new()
        {
            Id = emotion.Id,
            Source = emotion.Source.ToResponse(),
            Target = (object?)(emotion.PublicationTarget?.ToResponse()) ??
                     emotion.AuthorTarget!.ToResponse(),
            TargetKind = emotion.PublicationTarget is null ?
                        Emotion.TargetKind.Author :
                        Emotion.TargetKind.Publication,
            EmotionKind = emotion.Kind,
            CreationDate = emotion.CreationDate.AsUnix()
        };

    /// <summary>
    /// Extracts date & converts it to unix seconds.
    /// </summary>
    /// <param name="date">Date to convert.</param>
    /// <returns>Unix time seconds of date.</returns>
    private static long AsUnix(
        this DateTime date
        ) => (
            (DateTimeOffset)date
            ).ToUnixTimeSeconds();
}