using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SuringFun.ImageZ.Service.Model;

namespace SuringFun.ImageZ.Service.Service.Databases;

/// <summary>
/// Database context for the system. 
/// </summary>
public class ServiceDbContext :
    Microsoft.AspNetCore.
        Identity.EntityFrameworkCore.
            IdentityDbContext<Author, IdentityRole<int>, int>
{
    public ServiceDbContext(DbContextOptions options) : base(options)
    { }

    protected ServiceDbContext()
    { }

    protected override void OnModelCreating(
        ModelBuilder _)
    {
        // Let `IdentityDbContext` configure itself.
        base.OnModelCreating(_);

        // `Emotion` configuration.

        _.Entity<Emotion>().
            HasKey(x => x.Id);

        _.Entity<Emotion>().
            HasOne(x => x.Source).
            WithMany().
            IsRequired();

        _.Entity<Emotion>().
            HasOne(x => x.AuthorTarget).
            WithMany().
            IsRequired(false);

        _.Entity<Emotion>().
            HasOne(x => x.PublicationTarget).
            WithMany().
            IsRequired(false);

        // `Publication` configuration.
        _.Entity<Publication>().
            HasKey(x => x.Id);

        _.Entity<Publication>().
            HasOne(x => x.Author).
            WithMany(x => x.Publications).
            OnDelete(DeleteBehavior.SetNull); // Allow 
                                              // publication
                                              // to exist 
                                              // without author.       

        _.Entity<Attachment>().
            HasOne<Publication>().
            WithOne(x => x.Attachment).
            HasForeignKey<Publication>("AttachmentId").
            IsRequired();

        // `Author` configuration.
        _.Entity<Author>().
            HasKey(x => x.Id);
            
        _.Entity<Attachment>().
            HasOne<Author>().
            WithOne(x => x.AuthorPhoto).
            HasForeignKey<Author>("AuthorPhotoId").
            IsRequired(false);

    }
}