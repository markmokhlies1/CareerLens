using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Reviews;
using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.Reviews.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Reviews.Commands.Employer.UpdateReviewState
{
    [RequireRole("Employer")]
    public sealed record UpdateReviewStatusCommand(Guid ReviewId, Guid CompanyId, ReviewStatus Status)
    : IRequest<Result<Updated>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Moderator];
    }

    public sealed class UpdateReviewStatusCommandValidator
        : AbstractValidator<UpdateReviewStatusCommand>
    {
        private static readonly ReviewStatus[] AllowedStatuses =
        [
            ReviewStatus.Approved,
            ReviewStatus.Rejected
        ];

        public UpdateReviewStatusCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty()
                .WithMessage("Review ID is required.");

            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid review status.")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage("Status must be Approved or Rejected.");
        }
    }

    public sealed class UpdateReviewStatusCommandHandler(IAppDbContext context)
        : IRequestHandler<UpdateReviewStatusCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Updated>> Handle(UpdateReviewStatusCommand request, CancellationToken cancellationToken)
        {
            
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId
                                       && r.CompanyId == request.CompanyId,
                                     cancellationToken);

            if (review is null)
            {
                return ApplicationErrors.ReviewNotFound;
            }

            var result = request.Status == ReviewStatus.Approved
                ? review.Approve()
                : review.Reject();

            if (result.IsError)
            {
                return result.Errors;
            }

            if (request.Status == ReviewStatus.Approved)
            {
                review.AddDomainEvent(new ReviewApproved(review.Id, review.UserId, review.Headline!));
            }

            if (request.Status == ReviewStatus.Rejected)
            {
                review.AddDomainEvent(new ReviewRejected(review.Id, review.UserId, review.Headline!));
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
