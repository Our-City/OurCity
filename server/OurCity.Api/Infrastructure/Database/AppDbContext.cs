using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OurCity.Api.Infrastructure.Database;

public class AppDbContext : IdentityDbContext<User, UserRole, Guid>
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<PostVote> PostVotes { get; set; }
    public DbSet<CommentVote> CommentVotes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostBookmark> PostBookmarks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    //OnModelCreating
    // -> Can get more granular about what the tables will look like
    // -> Seed data when running migration
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasData(
                new Tag
                {
                    Id = Guid.Parse("3b84d6d5-4d4e-4e09-8a90-6c2d257ae14c"),
                    Name = "Construction",
                },
                new Tag
                {
                    Id = Guid.Parse("6b8e5470-5a3e-48a7-a3e3-142e7e8b2e02"),
                    Name = "Transportation",
                },
                new Tag
                {
                    Id = Guid.Parse("5f8b0e26-33a1-4e9f-a3c5-7e78f32f804a"),
                    Name = "Entertainment",
                },
                new Tag
                {
                    Id = Guid.Parse("91f75b8d-bf32-46af-a6a9-8f89417cbbd0"),
                    Name = "Shopping",
                },
                new Tag
                {
                    Id = Guid.Parse("c4db2614-0d47-4c16-89da-fd8c97a216f4"),
                    Name = "Food & Dining",
                },
                new Tag
                {
                    Id = Guid.Parse("8c7b9a39-b4a9-40a3-85ce-034d97a2a6c2"),
                    Name = "Parks & Recreation",
                },
                new Tag
                {
                    Id = Guid.Parse("f1f8e911-61db-45a2-b9df-7dc6de4c9a0d"),
                    Name = "Safety",
                },
                new Tag
                {
                    Id = Guid.Parse("1f59e1d4-37b7-4ad2-9f6f-431a5e8cf8b7"),
                    Name = "Community Events",
                },
                new Tag
                {
                    Id = Guid.Parse("41a6f4ac-8a91-4209-b40e-8b14b9a01873"),
                    Name = "Infrastructure",
                },
                new Tag
                {
                    Id = Guid.Parse("4f6329f1-3201-4a94-b41c-cf74ed91f777"),
                    Name = "Business",
                },
                new Tag
                {
                    Id = Guid.Parse("08e4cb83-1d93-4e0c-bc4c-30c2aee497b8"),
                    Name = "Education",
                },
                new Tag
                {
                    Id = Guid.Parse("f6d81e88-8332-4ee7-96b9-8517f7d7a2d9"),
                    Name = "Healthcare",
                },
                new Tag
                {
                    Id = Guid.Parse("7dd62a06-5a7c-44ff-a2f1-299a507d21aa"),
                    Name = "Environment",
                },
                new Tag
                {
                    Id = Guid.Parse("e122c911-8cbe-45e0-9d91-9353ed685c61"),
                    Name = "Sports",
                },
                new Tag
                {
                    Id = Guid.Parse("c6d13b79-0a6a-4db3-a219-1c6240b9ef82"),
                    Name = "Culture",
                },
                new Tag
                {
                    Id = Guid.Parse("0a7f2a8d-504c-4b17-8448-7a274a1bba44"),
                    Name = "Tourism",
                },
                new Tag
                {
                    Id = Guid.Parse("3a46f0b5-238f-4e41-bbb4-254bdb14f92e"),
                    Name = "Housing",
                },
                new Tag { Id = Guid.Parse("9e4f0c3f-02e4-4c88-bf89-9cc7cf7b63c3"), Name = "Events" }
            );
        });

        modelBuilder
            .Entity<PostBookmark>()
            .HasOne(pb => pb.User)
            .WithMany(u => u.PostBookmarks)
            .HasForeignKey(pb => pb.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<PostBookmark>()
            .HasOne(pb => pb.Post)
            .WithMany(p => p.Bookmarks)
            .HasForeignKey(pb => pb.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
