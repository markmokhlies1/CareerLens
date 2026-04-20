using CareerLens.Domain.Interviews.InterviewQuestions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class InterviewQuestionConfiguration : IEntityTypeConfiguration<InterviewQuestion>
    {
        public void Configure(EntityTypeBuilder<InterviewQuestion> builder)
        {
            
            builder.ToTable("InterviewQuestions");

            builder.HasKey(iq => iq.Id);

            builder.Property(iq => iq.Id)
                   .ValueGeneratedNever();

            
            builder.Property(iq => iq.Question)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(iq => iq.Answer)
                   .IsRequired(false)
                   .HasMaxLength(500);

            builder.Property<Guid>("InterviewId")
                   .IsRequired();

        }
    }
}
