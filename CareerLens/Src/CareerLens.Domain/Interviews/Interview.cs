using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Interviews.Enums;
using CareerLens.Domain.Interviews.InterviewQuestions;
using CareerLens.Domain.Reviews;
using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.DomainUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace CareerLens.Domain.Interviews
{
    public sealed class Interview : AuditableEntity 
    {  
        public Guid UserId { get; private set; }
        public User? User { get; private set; }
        public Guid CompanyId { get; private set; }
        public OverallExperience OverallExperience { get; private set; }
        public string? JobTitle { get; private set; }
        public string? Description { get; private set; }
        public InterviewDifficulty InterviewDifficulty { get; private set; }
        public GettingOffer GettingOffer { get; private set; }
        public InterviewSource? Source { get; private set; }
        public string? Location { get; private set; }
        public InterviewDuration? Duration { get; private set; }
        public InterviewDate? Date { get; private set; }
        public InterviewStage? Stages { get; private set; }
        public HelpingLevel? HelpingLevel { get; private set; }
        public InterviewStatus InterviewStatus { get; private set; }

        private readonly List<InterviewQuestion> _interviewQuestions = [];
        public IEnumerable<InterviewQuestion> InterviewQuestions => _interviewQuestions.AsReadOnly();
        private Interview()
        {
        }
        private Interview
            (Guid id,Guid userId, Guid companyId, OverallExperience overallExperience, string? jobTitle,
            string? description, InterviewDifficulty interviewDifficulty, GettingOffer gettingOffer,
            InterviewSource? source, string? location, InterviewDuration? duration, InterviewDate? date,
            InterviewStage? stages, HelpingLevel? helpingLevel, List<InterviewQuestion> interviewQuestions, InterviewStatus interviewStatus= InterviewStatus.Pending)
            : base(id)
        {
            UserId = userId;
            CompanyId = companyId;
            OverallExperience = overallExperience;
            JobTitle = jobTitle;
            Description = description;
            InterviewDifficulty = interviewDifficulty;
            GettingOffer = gettingOffer;
            Source = source;
            Location = location;
            Duration = duration;
            Date = date;
            Stages = stages;
            HelpingLevel = helpingLevel;
            _interviewQuestions = interviewQuestions;
            InterviewStatus = interviewStatus;
        }

        public static Result<Interview> Create(Guid id, Guid userId, Guid companyId, OverallExperience overallExperience,
            InterviewDifficulty interviewDifficulty, GettingOffer gettingOffer, InterviewSource? source,
            HelpingLevel? helpingLevel, List<InterviewQuestion> interviewQuestions, string jobTitle,
            string description, string? location = null, InterviewDuration? duration = null,
            InterviewDate? date = null, InterviewStage? stages = null)
        {
            if (id == Guid.Empty)
            {
                return InterviewErrors.IdRequired;
            }

            if (userId == Guid.Empty)
            {
                return InterviewErrors.UserIdRequired;
            }

            if (companyId == Guid.Empty)
            {
                return InterviewErrors.CompanyIdRequired;
            }

            if (!Enum.IsDefined(overallExperience))
            {
                return InterviewErrors.OverallExperienceInvalid;
            }

            if (!Enum.IsDefined(interviewDifficulty))
            {

                return InterviewErrors.InterviewDifficultyInvalid;
            }

            if (!Enum.IsDefined(gettingOffer))
            {
                return InterviewErrors.GettingOfferInvalid;
            }

            if (string.IsNullOrWhiteSpace(jobTitle))
            {
                return InterviewErrors.JobTitleRequired;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return InterviewErrors.DescriptionRequired;
            }


            if (Helper.CountWords(description) < CareerLensConstants.InterviewDescriptionLength)
            {
                return InterviewErrors.DescriptionTooShort;
            }

            if (interviewQuestions == null || interviewQuestions.Count == 0)
            {
                return InterviewErrors.QuestionsRequired;
            }

            if (source.HasValue && !Enum.IsDefined(source.Value))
            {
                return InterviewErrors.SourceInvalid;
            }

            if (helpingLevel.HasValue && !Enum.IsDefined(helpingLevel.Value))
            {
                return InterviewErrors.HelpingLevelInvalid;
            }

            return new Interview(id,userId,companyId,overallExperience,
                jobTitle,description,interviewDifficulty,gettingOffer,
                source,location,duration,date,stages,helpingLevel,
                interviewQuestions,InterviewStatus.Pending
            );
        }

        public Result<Updated> Update(
                                      OverallExperience overallExperience,
                                      InterviewDifficulty interviewDifficulty,
                                      GettingOffer gettingOffer,
                                      string jobTitle,
                                      string description,
                                      InterviewSource? source,
                                      HelpingLevel? helpingLevel,
                                      List<InterviewQuestion> interviewQuestions,
                                      string? location = null,
                                      InterviewDuration? duration = null,
                                      InterviewDate? date = null,
                                      InterviewStage? stages = null)
        {
            if (!Enum.IsDefined(overallExperience))
            {
                return InterviewErrors.OverallExperienceInvalid;
            }

            if (!Enum.IsDefined(interviewDifficulty))
            {
                return InterviewErrors.InterviewDifficultyInvalid;
            }

            if (!Enum.IsDefined(gettingOffer))
            {
                return InterviewErrors.GettingOfferInvalid;
            }

            if (string.IsNullOrWhiteSpace(jobTitle))
            {
                return InterviewErrors.JobTitleRequired;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return InterviewErrors.DescriptionRequired;
            }

            if (Helper.CountWords(description) < CareerLensConstants.InterviewDescriptionLength)
            {
                return InterviewErrors.DescriptionTooShort;
            }

            if (interviewQuestions is null || interviewQuestions.Count == 0)
            {
                return InterviewErrors.QuestionsRequired;
            }

            if (source.HasValue && !Enum.IsDefined(source.Value))
            {
                return InterviewErrors.SourceInvalid;
            }

            if (helpingLevel.HasValue && !Enum.IsDefined(helpingLevel.Value))
            {
                return InterviewErrors.HelpingLevelInvalid;
            }

            OverallExperience = overallExperience;
            InterviewDifficulty = interviewDifficulty;
            GettingOffer = gettingOffer;
            JobTitle = jobTitle;
            Description = description;
            Source = source;
            HelpingLevel = helpingLevel;
            Location = location;
            Duration = duration;
            Date = date;
            Stages = stages;

            _interviewQuestions.Clear();
            _interviewQuestions.AddRange(interviewQuestions);

            return Result.Updated;
        }

        public Result<Updated> AddQuestion(InterviewQuestion question)
        {
            if (_interviewQuestions.Any(q => q.Id == question.Id))
            {
                return InterviewErrors.QuestionAlreadyAdded;
            }

            _interviewQuestions.Add(question);
            return Result.Updated;
        }
        public Result<Updated> RemoveQuestion(Guid questionId)
        {
            var question = _interviewQuestions.FirstOrDefault(q => q.Id == questionId);
            
            if (question is null)
            {

                return InterviewErrors.QuestionNotFound;
            }

            _interviewQuestions.Remove(question);
            return Result.Updated;
        }
        private bool IsEditable => InterviewStatus is InterviewStatus.Pending;
        
        public Result<Updated> UpdateState(InterviewStatus newInterviewStatus)
        {
            if (!IsEditable && newInterviewStatus == InterviewStatus.Approved)
            {
                return InterviewErrors.CannotApproveNonPendingInterview;
            }

            if (!IsEditable && newInterviewStatus == InterviewStatus.Rejected)
            {
                return InterviewErrors.CannotRejectNonPendingInterview;
            }

            InterviewStatus = newInterviewStatus;

            return Result.Updated;
        }
    }
}
