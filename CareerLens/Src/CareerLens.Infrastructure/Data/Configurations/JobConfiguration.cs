using CareerLens.Domain.Jobs;
using CareerLens.Domain.Jobs.Enums;
using CareerLens.Domain.DomainUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            
            builder.ToTable("Jobs");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Id)
                   .ValueGeneratedNever();

            builder.HasOne(j => j.Company)
                   .WithMany()
                   .HasForeignKey(j => j.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(j => j.PostedByUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(j => j.CompanyId)
                   .IsRequired();

            builder.Property(j => j.PostedByUserId)
                   .IsRequired();

            builder.Property(j => j.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(j => j.Description)
                   .IsRequired()
                   .HasMaxLength(5000);

            builder.Property(j => j.Location)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(j => j.ApplyUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(j => j.MinSalary)
                   .HasPrecision(18, 2)
                   .IsRequired(false);

            builder.Property(j => j.MaxSalary)
                   .HasPrecision(18, 2)
                   .IsRequired(false);

            builder.Property(j => j.EmploymentType)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(j => j.WorkplaceType)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(j => j.ExperienceLevel)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(j => j.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired()
                   .HasDefaultValue(JobStatus.Draft);

            builder.Property(j => j.PayPeriod)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired(false);
        }
    }
}
