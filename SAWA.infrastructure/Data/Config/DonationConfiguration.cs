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
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("Donations");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Description)
                   .HasMaxLength(1000);

            builder.Property(d => d.Amount)
                   .IsRequired();
            builder.Property(d => d.Address)
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(d => d.Phone)
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.HasOne(d => d.User)
               .WithMany(u => u.DonationsGiven)
               .HasForeignKey(d => d.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Charity)
                   .WithMany(u => u.DonationsReceived)
                   .HasForeignKey(d => d.CharityId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(d => d.Photos)
                    .WithOne()
                    .HasForeignKey(p => p.DonationId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
