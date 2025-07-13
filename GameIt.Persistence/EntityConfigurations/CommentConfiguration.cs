using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameIt.Domain.EntityMapping;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        // Primary Key 
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(500);

        // Timestamps 
        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Game)
            .WithMany(g => g.Comments)
            .HasForeignKey(c => c.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
