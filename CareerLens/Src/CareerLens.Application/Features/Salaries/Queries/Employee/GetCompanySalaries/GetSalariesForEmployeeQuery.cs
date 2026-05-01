using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Salaries.Dtos;
using CareerLens.Application.Features.Salaries.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Salaries.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Salaries.Queries.Employee.GetCompanySalaries
{
    [RequireRole("Employee")]
    public sealed record GetSalariesForEmployeeQuery(Guid CompanyId, int Page, int PageSize)
        : IRequest<Result<PaginatedList<ISalaryResponse>>>;

    public sealed class GetSalariesForEmployeeQueryValidator : AbstractValidator<GetSalariesForEmployeeQuery>
    {
        public GetSalariesForEmployeeQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");
        }
    }

    public sealed class GetSalariesForEmployeeQueryHandler(IAppDbContext context)
        : IRequestHandler<GetSalariesForEmployeeQuery, Result<PaginatedList<ISalaryResponse>>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<PaginatedList<ISalaryResponse>>> Handle(GetSalariesForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var baseQuery = _context.Salaries
                .Where(s => s.CompanyId == request.CompanyId
                         && s.Status == SalaryStatus.Approved);

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
                .Select(s => (ISalaryResponse)s.ToBasicDto())
                .ToList()
                .AsReadOnly();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var result = new PaginatedList<ISalaryResponse>
            {
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            };

            return result;
        }
    }
}
