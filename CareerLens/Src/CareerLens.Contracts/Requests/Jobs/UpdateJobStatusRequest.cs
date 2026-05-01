using CareerLens.Domain.Jobs.Enums;
using FluentValidation;

namespace CareerLens.Contracts.Requests.Jobs
{
    public sealed class UpdateJobStatusRequest
    {
        public JobStatus Status { get; set; }
    }

    public sealed class UpdateJobStatusRequestValidator : AbstractValidator<UpdateJobStatusRequest>
    {
        public UpdateJobStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid job status.")
                .NotEqual(JobStatus.Draft)
                .WithMessage("Cannot set status back to Draft directly. Update the job instead.");
        }
    }
}
