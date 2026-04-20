using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Jobs.Dtos;
using CareerLens.Application.Features.Jobs.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Jobs.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Jobs.Queries.Employer.GetCompanyJobs
{
    [RequireRole("Employer")]
    public sealed record GetCompanyJobsForEmployerQuery(Guid CompanyId,
                                                        int Page,
                                                        int PageSize,
                                                        JobStatus? Status = null)
    : IRequest<Result<PaginatedList<IJobResponse>>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Hr];
    }

    public sealed class GetCompanyJobsForEmployerQueryValidator
    : AbstractValidator<GetCompanyJobsForEmployerQuery>
    {
        public GetCompanyJobsForEmployerQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Invalid job status.");
        }
    }

    public sealed class GetCompanyJobsForEmployerQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCompanyJobsForEmployerQuery, Result<PaginatedList<IJobResponse>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<PaginatedList<IJobResponse>>> Handle(GetCompanyJobsForEmployerQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _context.Jobs
                .Where(j => j.CompanyId == request.CompanyId);

            baseQuery = request.Status.HasValue
                ? baseQuery.Where(j => j.Status == request.Status.Value)
                : baseQuery.Where(j => j.Status == JobStatus.Published);  

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
                .Select(j => (IJobResponse)j.ToEmployerDto())
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
