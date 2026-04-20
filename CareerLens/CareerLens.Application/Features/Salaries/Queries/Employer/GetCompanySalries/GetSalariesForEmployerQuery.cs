using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Salaries.Dtos;
using CareerLens.Application.Features.Salaries.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Salaries.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Salaries.Queries.Employer.GetCompanySalries
{
    [RequireRole("Employer")]
    public sealed record GetSalariesForEmployerQuery(Guid CompanyId, int Page, int PageSize, SalaryStatus? Status = null)
        : IRequest<Result<PaginatedList<ISalaryResponse>>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Moderator];
    }

    public sealed class GetSalariesForEmployerQueryValidator
        : AbstractValidator<GetSalariesForEmployerQuery>
    {
        public GetSalariesForEmployerQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Invalid salary status.");
        }
    }

    public sealed class GetSalariesForEmployerQueryHandler(IAppDbContext context)
        : IRequestHandler<GetSalariesForEmployerQuery, Result<PaginatedList<ISalaryResponse>>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<PaginatedList<ISalaryResponse>>> Handle(GetSalariesForEmployerQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _context.Salaries
                .Where(s => s.CompanyId == request.CompanyId);

            baseQuery = request.Status.HasValue
                ? baseQuery.Where(s => s.Status == request.Status.Value)
                : baseQuery.Where(s => s.Status == SalaryStatus.Pending
                                    || s.Status == SalaryStatus.Approved);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PaginatedList<ISalaryResponse>
                {
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Items = []
                };
            }

            var salaries = await baseQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(s => s.Company)
                .ToListAsync(cancellationToken);

            var items = salaries
                .Select(s => (ISalaryResponse)s.ToEmployerDto())
                .ToList()
                .AsReadOnly();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedList<ISalaryResponse>
            {
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            };
        }
    }
}
