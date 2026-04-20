using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Reviews.Dtos;
using CareerLens.Application.Features.Reviews.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Reviews;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Reviews.Queries.Employee.GetReviewById
{
    [RequireRole("Employee")]
    public sealed record GetReviewByIdForEmployeeQuery(Guid ReviewId)
    : IRequest<Result<IReviewResponse>>;

    public sealed class GetReviewByIdForEmployeeQueryValidator
        : AbstractValidator<GetReviewByIdForEmployeeQuery>
    {
        public GetReviewByIdForEmployeeQueryValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty()
                .WithMessage("Review ID is required.");
        }
    }

    public sealed class GetReviewByIdForEmployeeQueryHandler(IAppDbContext context)
        : IRequestHandler<GetReviewByIdForEmployeeQuery, Result<IReviewResponse>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<IReviewResponse>> Handle(
            GetReviewByIdForEmployeeQuery request,
            CancellationToken cancellationToken)
        {
            var review = await _context.Reviews
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (review is null)
            {
                return ApplicationErrors.ReviewNotFound;
            }

            return review.ToBasicDto();
        }
    }


}
