using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyMembers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Industry).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Location).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Website).IsRequired().HasMaxLength(500);

            builder.Property(c => c.Description)
                   .IsRequired()
                   .HasMaxLength(CareerLensConstants.CompanyDescriptionMaxLenght);

            builder.Property(c => c.FoundedYear).IsRequired();
            builder.Property(c => c.IsClaimed).IsRequired().HasDefaultValue(false);

            builder.Property(c => c.Size)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.HasMany(c => c.Members)              
                   .WithOne()
                   .HasForeignKey("CompanyId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Members)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(c => c.Requests)             
                   .WithOne()
                   .HasForeignKey("CompanyId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Requests)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
