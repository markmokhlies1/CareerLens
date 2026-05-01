using CareerLens.Domain.Interviews.Enums;

namespace CareerLens.Application.Features.Interviews.Dtos
{
    public class InterviewBasicDto : IInterviewResponse 
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public OverallExperience OverallExperience { get; set; }
        public string? JobTitle { get; set; }
        public string? Description { get; set; }
        public InterviewDifficulty InterviewDifficulty { get; set; }
        public GettingOffer GettingOffer { get; set; }
        public InterviewSource? Source { get; set; }
        public string? Location { get; set; }
        public InterviewDuration? Duration { get; set; }
        public InterviewDate? Date { get; set; }
        public InterviewStage? Stages { get; set; }
        public HelpingLevel? HelpingLevel { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }

        public List<InterviewQuestionDto> InterviewQuestions = [];
    }
}
