using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Salaries.Dtos;
using CareerLens.Application.Features.Salaries.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Salaries;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Salaries.Queries.Employer.GetSalaryById
{
    [RequireRole("Employer")]
    public sealed record GetSalaryByIdForEmployerQuery(Guid CompanyId, Guid SalaryId)
    : IRequest<Result<ISalaryResponse>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Moderator];
    }

    public sealed class GetSalaryByIdForEmployerQueryValidator
        : AbstractValidator<GetSalaryByIdForEmployerQuery>
    {
        public GetSalaryByIdForEmployerQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.SalaryId)
                .NotEmpty()
                .WithMessage("Salary ID is required.");
        }
    }

    public sealed class GetSalaryByIdForEmployerQueryHandler(IAppDbContext context)
        : IRequestHandler<GetSalaryByIdForEmployerQuery, Result<ISalaryResponse>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<ISalaryResponse>> Handle(GetSalaryByIdForEmployerQuery request, CancellationToken cancellationToken)
        {
            var salary = await _context.Salaries
                .Include(s => s.Company)
                .FirstOrDefaultAsync(s => s.Id == request.SalaryId, cancellationToken);

            if (salary is null)
            {
                return ApplicationErrors.SalaryNotFound;
            }

            return salary.ToEmployerDto();
        }
    }
}
