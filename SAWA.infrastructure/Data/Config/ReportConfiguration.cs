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
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.TargetId)
                   .IsRequired();

            builder.Property(r => r.Type)
                   .IsRequired();

            builder.HasOne(r => r.Reporter)
                   .WithMany(u => u.Reports)
                   .HasForeignKey(r => r.ReporterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
