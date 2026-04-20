using CareerLens.Domain.Notifications;
using CareerLens.Domain.DomainUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {

            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                   .ValueGeneratedNever();

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(n => n.UserId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.Property(n => n.UserId)
                   .IsRequired();

            builder.Property(n => n.Title)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(n => n.ReferenceId)
                   .IsRequired(false)
                   .HasMaxLength(100);

            builder.Property(n => n.ReferenceType)
                   .IsRequired(false)
                   .HasMaxLength(50);

            builder.Property(n => n.IsRead)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(n => n.Type)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

        }
    }
}
