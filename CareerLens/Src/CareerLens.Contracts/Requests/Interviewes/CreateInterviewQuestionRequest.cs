using FluentValidation;

namespace CareerLens.Contracts.Requests.Interviews
{
    public class CreateInterviewQuestionRequest
    {
        public string? QuestionText { get; set; }
         public string? Answer {  get; set; }
    }

    public sealed class CreateInterviewQuestionRequestValidator
    : AbstractValidator<CreateInterviewQuestionRequest>
    {
        public CreateInterviewQuestionRequestValidator()
        {
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
