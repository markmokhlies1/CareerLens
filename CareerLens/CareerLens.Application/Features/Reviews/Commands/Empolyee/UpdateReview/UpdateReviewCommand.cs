using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
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

namespace CareerLens.Application.Features.Reviews.Commands.Employee.UpdateReview
{
    [RequireRole("Employee")]
    public sealed record UpdateReviewCommand(Guid ReviewId,
                                             int OverallRating,
                                             EmploymentStatus EmploymentStatus,
                                             JobFunction JobFunction,
                                             LengthOfEmployment LengthOfEmployment,
                                             string Headline,
                                             string Pros,
                                             string Cons,
                                             EmployeeType EmployeeType,
                                             string? JobTitle = null,
                                             string? AdviceToManagement = null,
                                             int? CareerOpportunities = null,
                                             int? CompensationAndBenefits = null,
                                             int? CultureAndValues = null,
                                             int? DiversityAndInclusion = null,
                                             int? SeniorManagement = null,
                                             int? WorkLifeBalance = null,
                                             Sentiment? CeoRating = null,
                                             Sentiment? RecommendToFriend = null,
                                             Sentiment? BusinessOutlook = null,
                                             string? Location = null)
    : IRequest<Result<Updated>>;

    public sealed class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty()
                .WithMessage("Review ID is required.");

            RuleFor(x => x.OverallRating)
                .InclusiveBetween(1, 5)
                .WithMessage("Overall rating must be between 1 and 5.");

            RuleFor(x => x.EmploymentStatus)
                .IsInEnum()
                .WithMessage("Invalid employment status.");

            RuleFor(x => x.JobFunction)
                .IsInEnum()
                .WithMessage("Invalid job function.");

            RuleFor(x => x.LengthOfEmployment)
                .IsInEnum()
                .WithMessage("Invalid length of employment.");

            RuleFor(x => x.EmployeeType)
                .IsInEnum()
                .WithMessage("Invalid employee type.");

            RuleFor(x => x.Headline)
                .NotEmpty()
                .WithMessage("Headline is required.");

            RuleFor(x => x.Pros)
                .NotEmpty()
                .WithMessage("Pros is required.")
                .Must(p => Helper.CountWords(p) >= CareerLensConstants.InterviewReviewPronsLength)
                .When(x => !string.IsNullOrWhiteSpace(x.Pros))
                .WithMessage($"Pros must be at least {CareerLensConstants.InterviewReviewPronsLength} words.");

            RuleFor(x => x.Cons)
                .NotEmpty()
                .WithMessage("Cons is required.")
                .Must(c => Helper.CountWords(c) >= CareerLensConstants.InterviewReviewConsLength)
                .When(x => !string.IsNullOrWhiteSpace(x.Cons))
                .WithMessage($"Cons must be at least {CareerLensConstants.InterviewReviewConsLength} words.");

            RuleFor(x => x.CareerOpportunities)
                .InclusiveBetween(1, 5)
                .When(x => x.CareerOpportunities.HasValue)
                .WithMessage("Career opportunities must be between 1 and 5.");

            RuleFor(x => x.CompensationAndBenefits)
                .InclusiveBetween(1, 5)
                .When(x => x.CompensationAndBenefits.HasValue)
                .WithMessage("Compensation and benefits must be between 1 and 5.");

            RuleFor(x => x.CultureAndValues)
                .InclusiveBetween(1, 5)
                .When(x => x.CultureAndValues.HasValue)
                .WithMessage("Culture and values must be between 1 and 5.");

            RuleFor(x => x.DiversityAndInclusion)
                .InclusiveBetween(1, 5)
                .When(x => x.DiversityAndInclusion.HasValue)
                .WithMessage("Diversity and inclusion must be between 1 and 5.");

            RuleFor(x => x.SeniorManagement)
                .InclusiveBetween(1, 5)
                .When(x => x.SeniorManagement.HasValue)
                .WithMessage("Senior management must be between 1 and 5.");

            RuleFor(x => x.WorkLifeBalance)
                .InclusiveBetween(1, 5)
                .When(x => x.WorkLifeBalance.HasValue)
                .WithMessage("Work life balance must be between 1 and 5.");

            RuleFor(x => x.CeoRating)
                .IsInEnum()
                .When(x => x.CeoRating.HasValue)
                .WithMessage("Invalid CEO rating.");

            RuleFor(x => x.RecommendToFriend)
                .IsInEnum()
                .When(x => x.RecommendToFriend.HasValue)
                .WithMessage("Invalid recommend to friend.");

            RuleFor(x => x.BusinessOutlook)
                .IsInEnum()
                .When(x => x.BusinessOutlook.HasValue)
                .WithMessage("Invalid business outlook.");
        }
    }

    public sealed class UpdateReviewCommandHandler(IAppDbContext context, IUser currentUser)
    : IRequestHandler<UpdateReviewCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<Updated>> Handle(UpdateReviewCommand request,CancellationToken cancellationToken)
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

            var updateResult = review.Update(overallRating: request.OverallRating,
                                             employmentStatus: request.EmploymentStatus,
                                             jobFunction: request.JobFunction,
                                             lengthOfEmployment: request.LengthOfEmployment,
                                             headline: request.Headline,
                                             pros: request.Pros,
                                             cons: request.Cons,
                                             employeeType: request.EmployeeType,
                                             jobTitle: request.JobTitle,
                                             adviceToManagement: request.AdviceToManagement,
                                             careerOpportunities: request.CareerOpportunities,
                                             compensationAndBenefits: request.CompensationAndBenefits,
                                             cultureAndValues: request.CultureAndValues,
                                             diversityAndInclusion: request.DiversityAndInclusion,
                                             seniorManagement: request.SeniorManagement,
                                             workLifeBalance: request.WorkLifeBalance,
                                             ceoRating: request.CeoRating,
                                             recommendToFriend: request.RecommendToFriend,
                                             businessOutlook: request.BusinessOutlook,
                                             location: request.Location);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
