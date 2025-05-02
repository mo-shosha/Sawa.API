using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Data.Config
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Content)
                   .HasMaxLength(2000)
                   .IsRequired();

            builder.HasOne(p => p.Charity)
                   .WithMany(u => u.Posts)
                   .HasForeignKey(p => p.CharityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Comments)
                   .WithOne(c => c.Post)
                   .HasForeignKey(c => c.PostId);

            builder.HasMany(p => p.Photos)
                   .WithOne(photo => photo.Post)
                   .HasForeignKey(photo => photo.PostId);
        }
    }
}
