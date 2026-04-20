namespace CareerLens.Application.Features.Interviews.Dtos
{
    public class InterviewQuestionDto 
    {
        public Guid QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public string? Answer { get; set; }
    }
}
