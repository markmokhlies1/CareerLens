using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
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

namespace CareerLens.Application.Features.Salaries.Commands.Employee.RemoveSalary
{
    [RequireRole("Employee")]
    public sealed record RemoveSalaryCommand(Guid SalaryId, Guid CompanyId) : IRequest<Result<Deleted>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Moderator];
    }

    public sealed class RemoveSalaryCommandValidator : AbstractValidator<RemoveSalaryCommand>
    {
        public RemoveSalaryCommandValidator()
        {
            RuleFor(x => x.SalaryId)
                .NotEmpty()
                .WithMessage("Salary ID is required.");
        }
    }

    public sealed class RemoveSalaryCommandHandler(IAppDbContext context, IUser currentUser)
    : IRequestHandler<RemoveSalaryCommand, Result<Deleted>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;


        public async Task<Result<Deleted>> Handle(RemoveSalaryCommand request,CancellationToken cancellationToken)
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

            if (salary.Id != userId)
            {
                return ApplicationErrors.NotSalaryOwner;
            }

            _context.Salaries.Remove(salary);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Deleted;
        }
    }
}
