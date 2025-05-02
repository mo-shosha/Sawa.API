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
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Description)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(b => b.Address)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.HasOne(b => b.Charity)
                   .WithMany(u => u.Branches)
                   .HasForeignKey(b => b.CharityId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
