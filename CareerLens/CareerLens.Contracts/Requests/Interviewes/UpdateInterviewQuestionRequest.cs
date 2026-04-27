using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Contracts.Requests.Interviewes
{
    public class UpdateInterviewQuestionRequest
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? Answer { get; set; }
    }
    public sealed class UpdateInterviewQuestionRequestValidator
    : AbstractValidator<UpdateInterviewQuestionRequest>
    {
        public UpdateInterviewQuestionRequestValidator()
        {
            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .WithMessage("Question Id is required.");

            RuleFor(x => x.QuestionText)
                .NotEmpty()
                .WithMessage("Question text is required.")
                .MaximumLength(500)
                .WithMessage("Question text must not exceed 500 characters.");

            RuleFor(x => x.Answer)
                .MaximumLength(1000)
                .WithMessage("Answer must not exceed 1000 characters.")
                .When(x => x.Answer is not null);
        }
    }
}
