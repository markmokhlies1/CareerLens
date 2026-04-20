using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Interviews.Enums;
using CareerLens.Domain.Interviews.Events;
using CareerLens.Domain.DomainUsers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.Commands.Employer.UpdateState
{
    [RequireRole("Employer")]
    public sealed record UpdateInterviewStateCommand(Guid InterviewId, Guid CompanyId, InterviewStatus InterviewStatus)
        : IRequest<Result<Updated>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles => [CompanyMemberRole.Moderator];
    }

    public sealed class UpdateInterviewStateCommandValidator : AbstractValidator<UpdateInterviewStateCommand>
    {
        public UpdateInterviewStateCommandValidator()
        {
            RuleFor(x => x.InterviewId)
            .NotEmpty()
            .WithMessage("InterviewId is required.");

            RuleFor(x => x.InterviewStatus)
                .IsInEnum()
                .WithMessage("InterviewStatus is invalid.")
                .WithMessage("Cannot set status back to Pending.");
        }
    }
    public sealed class UpdateInterviewStateCommandHandler(IAppDbContext context)
        : IRequestHandler<UpdateInterviewStateCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Updated>> Handle(UpdateInterviewStateCommand request, CancellationToken cancellationToken)
        {
            var interview = await _context.Interviews.FirstOrDefaultAsync(i => i.Id == request.InterviewId,cancellationToken);

            if (interview is null)
            {
                return ApplicationErrors.InterviewNotFound;
            }

            var updatedResult = interview.UpdateState(request.InterviewStatus);

            if (updatedResult.IsError)
            {
                return updatedResult.Errors;
            }

            if (request.InterviewStatus == InterviewStatus.Approved)
            {
                interview.AddDomainEvent(new InterviewApproved(interview.Id,interview.UserId,interview.JobTitle!));
            }

            if (request.InterviewStatus == InterviewStatus.Rejected)
            {
                interview.AddDomainEvent(new InterviewRejected(interview.Id,interview.UserId,interview.JobTitle!));
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
