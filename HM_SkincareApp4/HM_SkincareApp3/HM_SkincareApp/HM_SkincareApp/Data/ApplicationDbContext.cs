using HM_SkincareApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


// PASUL 3 - useri si roluri

namespace HM_SkincareApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<BookmarkCollection> BookmarkCollections { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // definirea relatiei many-to-many dintre Collection si Bookmark

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<BookmarkCollection>()
                .HasKey(bm => new { bm.Id, bm.BookmarkId, bm.CollectionId });


            // definire relatii cu modelele Bookmark si Collection (FK)

            modelBuilder.Entity<BookmarkCollection>()
                .HasOne(bm => bm.Bookmark)
                .WithMany(bm => bm.BookmarkCollections)
                .HasForeignKey(bm => bm.BookmarkId);

            modelBuilder.Entity<BookmarkCollection>()
                .HasOne(bm => bm.Collection)
                .WithMany(bm => bm.BookmarkCollections)
                .HasForeignKey(bm => bm.CollectionId);
        }
    }
}