using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Companies.Dtos;
using CareerLens.Application.Features.Interviews.Dtos;
using CareerLens.Application.Features.Interviews.Mappers;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.Enums;
using CareerLens.Domain.Interviews.Events;
using CareerLens.Domain.Interviews.InterviewQuestions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.Commands.Employee.CreateInterview
{
    [RequireRole("Employee")]
    public sealed record CreateInterviewCommand(Guid CompanyId,
                                                OverallExperience OverallExperience,
                                                InterviewDifficulty InterviewDifficulty,
                                                GettingOffer GettingOffer,
                                                string JobTitle,
                                                string Description,
                                                InterviewSource? Source,
                                                HelpingLevel? HelpingLevel,
                                                string? Location,
                                                InterviewDuration? Duration,
                                                InterviewDate? Date,
                                                InterviewStage? Stages,
                                                List<CreateInterviewQuestionCommand> Questions)
                                                    : IRequest<Result<IInterviewResponse>>;

    public sealed record CreateInterviewQuestionCommand(string QuestionText, string? Answer) : IRequest<Result<InterviewQuestionDto>>;

    public sealed class CreateInterviewCommandValidator : AbstractValidator<CreateInterviewCommand>
    {
        public CreateInterviewCommandValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company is required.");

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
                .SetValidator(new CreateInterviewQuestionValidator());
        }
    }

    public sealed class CreateInterviewQuestionValidator : AbstractValidator<CreateInterviewQuestionCommand>
    {
        public CreateInterviewQuestionValidator()
        {
            RuleFor(x => x.QuestionText)
                .NotEmpty()
                .WithMessage("Question text is required.");
        }
    }

    public sealed class CreateInterviewCommandHandler(IAppDbContext context,IUser currentUser) 
        : IRequestHandler<CreateInterviewCommand, Result<IInterviewResponse>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;
        public async Task<Result<IInterviewResponse>> Handle(CreateInterviewCommand request, CancellationToken cancellationToken)
        {

            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var questions = new List<InterviewQuestion>();

            foreach (var q in request.Questions)
            {
                var questionResult = InterviewQuestion.Create(Guid.NewGuid(), q.QuestionText, q.Answer);

                if (questionResult.IsError)
                {
                    return questionResult.Errors;
                }

                questions.Add(questionResult.Value);
            }

            var interviewResult = Interview.Create(
                id: Guid.NewGuid(),
                userId: userId,
                companyId: request.CompanyId,
                overallExperience: request.OverallExperience,
                interviewDifficulty: request.InterviewDifficulty,
                gettingOffer: request.GettingOffer,
                source: request.Source,
                helpingLevel: request.HelpingLevel,
                interviewQuestions: questions,
                jobTitle: request.JobTitle,
                description: request.Description,
                location: request.Location,
                duration: request.Duration,
                date: request.Date,
                stages: request.Stages
            );

            if (interviewResult.IsError)
            {
                return interviewResult.Errors;
            }

            var interview = interviewResult.Value;

            await _context.Interviews.AddAsync(interview, cancellationToken);

            interview.AddDomainEvent(new InterviewCreated(interview.Id, interview.CompanyId, interview.UserId, interview.JobTitle!));

            await _context.SaveChangesAsync(cancellationToken);

            return interview.ToBasicDto();
        }
    }
}
