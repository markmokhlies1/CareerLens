using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.InterviewQuestions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.ToTable("Interviews");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                   .ValueGeneratedNever();

            builder.HasOne(i => i.User)
                   .WithMany()
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<InterviewQuestion>("_interviewQuestions")
                   .WithOne()
                   .HasForeignKey("InterviewId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(i => i.InterviewQuestions)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(i => i.UserId)
                   .IsRequired();

            builder.Property(i => i.CompanyId)
                   .IsRequired();

            builder.Property(i => i.JobTitle)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(i => i.Description)
                   .IsRequired()
                   .HasMaxLength(4000);

            builder.Property(i => i.Location)
                   .HasMaxLength(150);

            builder.Property(i => i.OverallExperience)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(i => i.InterviewDifficulty)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(i => i.GettingOffer)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(i => i.Source)
                   .HasConversion<string>()           
                   .HasMaxLength(30);

            builder.Property(i => i.HelpingLevel)
                   .HasConversion<string>()           
                   .HasMaxLength(20);

            builder.Property(i => i.InterviewStatus)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(i => i.Stages)
                   .HasConversion<string>()           
                   .HasMaxLength(30);

           
            builder.OwnsOne(i => i.Date, date =>
            {
                date.Property(d => d.Year)
                    .HasColumnName("InterviewYear")
                    .IsRequired(false);               

                date.Property(d => d.Month)
                    .HasColumnName("InterviewMonth")
                    .IsRequired();
            });

            
            builder.OwnsOne(i => i.Duration, dur =>
            {
                dur.Property(d => d.Value)
                   .HasColumnName("DurationValue")
                   .IsRequired(false);

                dur.Property(d => d.Unit)
                   .HasColumnName("DurationUnit")
                   .HasConversion<string>()           
                   .HasMaxLength(10)
                   .IsRequired(false);
            });
        }
    }
}
