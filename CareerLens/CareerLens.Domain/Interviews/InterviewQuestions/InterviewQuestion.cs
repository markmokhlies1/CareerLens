using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
using CareerLens.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Interviews.InterviewQuestions
{
    public sealed class InterviewQuestion : AuditableEntity
    {
        public string? Question { get; private set; }
        public string? Answer { get; private set; }

        private InterviewQuestion() { }

        private InterviewQuestion(Guid id, string question, string? answer)
            : base(id)
        {
            Question = question;
            Answer = answer;
        }

        public static Result<InterviewQuestion> Create(Guid id, string question, string? answer)
        {
            if (id == Guid.Empty)
            {
                return InterviewQuestionErrors.IdRequired;
            }

            if (string.IsNullOrWhiteSpace(question))
            {
                return InterviewQuestionErrors.QuestionRequired;
            }

            var wordCount = question.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            if (Helper.CountWords(question) < CareerLensConstants.QuestionLength)
            {
                return InterviewQuestionErrors.QuestionTooShort;
            }

            return new InterviewQuestion(id, question, answer);
        }

        public Result<Updated> UpdateAnswer(string? answer)
        {
            Answer = answer;
            return Result.Updated;
        }
    }
}
