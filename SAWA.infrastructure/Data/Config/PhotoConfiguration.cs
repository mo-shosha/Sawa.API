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
    public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.ToTable("Photos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            

            builder.HasOne(p => p.Branch)
                   .WithMany(b => b.Photos)
                   .HasForeignKey(p => p.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Post)
                   .WithMany(post => post.Photos)
                   .HasForeignKey(p => p.PostId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.HelpRequest)
               .WithMany(h => h.Photos)
               .HasForeignKey(p => p.HelpRequestId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Donation)
                   .WithMany(d => d.Photos)
                   .HasForeignKey(p => p.DonationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

