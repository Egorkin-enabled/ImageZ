using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuringFun.ImageZ.Service.Model;

namespace SuringFun.ImageZ.Service.Service;

/// <summary>
/// Database context for the system. 
/// </summary>
public class ServiceDbContext :
    Microsoft.AspNetCore.
        Identity.EntityFrameworkCore.
            IdentityDbContext<Author, IdentityRole<int>, int>
{
    public ServiceDbContext(DbContextOptions options) : base(options)
    {}

    protected ServiceDbContext()
    {}

    protected override void OnModelCreating(
        ModelBuilder _)
    {
        // Let `IdentityDbContext` configure itself.
        base.OnModelCreating(_);

        // `Emotion` configuration.

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
            HasOne(x => x.Author).
            WithMany(x => x.Publications).
            OnDelete(DeleteBehavior.SetNull); // Allow 
                                              // publication
                                              // to exist 
                                              // without author.       

        _.Entity<Publication>().
            HasOne(x => x.Attachment).
            WithOne().
            HasForeignKey<Attachment>().
            OnDelete(DeleteBehavior.Cascade);

        // `Author` configuration.
        _.Entity<Author>().
            HasOne(x => x.AuthorPhoto).
            WithOne().
            HasForeignKey<Attachment>().
            IsRequired(false);
    }
}