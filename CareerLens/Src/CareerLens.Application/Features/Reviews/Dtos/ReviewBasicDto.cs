using CareerLens.Domain.Reviews.Enums;

namespace CareerLens.Application.Features.Reviews.Dtos
{
    #region DTO&MAPPINGS

    public class ReviewBasicDto : IReviewResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int OverallRating { get; set; }
        public EmploymentStatus? EmploymentStatus { get; set; }
        public string? Headline { get; set; }
        public string? Pros { get; set; }
        public string? Cons { get; set; }
        public JobFunction JobFunction { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public string? JobTitle { get; set; }
        public string? AdviceToManagement { get; set; }
        public int? CareerOpportunities { get; set; }
        public int? CompensationAndBenefits { get; set; }
        public int? CultureAndValues { get; set; }
        public int? DiversityAndInclusion { get; set; }
        public int? SeniorManagement { get; set; }
        public int? WorkLifeBalance { get; set; }
        public Sentiment? CeoRating { get; set; }
        public Sentiment? RecommendToFriend { get; set; }
        public Sentiment? BusinessOutlook { get; set; }
        public string? Location { get; set; }
        public LengthOfEmployment? LengthOfEmployment { get; set; }
    }

    #endregion
}
