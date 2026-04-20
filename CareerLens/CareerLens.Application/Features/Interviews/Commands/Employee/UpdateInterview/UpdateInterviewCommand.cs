using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.Enums;
using CareerLens.Domain.Interviews.InterviewQuestions;
using CareerLens.Domain.DomainUsers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CareerLens.Application.Features.Interviews.Commands.Employee.UpdateInterview.UpdateInterviewCommandValidator;

namespace CareerLens.Application.Features.Interviews.Commands.Employee.UpdateInterview
{
    [RequireRole("Employee")]
    public sealed record UpdateInterviewCommand(Guid InterviewId,
                                                OverallExperience OverallExperience,
                                                InterviewDifficulty InterviewDifficulty,
                                                GettingOffer GettingOffer,
                                                string JobTitle,
                                                string Description,
                                                InterviewSource? Source,
                                                HelpingLevel? HelpingLevel,
                                                List<UpdateInterviewQuestionCommand> Questions,
                                                string? Location = null,
                                                InterviewDuration? Duration = null,
                                                InterviewDate? Date = null,
                                                InterviewStage? Stages = null) 
        : IRequest<Result<Updated>>;

    public sealed class UpdateInterviewCommandValidator : AbstractValidator<UpdateInterviewCommand>
    {
        public UpdateInterviewCommandValidator()
        {
            RuleFor(x => x.InterviewId)
                .NotEmpty()
                .WithMessage("Interview Id is required.");

            RuleFor(x => x.JobTitle)
                .NotEmpty()
                .WithMessage("Job title is required.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .Must(d => Helper.CountWords(d) >= CareerLensConstants.InterviewDescriptionLength)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage($"Description must be at least {CareerLensConstants.InterviewDescriptionLength} words.");

            RuleFor(x => x.OverallExperience)
                .IsInEnum()
                .WithMessage("Invalid overall experience value.");

            RuleFor(x => x.InterviewDifficulty)
                .IsInEnum()
                .WithMessage("Invalid interview difficulty value.");

            RuleFor(x => x.GettingOffer)
                .IsInEnum()
                .WithMessage("Invalid getting offer value.");

            RuleFor(x => x.Source)
                .IsInEnum()
                .When(x => x.Source.HasValue)
                .WithMessage("Invalid source value.");

            RuleFor(x => x.HelpingLevel)
                .IsInEnum()
                .When(x => x.HelpingLevel.HasValue)
                .WithMessage("Invalid helping level value.");

            RuleFor(x => x.Questions)
                .NotEmpty()
                .WithMessage("At least one question is required.");

            RuleForEach(x => x.Questions)
                .SetValidator(new UpdateInterviewQuestionCommandValidator());
        }
    }
    public sealed record UpdateInterviewQuestionCommand(Guid QuestionId, string QuestionText, string? Answer) : IRequest<Result<Updated>>;
    public sealed class UpdateInterviewQuestionCommandValidator : AbstractValidator<UpdateInterviewQuestionCommand>
    {
        public UpdateInterviewQuestionCommandValidator()
        {
            RuleFor(x => x.QuestionId)
           .NotEmpty()
           .WithMessage("QuestionId is required.");

            RuleFor(x => x.QuestionText)
           .NotEmpty()
           .WithMessage("Question text is required.");
        }
    }

    public sealed class UpdateInterviewCommandHandler (IAppDbContext context, IUser currentUser)
        : IRequestHandler<UpdateInterviewCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;
        public async Task<Result<Updated>> Handle(UpdateInterviewCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var interview = await _context.Interviews
                .Include(i => i.InterviewQuestions)
                .FirstOrDefaultAsync(i => i.Id == request.InterviewId, cancellationToken);

            if (interview is null)
            {
                return ApplicationErrors.InterviewNotFound;
            }

            if (interview.UserId != userId)
                return ApplicationErrors.NotInterviewOwner;

            var questions = new List<InterviewQuestion>();

            foreach (var q in request.Questions)
            {
                var questionResult = InterviewQuestion.Create(
                     q.QuestionId,
                     q.QuestionText,
                     q.Answer
                );

                if (questionResult.IsError)
                {
                    return questionResult.Errors;
                }

                questions.Add(questionResult.Value);
            }

            var updateResult = interview.Update(
                overallExperience: request.OverallExperience,
                interviewDifficulty: request.InterviewDifficulty,
                gettingOffer: request.GettingOffer,
                jobTitle: request.JobTitle,
                description: request.Description,
                source: request.Source,
                helpingLevel: request.HelpingLevel,
                interviewQuestions: questions,
                location: request.Location,
                duration: request.Duration,
                date: request.Date,
                stages: request.Stages
            );

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
