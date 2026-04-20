using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Jobs.Dtos;
using CareerLens.Application.Features.Jobs.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Jobs.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Jobs.Queries.Employee.GetCompanyJobs
{
    [RequireRole("Employee")]
    public sealed record GetCompanyJobsForEmployeeQuery(Guid CompanyId, int Page, int PageSize) 
        : IRequest<Result<PaginatedList<IJobResponse>>>;
    public sealed class GetCompanyJobsForEmployeeQueryValidator : AbstractValidator<GetCompanyJobsForEmployeeQuery>
    {
        public GetCompanyJobsForEmployeeQueryValidator()
        {
             RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Job ID is required.");
        }
    }
    public sealed class GetCompanyJobsForEmployeeQueryHandler (IAppDbContext context)
        : IRequestHandler<GetCompanyJobsForEmployeeQuery, Result<PaginatedList<IJobResponse>>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<PaginatedList<IJobResponse>>> Handle(GetCompanyJobsForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var baseQuery = _context.Jobs
                .AsNoTracking()
                .Where(j => j.CompanyId == request.CompanyId
                         && j.Status == JobStatus.Published);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PaginatedList<IJobResponse>
                {
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Items = []
                };
            }

            var jobs = await baseQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(j => j.Company)
                .ToListAsync(cancellationToken);

            var items = jobs
                .Select(j => (IJobResponse)j.ToBasicDto())
                .ToList()
                .AsReadOnly();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedList<IJobResponse>
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
