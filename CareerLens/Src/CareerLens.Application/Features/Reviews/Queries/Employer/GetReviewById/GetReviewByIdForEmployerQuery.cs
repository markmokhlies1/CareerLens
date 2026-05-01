using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Reviews.Dtos;
using CareerLens.Application.Features.Reviews.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Reviews.Queries.Employer.GetReviewById
{
    [RequireRole("Employer")]
    public sealed record GetReviewByIdForEmployerQuery(Guid ReviewId, Guid CompanyId)
    : IRequest<Result<IReviewResponse>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Moderator];
    }

    public sealed class GetReviewByIdForEmployerQueryValidator
        : AbstractValidator<GetReviewByIdForEmployerQuery>
    {
        public GetReviewByIdForEmployerQueryValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty()
                .WithMessage("Review ID is required.");
        }
    }

    public sealed class GetReviewByIdForEmployerQueryHandler(IAppDbContext context)
        : IRequestHandler<GetReviewByIdForEmployerQuery, Result<IReviewResponse>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<IReviewResponse>> Handle(GetReviewByIdForEmployerQuery request, CancellationToken cancellationToken)
        {

            var review = await _context.Reviews
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (review is null)
            {
                return ApplicationErrors.ReviewNotFound;
            }

            return review.ToEmployerDto();
        }
    }
}
