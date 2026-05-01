using CareerLens.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Interviews.InterviewQuestions
{
    public static class InterviewQuestionErrors
    {
        public static Error IdRequired =>
            Error.Validation("InterviewQuestion.Id.Required", "Interview question Id is required.");

        public static Error QuestionRequired =>
            Error.Validation("InterviewQuestion.Question.Required", "Question text is required.");

        public static Error QuestionTooShort =>
            Error.Validation("InterviewQuestion.Question.TooShort","Interview question must contain at least 5 words.");
    }
}
