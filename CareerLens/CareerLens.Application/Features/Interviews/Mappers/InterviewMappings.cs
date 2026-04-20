using CareerLens.Application.Features.Interviews.Dtos;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.InterviewQuestions;

namespace CareerLens.Application.Features.Interviews.Mappers
{
    public static class InterviewMappings
    {
        public static InterviewBasicDto ToBasicDto(this Interview entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new InterviewBasicDto
            {
                UserId = entity.UserId,
                CompanyId = entity.CompanyId,
                OverallExperience = entity.OverallExperience,
                JobTitle = entity.JobTitle,
                Description = entity.Description,
                InterviewDifficulty = entity.InterviewDifficulty,
                GettingOffer = entity.GettingOffer,
                Source = entity.Source,
                Location = entity.Location,
                Duration = entity.Duration,
                Date = entity.Date,
                Stages = entity.Stages,
                HelpingLevel = entity.HelpingLevel,
                UserName = entity.User!.FirstName! + " " + entity.User.LastName!,
                UserEmail = entity.User?.Email,
                InterviewQuestions = entity.InterviewQuestions?.Select(v => v.ToInterviewQuestionDto()).ToList() ?? []
            };
        }
        public static InterviewEmployerDto ToEmployerDto(this Interview entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new InterviewEmployerDto
            {
                UserId = entity.UserId,
                CompanyId = entity.CompanyId,
                OverallExperience = entity.OverallExperience,
                JobTitle = entity.JobTitle,
                Description = entity.Description,
                InterviewDifficulty = entity.InterviewDifficulty,
                GettingOffer = entity.GettingOffer,
                Source = entity.Source,
                Location = entity.Location,
                Duration = entity.Duration,
                Date = entity.Date,
                Stages = entity.Stages,
                HelpingLevel = entity.HelpingLevel,
                UserName = entity.User!.FirstName! + " " + entity.User.LastName!,
                UserEmail = entity.User?.Email,
                InterviewStatus = entity.InterviewStatus,
                InterviewQuestions = entity.InterviewQuestions?.Select(v => v.ToInterviewQuestionDto()).ToList() ?? []
            };
        }
        public static InterviewQuestionDto ToInterviewQuestionDto(this InterviewQuestion entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return new InterviewQuestionDto
            {
                QuestionId = entity.Id,
                QuestionText = entity.Question,
                Answer = entity.Answer
            };
        }
        public static List<IInterviewResponse> ToBasicDtos(this IEnumerable<Interview> entities)
        {
            return [.. entities.Select(e => e.ToBasicDto())];
        }
        public static List<IInterviewResponse> ToEmployerDtos(this IEnumerable<Interview> entities)
        {
            return [.. entities.Select(e => e.ToEmployerDto())];
        }
    }
}
