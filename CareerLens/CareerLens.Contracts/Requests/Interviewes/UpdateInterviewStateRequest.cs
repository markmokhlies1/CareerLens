using CareerLens.Domain.Interviews.Enums;
using FluentValidation;

namespace CareerLens.Contracts.Requests.Interviewes
{
    public sealed class UpdateInterviewStateRequest
    {
        public InterviewStatus InterviewStatus { get; set; }
    }
    public sealed class UpdateInterviewStateRequestValidator
    : AbstractValidator<UpdateInterviewStateRequest>
    {
        public UpdateInterviewStateRequestValidator()
        {
            RuleFor(x => x.InterviewStatus)
                .IsInEnum()
                .WithMessage("Interview status is invalid.")
                .NotEqual(InterviewStatus.Pending)
                .WithMessage("Cannot set status back to Pending.");
        }
    }
}
