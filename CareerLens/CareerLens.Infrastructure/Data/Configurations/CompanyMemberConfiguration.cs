using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.DomainUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
    {
        public void Configure(EntityTypeBuilder<CompanyMember> builder)
        {
           
            builder.ToTable("CompanyMembers");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Id)
                   .ValueGeneratedNever();

            builder.HasOne<Company>()
                   .WithMany()
                   .HasForeignKey(cm => cm.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(cm => cm.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(cm => cm.CompanyId)
                   .IsRequired();

            builder.Property(cm => cm.UserId)
                   .IsRequired();

            builder.Property(cm => cm.Role)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();
        }
    }
}
