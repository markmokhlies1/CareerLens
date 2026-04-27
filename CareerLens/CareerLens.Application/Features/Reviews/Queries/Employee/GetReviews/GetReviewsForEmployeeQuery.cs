using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Reviews.Dtos;
using CareerLens.Application.Features.Reviews.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Reviews.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Reviews.Queries.Employee.GetReviews
{
    public sealed record GetReviewsForEmployeeQuery(Guid CompanyId,
                                                    int Page,
                                                    int PageSize)
    : IRequest<Result<PaginatedList<IReviewResponse>>>;

    public sealed class GetReviewsForEmployeeQueryValidator
        : AbstractValidator<GetReviewsForEmployeeQuery>
    {
        public GetReviewsForEmployeeQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("PageSize must be between 1 and 50.");
        }
    }

    public sealed class GetReviewsForEmployeeQueryHandler(IAppDbContext context)
        : IRequestHandler<GetReviewsForEmployeeQuery, Result<PaginatedList<IReviewResponse>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<PaginatedList<IReviewResponse>>> Handle(GetReviewsForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var baseQuery = _context.Reviews
                .Where(r => r.CompanyId == request.CompanyId
                         && r.Status == ReviewStatus.Approved);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PaginatedList<IReviewResponse>
                {
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Items = []
                };
            }

            var reviews = await baseQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(r => r.Company)
                .ToListAsync(cancellationToken);

            var items = reviews
                .Select(r => (IReviewResponse)r.ToBasicDto())
                .ToList()
                .AsReadOnly();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedList<IReviewResponse>
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
