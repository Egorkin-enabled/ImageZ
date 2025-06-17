namespace SuringFun.ImageZ.Service.Response;

/// <summary>
/// Represents response for publications.
/// </summary>
public class PublicationResponse
{
    /// <summary>
    /// Identifer of the publication.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Author, who created publication.
    /// </summary>
    public AuthorResponse? Author { get; set; }

    /// <summary>
    /// Attachment, which represents publication.
    /// </summary>
    public AttachmentResponse Attachment { get; set; }
        = default!;

    /// <summary>
    /// Publication creation date in Unix time. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use `long` instead of `DateTime` allows us to get 
    /// easy-to-parse timestamp, which is independed out of 
    /// client's `CultureInfo` environment.
    /// </para>
    /// <para>
    ///  `DateTime` instance is converted into date-formatted 
    /// string during serialization. Format of string vary
    /// out of user culture environment, so we can face with API 
    /// incopatibility problem due to user's local machine 
    /// language settings. This leads to client-side code should
    /// cary out of dates format consistance. Why do we need add
    /// additional complexity to client-side code, right?
    /// </para>
    /// <para>
    /// This decision increases time portability over machines.
    /// Unix time is well-known over the world and modern 
    /// browsers have API to convert it into their's datetime
    /// representation object. Unix timestamp exchange allows us
    /// to not think about client's environment during parsing
    /// or sending. Client needs only correct datetime object
    /// to represent their's local time instead of Utc one, 
    /// which is out of parsing problem.
    /// </para>
    /// <para>
    /// Yeah, we lose microseconds, but do you really need them?
    /// Moreover, for creation date even seconds time resolution 
    /// may be excessful! So, we lose nothing. Only gain.
    /// </para>
    /// </remarks>
    public long CreationDate { get; set; }
}

