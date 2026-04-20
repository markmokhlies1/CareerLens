using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class CompanyClaimRequestConfiguration : IEntityTypeConfiguration<CompanyClaimRequest>
    {
        public void Configure(EntityTypeBuilder<CompanyClaimRequest> builder)
        {
            builder.ToTable("CompanyClaimRequests");

            builder.HasKey(ccr => ccr.Id);

            builder.Property(ccr => ccr.Id)
                   .ValueGeneratedNever();

            builder.HasOne(ccr => ccr.Company)
                   .WithMany()
                   .HasForeignKey(ccr => ccr.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ccr => ccr.User)
                   .WithMany()
                   .HasForeignKey(ccr => ccr.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ccr => ccr.CompanyId)
                   .IsRequired();

            builder.Property(ccr => ccr.UserId)
                   .IsRequired();

            builder.Property(ccr => ccr.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired()
                   .HasDefaultValue(ClaimStatus.Pending);

            builder.Property(ccr => ccr.CompanyMemberRole)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(ccr => ccr.AdminNote)
                   .IsRequired()
                   .HasMaxLength(500);

        }
    }
}
