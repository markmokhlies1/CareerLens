using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Salaries;
using CareerLens.Domain.Salaries.Enums;
using CareerLens.Domain.DomainUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerLens.Infrastructure.Data.Configurations
{
    public sealed class SalaryConfiguration : IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> builder)
        {
            builder.ToTable("Salaries");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .ValueGeneratedNever();

            builder.HasOne(s => s.Company)
                   .WithMany()
                   .HasForeignKey(s => s.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.UserId)
                   .IsRequired();

            builder.Property(s => s.CompanyId)
                   .IsRequired();

            builder.Property(s => s.JobTitle)
                   .IsRequired()
                   .HasMaxLength(CareerLensConstants.SalaryJobTitleLenght);

            builder.Property(s => s.Location)
                   .IsRequired()
                   .HasMaxLength(CareerLensConstants.SalaryLocationLength);

            builder.Property(s => s.Year)
                   .IsRequired();

            builder.Property(s => s.EmployeeType)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.EmploymentStatus)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.LengthOfEmployment)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(s => s.SalaryPeriod)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired()
                   .HasDefaultValue(SalaryStatus.Pending);

            builder.OwnsOne(s => s.BasePay, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("BasePayAmount")
                     .HasPrecision(18, 2)
                     .IsRequired();

                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                            .HasColumnName("BasePayCurrency")
                            .HasMaxLength(3)
                            .IsRequired();
                });
            });


            builder.OwnsOne(s => s.Bonus, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("BonusAmount")
                     .HasPrecision(18, 2)
                     .IsRequired(false);

                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                            .HasColumnName("BonusCurrency")
                            .HasMaxLength(3)
                            .IsRequired(false);
                });
            });

            builder.OwnsOne(s => s.Stock, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("StockAmount")
                     .HasPrecision(18, 2)
                     .IsRequired(false);

                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                            .HasColumnName("StockCurrency")
                            .HasMaxLength(3)
                            .IsRequired(false);
                });
            });

            builder.OwnsOne(s => s.ProfitSharing, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("ProfitSharingAmount")
                     .HasPrecision(18, 2)
                     .IsRequired(false);

                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                            .HasColumnName("ProfitSharingCurrency")
                            .HasMaxLength(3)
                            .IsRequired(false);
                });
            });

            builder.OwnsOne(s => s.Tips, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("TipsAmount")
                     .HasPrecision(18, 2)
                     .IsRequired(false);

                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                            .HasColumnName("TipsCurrency")
                            .HasMaxLength(3)
                            .IsRequired(false);
                });
            });

            builder.OwnsOne(s => s.Commission, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("CommissionAmount")
                     .HasPrecision(18, 2)
                     .IsRequired(false);

                money.OwnsOne(m => m.Currency, currency =>
                {
                    currency.Property(c => c.Code)
                            .HasColumnName("CommissionCurrency")
                            .HasMaxLength(3)
                            .IsRequired(false);
                });
            });
        }
    }
}
