using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Behaviours;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.DomainUsers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.Commands.Employee.RemoveInterview
{
    [RequireRole("Employee")]
    public sealed record RemoveInterviewCommand(Guid InterviewId) : IRequest<Result<Deleted>>;
    public sealed class RemoveInterviewCommandValidator : AbstractValidator<RemoveInterviewCommand>
    {
        public RemoveInterviewCommandValidator() 
        {
            RuleFor(x => x.InterviewId)
                .NotEmpty()
                .WithMessage("Interview Id is required.");
        }
    }
    public sealed class RemoveInterviewCommandHandler 
        : IRequestHandler<RemoveInterviewCommand, Result<Deleted>>
    {
        private readonly IAppDbContext _context;
        private readonly IUser _currentUser;

        public RemoveInterviewCommandHandler(IAppDbContext context, IUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<Deleted>> Handle(RemoveInterviewCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();

            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == request.InterviewId, cancellationToken);

            if (interview is null)
                return ApplicationErrors.InterviewNotFound;

            if (interview.UserId != userIdResult.Value)
                return ApplicationErrors.NotInterviewOwner;

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Deleted;

        }
    }
}
