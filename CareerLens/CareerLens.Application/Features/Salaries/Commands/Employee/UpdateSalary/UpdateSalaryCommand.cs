using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Salaries.Commands.Employee.CreateSalary;
using CareerLens.Application.Features.Salaries.Dtos;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.Salaries;
using CareerLens.Domain.Salaries.Enums;
using CareerLens.Domain.DomainUsers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Salaries.Commands.Employee.UpdateSalary
{
    [RequireRole("Employee")]
    public sealed record UpdateSalaryCommand(Guid SalaryId,
                                             string JobTitle,
                                             EmployeeType EmployeeType,
                                             EmploymentStatus EmploymentStatus,
                                             LengthOfEmployment LengthOfEmployment,
                                             string Location,
                                             MoneyDto BasePay,
                                             SalaryPeriod SalaryPeriod,
                                             int Year,
                                             MoneyDto? Bonus,
                                             MoneyDto? Stock,
                                             MoneyDto? ProfitSharing,
                                             MoneyDto? Tips,
                                             MoneyDto? Commission)
    : IRequest<Result<Updated>>;
    public sealed class UpdateSalaryCommandValidator : AbstractValidator<UpdateSalaryCommand>
    {
        public UpdateSalaryCommandValidator()
        {
            RuleFor(x => x.SalaryId)
                .NotEmpty()
                .WithMessage("Salary ID is required.");

            RuleFor(x => x.JobTitle)
                .NotEmpty()
                .WithMessage("Job title is required.")
                .MaximumLength(CareerLensConstants.SalaryJobTitleLenght)
                .WithMessage($"Job title must not exceed {CareerLensConstants.SalaryJobTitleLenght} characters.");

            RuleFor(x => x.EmployeeType)
                .IsInEnum()
                .WithMessage("Invalid employee type.");

            RuleFor(x => x.EmploymentStatus)
                .IsInEnum()
                .WithMessage("Invalid employment status.");

            RuleFor(x => x.LengthOfEmployment)
                .IsInEnum()
                .WithMessage("Invalid length of employment.");

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage("Location is required.")
                .MaximumLength(CareerLensConstants.SalaryLocationLength)
                .WithMessage($"Location must not exceed {CareerLensConstants.SalaryLocationLength} characters.");

            RuleFor(x => x.SalaryPeriod)
                .IsInEnum()
                .WithMessage("Invalid salary period.");

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, DateTime.UtcNow.Year + 1)
                .WithMessage($"Year must be between 2000 and {DateTime.UtcNow.Year + 1}.");

            RuleFor(x => x.BasePay)
                .NotNull()
                .WithMessage("Base pay is required.")
                .SetValidator(new CreateMoneyCommandValidator()!);

            RuleFor(x => x.BasePay.Amount)
                .GreaterThanOrEqualTo(CareerLensConstants.SalaryBasePayAmountForYear)
                .When(x => x.SalaryPeriod == SalaryPeriod.Yearly && x.BasePay is not null)
                .WithMessage($"Yearly base pay must be at least {CareerLensConstants.SalaryBasePayAmountForYear}.");

            RuleFor(x => x.BasePay.Amount)
                .GreaterThanOrEqualTo(CareerLensConstants.SalaryBasePayAmountForMonth)
                .When(x => x.SalaryPeriod == SalaryPeriod.Monthly && x.BasePay is not null)
                .WithMessage($"Monthly base pay must be at least {CareerLensConstants.SalaryBasePayAmountForMonth}.");

            RuleFor(x => x.Bonus)
                .SetValidator(new NullableCreateMoneyCommandValidator()!);

            RuleFor(x => x.Stock)
                .SetValidator(new NullableCreateMoneyCommandValidator()!);

            RuleFor(x => x.ProfitSharing)
                .SetValidator(new NullableCreateMoneyCommandValidator()!);

            RuleFor(x => x.Tips)
                .SetValidator(new NullableCreateMoneyCommandValidator()!);

            RuleFor(x => x.Commission)
                .SetValidator(new NullableCreateMoneyCommandValidator()!);
        }
    }

    public sealed class UpdateSalaryCommandHandler(IAppDbContext context, IUser currentUser)
    : IRequestHandler<UpdateSalaryCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<Updated>> Handle(UpdateSalaryCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var salary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.Id == request.SalaryId, cancellationToken);

            if (salary is null)
            {
                return ApplicationErrors.SalaryNotFound;
            }

            if (salary.UserId != userId)
            {
                return ApplicationErrors.NotSalaryOwner;
            }

            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == salary.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var basePayResult = Money.Create(request.BasePay.Amount, request.BasePay.Currency);
            if (basePayResult.IsError) return basePayResult.Errors;

            var bonusResult = CreateOptionalMoney(request.Bonus);
            if (bonusResult.IsError) return bonusResult.Errors;

            var stockResult = CreateOptionalMoney(request.Stock);
            if (stockResult.IsError) return stockResult.Errors;

            var profitSharingResult = CreateOptionalMoney(request.ProfitSharing);
            if (profitSharingResult.IsError) return profitSharingResult.Errors;

            var tipsResult = CreateOptionalMoney(request.Tips);
            if (tipsResult.IsError) return tipsResult.Errors;

            var commissionResult = CreateOptionalMoney(request.Commission);
            if (commissionResult.IsError) return commissionResult.Errors;

            var updateResult = salary.Update(
                jobTitle: request.JobTitle,
                employeeType: request.EmployeeType,
                employmentStatus: request.EmploymentStatus,
                lengthOfEmployment: request.LengthOfEmployment,
                location: request.Location,
                basePay: basePayResult.Value,
                salaryPeriod: request.SalaryPeriod,
                year: request.Year,
                bonus: bonusResult.Value,
                stock: stockResult.Value,
                profitSharing: profitSharingResult.Value,
                tips: tipsResult.Value,
                commission: commissionResult.Value
            );

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }

        private static Result<Money?> CreateOptionalMoney(MoneyDto? moneyDto)
        {
            if (moneyDto is null)
            {
                return (Money?)null;
            }

            var result = Money.Create(moneyDto.Amount, moneyDto.Currency);

            if (result.IsError)
            {
                return result.Errors;
            }

            return (Money?)result.Value;
        }
    }
}
