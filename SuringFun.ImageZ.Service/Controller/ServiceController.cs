using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuringFun.ImageZ.Service.Model;
using SuringFun.ImageZ.Service.Request;
using SuringFun.ImageZ.Service.Response;
using SuringFun.ImageZ.Service.Service.Databases;
using SuringFun.ImageZ.Service.Service.FileServices;

namespace SuringFun.ImageZ.Service.Controller;

/// <summary>
/// API for service.
/// </summary>
public class ServiceController :
    Microsoft.AspNetCore.Mvc.Controller
{
    /// <summary>
    /// Gets info about author.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("authors/{authorId}")]
    public IActionResult GetAuthor(
        [FromServices] ServiceDbContext context,
        [FromRoute] int authorId
    )
    {
        Author? author = // Allow `null` to handle `NotFound`.
            context.Set<Author>().
            FirstOrDefault(x => x.Id == authorId);

        if (author is not null)
            return Ok(author.ToResponse());

        return NotFound();
    }

    [Authorize]
    [HttpPost("authors/@me/photo")]
    public async Task<IActionResult> PostPhoto(
        [FromServices] UserManager<Author> userManager,
        [FromServices] ServiceDbContext dbContext,
        [FromServices] IFileService fileService,
        [FromServices] IHttpContextAccessor accessor,

        [FromBody] PhotoRequest photoRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Author author =
            await AuthorizationController.GetSessionAuthor(
                userManager,
                accessor.HttpContext!
            ) ??
            throw new InvalidOperationException();

        Attachment? oldPhoto = author.AuthorPhoto;

        Attachment newPhoto = new()
        {
            ContentKey = fileService.CreateFile(
                "photo_content_",
                photoRequest.Content
                ),
            PreviewKey = photoRequest.Preview is not null ?
                    fileService.CreateFile(
                        "photo_preview_",
                        photoRequest.Preview
                    ) : null
        };

        dbContext.Add(newPhoto);

        if (oldPhoto is not null)
            dbContext.Remove(oldPhoto);

        author.AuthorPhoto = newPhoto;
        var task = dbContext.SaveChangesAsync();

        FreePhoto(fileService, oldPhoto);

        await task;

        return Ok();
    }

    private static void FreePhoto(IFileService fileService, Attachment? oldPhoto)
    {
        if (oldPhoto is not null)
        {
            fileService.DeleteFile(oldPhoto.ContentKey);

            if (oldPhoto.PreviewKey is not null)
                fileService.DeleteFile(oldPhoto.PreviewKey);
        }
    }

    [Authorize]
    [HttpDelete("authors/@me/photo")]
    public async Task<IActionResult> PostPhoto(
        [FromServices] UserManager<Author> userManager,
        [FromServices] ServiceDbContext dbContext,
        [FromServices] IFileService fileService,
        [FromServices] IHttpContextAccessor accessor
    )
    {
        Author author =
            await AuthorizationController.GetSessionAuthor(
                userManager,
                accessor.HttpContext!
            ) ??
            throw new InvalidOperationException();

        dbContext.
            Entry(author).
            Reference(x => x.AuthorPhoto).
            Load();

        Attachment? oldPhoto = author.AuthorPhoto;
        author.AuthorPhoto = null;

        if (oldPhoto is not null)
            dbContext.Remove(oldPhoto);

        var task = dbContext.SaveChangesAsync();

        FreePhoto(fileService, oldPhoto);

        await task;

        return Ok();
    }

    [Authorize]
    [HttpPost("publications")]
    public async Task<IActionResult> PostPublication(
        [FromServices] UserManager<Author> userManager,
        [FromServices] ServiceDbContext dbContext,
        [FromServices] IFileService fileService,
        [FromServices] IHttpContextAccessor accessor,

        [FromBody] PublicationRequest publicationRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        HttpContext httpContext = accessor.HttpContext!;

        Author author =
            await AuthorizationController.GetSessionAuthor(
                userManager,
                httpContext
            ) ??
            throw new InvalidOperationException();

        dbContext.
            Entry(author).
            Reference(x => x.AuthorPhoto).
            Load();

        Publication publicationInstance = new()
        {
            Author = author,

            // Use `UtcNow` to be able to get time independed 
            // out of server location.
            // That's allow us to calculate local zone time at 
            // the client. World-wide Echange is important!
            CreationDate = DateTime.UtcNow,

            Attachment = new Attachment()
            {
                Description = publicationRequest.Description,
                ContentKey =
                    fileService.CreateFile(
                        "pub_content_",
                        publicationRequest.Content
                        ),
                PreviewKey =
                    publicationRequest.Preview is not null ?
                    fileService.CreateFile(
                        "pub_preivew_",
                        publicationRequest.Preview
                        ) : null
            }
        };

        dbContext.Add(publicationInstance);
        dbContext.SaveChanges();

        return Ok(publicationInstance.ToResponse());
    }

    [Authorize]
    [HttpDelete("publications/{publicationId}")]
    public async Task<IActionResult> DeletePublication(
        [FromServices] UserManager<Author> userManager,
        [FromServices] ServiceDbContext dbContext,
        [FromServices] IFileService fileService,
        [FromServices] IHttpContextAccessor accessor,

        [FromRoute] int publicationId
    )
    {
        Author author =
            await AuthorizationController.GetSessionAuthor(
                userManager,
                accessor.HttpContext!
            ) ??
            throw new InvalidOperationException();

        // Can't use `is not null` here due to CS8122.
        // Entity Framewrok uses expression tree to parse it
        // and translate into SQL. So, due to incompatibility
        // issues lots of new syntax (from C# 3.0) is 
        // forbidden.

        Publication? publication =
            dbContext.Set<Publication>().First(
                x => x.Author != null && // Workaround `CS8122`. 
                     x.Author.Id == author.Id &&
                     x.Id == publicationId
                );

        if (publication is null)
            return NotFound();

        dbContext.Set<Publication>().Remove(publication);
        dbContext.SaveChanges();

        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("publications/{publicationId}")]
    public IActionResult GetPublication(
        [FromServices] ServiceDbContext dbContext,
        [FromRoute] int publicationId
    )
    {
        Publication? publication =
            dbContext.
                Set<Publication>().
                Include(x => x.Attachment).
                Include(x => x.Author).
                ThenInclude(x => x!.AuthorPhoto).
                FirstOrDefault(x => x.Id == publicationId);

        if (publication is null)
            return NotFound();

        return Ok(publication.ToResponse());
    }

    [AllowAnonymous]
    [HttpGet("publications")]
    public IActionResult GetPublicationsPage(
        [FromServices] ServiceDbContext dbContext,

        [FromQuery] int offset,
        [FromQuery] int limit,
        [FromQuery] int? authorId = null,
        [FromQuery] string? searchPattern = null
    )
    {
        // Let build query dynamically.

        // We'll use inner expression innovocation, so we
        // need to use `AsExpandable` to `LinqKit` &
        // `Linq.Expression.Optimizer`  reconstruct our 
        // expression. This allows us to `inline` calls & we
        // get straightforward expression, which is 
        // understandable by `EntityFramework.Core`. 

        var query =
            dbContext.Set<Publication>().
                Include(x => x.Attachment).
                Include(x => x.Author).
                ThenInclude(x => x!.AuthorPhoto).
                AsExpandable(ExpressionOptimizer.visit);

        if (authorId is int authorIdResolved)
            query = query.Where(
                x => x.Author != null &&
                     x.Author.Id == authorIdResolved
                );

        if (searchPattern is string searchPatternResolved)
        {
            string[] words = searchPatternResolved.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries
                );

            if (words.Length > 0)
            {

                var prediacte =
                PredicateBuilder.New<Publication>(
                    x => x.Attachment.Description != null
                    );

                var searchPrediacte =
                    PredicateBuilder.New<Publication>(false);

                foreach (var key in words)
                    searchPrediacte.Or(
                        x =>
                        x.Attachment.Description!.ToLower().
                        Contains(key.ToLower()));

                prediacte = prediacte.And(searchPrediacte);

                query = query.Where(prediacte);
            }
        }

        query = query.OrderByDescending(x => x.CreationDate);
        query = query.Skip(offset).Take(limit);

        // After query built, we can read all of the 
        // publications we need.

        Publication[] publications = query.ToArray();

        return Ok(
            new PageRepsonse<PublicationResponse>()
            {
                Offset = offset,
                Limit = limit,
                Items =
                    publications.
                    Select(x => x.ToResponse()).
                    ToArray()
            }
        );
    }

    [AllowAnonymous]
    [HttpGet("emotions")]
    public IActionResult GetEmotionsPage(
        [FromServices] ServiceDbContext dbContext,

        [FromQuery] int offset,
        [FromQuery] int limit,
        [FromQuery] int authorId,
        [FromQuery] Emotion.TargetKind targetKind
    )
    {
        IQueryable<Emotion> query =
            dbContext.Set<Emotion>().
            Include(x => x.Source).
            ThenInclude(x => x.AuthorPhoto).
            Include(x => x.AuthorTarget).
            ThenInclude(x => x!.AuthorPhoto).
            Include(x => x.PublicationTarget).
            ThenInclude(x => x!.Attachment).
            Include(x => x.PublicationTarget).
            ThenInclude(x => x!.Author).
            ThenInclude(x => x!.AuthorPhoto).
            Where(x => x.Source.Id == authorId).
            OrderByDescending(x => x.CreationDate);

        // Query will differ based of kind.
        switch (targetKind)
        {
            case Emotion.TargetKind.Author:
                query =
                    query.Where(
                        x => x.AuthorTarget != null);
                break;

            case Emotion.TargetKind.Publication:
                query =
                    query.Where(
                        x => x.PublicationTarget != null);
                break;
        }

        query = query.Skip(offset).Take(limit);

        // Query built. Can execute it.
        Emotion[] emotions = query.ToArray();

        return Ok(
            new PageRepsonse<EmotionResponse>()
            {
                Offset = offset,
                Limit = limit,
                Items =
                    emotions.
                        Select(x => x.ToResponse()).
                        ToArray()
            }
        );
    }

    [Authorize]
    [HttpDelete("emotions/{emotionId}")]
    public async Task<IActionResult> DeleteEmotion(
        [FromServices] UserManager<Author> userManager,
        [FromServices] ServiceDbContext dbContext,
        [FromServices] IHttpContextAccessor accessor,

        [FromRoute] int emotionId
    )
    {
        Author author =
            await AuthorizationController.GetSessionAuthor(
                userManager,
                accessor.HttpContext!
            ) ??
            throw new InvalidOperationException();

        Emotion? emotion =
            dbContext.Set<Emotion>().
                Include(x => x.AuthorTarget).
                ThenInclude(x => x!.AuthorPhoto).
                Include(x => x.PublicationTarget).
                ThenInclude(x => x!.Attachment).
                Include(x => x.PublicationTarget).
                ThenInclude(x => x!.Author).
                ThenInclude(x => x!.AuthorPhoto).
                FirstOrDefault(
                    x => x.Source.Id == author.Id &&
                        x.Id == emotionId
                );

        if (emotion is null)
            return NotFound();

        dbContext.Set<Emotion>().Remove(emotion);
        dbContext.SaveChanges();

        return Ok();
    }

    [Authorize]
    [HttpPost("emotions")]
    public async Task<IActionResult> PostEmotion(
        [FromServices] UserManager<Author> userManager,
        [FromServices] ServiceDbContext dbContext,
        [FromServices] IHttpContextAccessor httpContextA,

        [FromBody] EmotionRequest emotionRequest
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        (var targetId, var targetKind, var emotionKind) = (
            emotionRequest.TargetId,
            emotionRequest.TargetKind,
            emotionRequest.EmotionKind
        );

        Author author =
            await AuthorizationController.GetSessionAuthor(
                userManager,
                httpContextA.HttpContext!
            ) ??
            throw new InvalidOperationException();

        // Let initialize general part. Initialization differs
        // little bit based on `TargetKind` passed.
        Emotion emotionInstance = new Emotion()
        {
            CreationDate = DateTime.UtcNow,
            Source = author,
            Kind = emotionKind
        };

        // Select target based on kind.

        switch (targetKind)
        {
            case Emotion.TargetKind.Author:
                emotionInstance.AuthorTarget =
                    dbContext.Set<Author>().
                    Include(x => x.AuthorPhoto).
                    First(x => x.Id == targetId);
                break;

            case Emotion.TargetKind.Publication:
                emotionInstance.PublicationTarget =
                    dbContext.Set<Publication>().
                    Include(x => x.Attachment).
                    First(x => x.Id == targetId);
                break;
        }

        // Emotion formed.  Let check out if similar emotion 
        // exists.

        Emotion? existEmotion =
            dbContext.
            Set<Emotion>().
            FirstOrDefault(
                x =>
                x.Source.Id == emotionInstance.Source.Id && (
                    (
                        x.AuthorTarget != null &&
                        emotionInstance.AuthorTarget
                            != null &&
                        x.AuthorTarget.Id ==
                            emotionInstance.AuthorTarget.Id
                        ) ||
                    (
                        x.PublicationTarget != null &&
                        emotionInstance.PublicationTarget
                            != null &&
                        x.PublicationTarget.Id ==
                            emotionInstance.PublicationTarget.Id
                    )
                )
            );

        if (existEmotion is not null)
        {
            // XXX: Not sure how to fight with the race 
            //      condition without global lock here.
            //      Maybe, just ignore this problem?
            //      Or always create new record instead of
            //      modifying existing one? 

            // Emotion is exist. Let modify it instead of 
            // insertion.

            if (existEmotion.Kind != emotionInstance.Kind)
            {
                // Change our instance.
                existEmotion.Kind = emotionInstance.Kind;
                dbContext.SaveChanges();
            }
            else
            { /* Need no any change */ }

            // Return emotion we found.
            return Ok(existEmotion.ToResponse());
        }
        else
        {
            // No emotion exist.
            // Let save formed emotion.
            dbContext.Set<Emotion>().Add(emotionInstance);
            dbContext.SaveChanges();

            // Return created instance.
            return Ok(emotionInstance.ToResponse());
        }
        
    }
}