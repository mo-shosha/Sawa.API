using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Data.Config
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUsers");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(u => u.Address)
                   .HasMaxLength(500);

            builder.HasOne(u => u.ProfilePhoto)
                   .WithOne()
                   .HasForeignKey<AppUser>(u => u.ProfilePhotoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.WallpaperPhoto)
                   .WithOne()
                   .HasForeignKey<AppUser>(u => u.WallpaperPhotoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.DonationsGiven)
                   .WithOne(d => d.User)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.DonationsReceived)
                   .WithOne(d => d.Charity)
                   .HasForeignKey(d => d.CharityId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(u => u.MyHelpRequests)
                    .WithOne(hr => hr.User)
                    .HasForeignKey(hr => hr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ProvidedHelpRequests)
                   .WithOne(hr => hr.Charity)
                   .HasForeignKey(hr => hr.CharityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Comments)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId);

            builder.HasMany(u => u.Reports)
                   .WithOne(r => r.Reporter)
                   .HasForeignKey(r => r.ReporterId);

            builder.HasMany(u => u.Branches)
                   .WithOne(b => b.Charity)
                   .HasForeignKey(b => b.CharityId);

            builder.HasMany(u => u.Posts)
                   .WithOne(p => p.Charity)
                   .HasForeignKey(p => p.CharityId);
        }
    }
}
