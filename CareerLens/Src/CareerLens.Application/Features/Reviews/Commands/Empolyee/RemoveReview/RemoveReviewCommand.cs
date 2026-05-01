using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Reviews;
using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.DomainUsers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Reviews.Commands.Employee.RemoveReview
{
    [RequireRole("Employee")]
    public sealed record RemoveReviewCommand(Guid ReviewId)
    : IRequest<Result<Deleted>>;

    public sealed class RemoveReviewCommandValidator : AbstractValidator<RemoveReviewCommand>
    {
        public RemoveReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty()
                .WithMessage("Review ID is required.");
        }
    }

    public sealed class RemoveReviewCommandHandler(IAppDbContext context, IUser currentUser)
        : IRequestHandler<RemoveReviewCommand, Result<Deleted>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;
        public async Task<Result<Deleted>> Handle(RemoveReviewCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (review is null)
            {
                return ApplicationErrors.ReviewNotFound;
            }

            if (review.UserId != userId)
            {
                return ApplicationErrors.NotReviewOwner;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Deleted;
        }
    }
}
