using CareerLens.Domain.Reviews;
using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.DomainUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                   .ValueGeneratedNever();

            builder.HasOne(r => r.Company)
                   .WithMany()
                   .HasForeignKey(r => r.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(r => r.UserId)
                   .IsRequired();

            builder.Property(r => r.CompanyId)
                   .IsRequired();

            builder.Property(r => r.Headline)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(r => r.Pros)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(r => r.Cons)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(r => r.JobTitle)
                   .IsRequired(false)
                   .HasMaxLength(100);

            builder.Property(r => r.AdviceToManagement)
                   .IsRequired(false)
                   .HasMaxLength(2000);

            builder.Property(r => r.Location)
                   .IsRequired(false)
                   .HasMaxLength(150);

            builder.Property(r => r.OverallRating)
                   .IsRequired();

            builder.Property(r => r.CareerOpportunities)
                   .IsRequired(false);

            builder.Property(r => r.CompensationAndBenefits)
                   .IsRequired(false);

            builder.Property(r => r.CultureAndValues)
                   .IsRequired(false);

            builder.Property(r => r.DiversityAndInclusion)
                   .IsRequired(false);

            builder.Property(r => r.SeniorManagement)
                   .IsRequired(false);

            builder.Property(r => r.WorkLifeBalance)
                   .IsRequired(false);

            builder.Property(r => r.EmploymentStatus)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(r => r.EmployeeType)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(r => r.JobFunction)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(r => r.LengthOfEmployment)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired(false);

            builder.Property(r => r.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired()
                   .HasDefaultValue(ReviewStatus.Pending);

            builder.Property(r => r.CeoRating)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(r => r.RecommendToFriend)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(r => r.BusinessOutlook)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired(false);
        }
    }
}
