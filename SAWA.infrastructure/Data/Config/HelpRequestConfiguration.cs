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
    public class HelpRequestConfiguration : IEntityTypeConfiguration<HelpRequest>
    {
        public void Configure(EntityTypeBuilder<HelpRequest> builder)
        {
            builder.ToTable("HelpRequests");

            builder.HasKey(hr => hr.Id);

            builder.Property(hr => hr.Description)
                   .HasMaxLength(1000);

            builder.Property(hr => hr.Phone)
                   .HasMaxLength(50);

            builder.Property(hr => hr.Address)
                   .HasMaxLength(500);

            builder.HasOne(hr => hr.User)
                   .WithMany(u => u.MyHelpRequests)  
                   .HasForeignKey(hr => hr.UserId)
                   .OnDelete(DeleteBehavior.Restrict);  
            builder.HasOne(hr => hr.Charity)
                   .WithMany(c => c.ProvidedHelpRequests)   
                   .HasForeignKey(hr => hr.CharityId)
                   .OnDelete(DeleteBehavior.Restrict); 


            builder.HasMany(hr => hr.Photos)
                   .WithOne()
                   .HasForeignKey(p => p.HelpRequestId)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
