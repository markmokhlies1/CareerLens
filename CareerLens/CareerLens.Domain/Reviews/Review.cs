using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Helpers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Reviews.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Reviews
{
    public sealed class Review : AuditableEntity
    { 
        public Guid UserId { get; private set; } 
        public Guid CompanyId { get; private set; }
        public Company? Company { get; private set; }
        public int OverallRating { get; private set; }
        public EmploymentStatus? EmploymentStatus { get; private set; }
        public string? Headline { get; private set; }
        public string? Pros { get; private set; }
        public string? Cons { get; private set; }
        public JobFunction JobFunction { get; private set; }
        public EmployeeType EmployeeType { get; private set; }
        public string? JobTitle { get; private set; }
        public string? AdviceToManagement { get; private set; }
        public int? CareerOpportunities { get; private set; }
        public int? CompensationAndBenefits { get; private set; }
        public int? CultureAndValues { get; private set; }
        public int? DiversityAndInclusion { get; private set; }
        public int? SeniorManagement { get; private set; }
        public int? WorkLifeBalance { get; private set; }
        public Sentiment? CeoRating { get; private set; }
        public Sentiment? RecommendToFriend { get; private set; }
        public Sentiment? BusinessOutlook { get; private set; }
        public string? Location { get; private set; }
        public LengthOfEmployment? LengthOfEmployment { get; private set; }
        public ReviewStatus Status { get; private set; }
        private Review()
        {
        }

        private Review(Guid id,Guid userId,Guid companyId,int overallRating,EmploymentStatus employmentStatus,
            JobFunction jobFunction,LengthOfEmployment lengthOfEmployment,string headline,string pros,
            string cons,EmployeeType employeeType,string? jobTitle,string? adviceToManagement,int? careerOpportunities,
            int? compensationAndBenefits,
            int? cultureAndValues,int? diversityAndInclusion,int? seniorManagement,int? workLifeBalance,Sentiment? ceoRating,
            Sentiment? recommendToFriend,Sentiment? businessOutlook,string? location,ReviewStatus reviewStatus = ReviewStatus.Pending)
            : base(id)
        {
            UserId = userId;
            CompanyId = companyId;
            OverallRating = overallRating;
            EmploymentStatus = employmentStatus;
            JobFunction = jobFunction;
            LengthOfEmployment = lengthOfEmployment;
            Headline = headline;
            Pros = pros;
            Cons = cons;
            EmployeeType = employeeType;
            JobTitle = jobTitle;
            AdviceToManagement = adviceToManagement;
            CareerOpportunities = careerOpportunities;
            CompensationAndBenefits = compensationAndBenefits;
            CultureAndValues = cultureAndValues;
            DiversityAndInclusion = diversityAndInclusion;
            SeniorManagement = seniorManagement;
            WorkLifeBalance = workLifeBalance;
            CeoRating = ceoRating;
            RecommendToFriend = recommendToFriend;
            BusinessOutlook = businessOutlook;
            Location = location;
            Status = reviewStatus;
        }

        public static Result<Review> Create(Guid id,Guid userId,Guid companyId,int overallRating,EmploymentStatus employmentStatus,
                JobFunction jobFunction,LengthOfEmployment lengthOfEmployment,string headline,string pros,
                string cons,EmployeeType employeeType,string? jobTitle = null,string? adviceToManagement = null,
                int? careerOpportunities = null,int? compensationAndBenefits = null,int? cultureAndValues = null,
                int? diversityAndInclusion = null,int? seniorManagement = null,int? workLifeBalance = null,
                Sentiment? ceoRating = null,Sentiment? recommendToFriend = null,Sentiment? businessOutlook = null,string? location = null)
        {
            if (id == Guid.Empty)
            {
                return ReviewErrors.IdRequired;
            }

            if (userId == Guid.Empty)
            {
                return ReviewErrors.UserIdRequired;
            }

            if (companyId == Guid.Empty)
            {
                return ReviewErrors.CompanyIdRequired;
            }

            if (overallRating < 1 || overallRating > 5)
            {
                return ReviewErrors.OverallRatingInvalid;
            }

            if (!Enum.IsDefined(employeeType))
            {
                return ReviewErrors.EmployeeTypeInvalid;
            }

            if (!Enum.IsDefined(employmentStatus))
            {
                return ReviewErrors.EmploymentStatusInvalid;
            }

            if (!Enum.IsDefined(jobFunction))
            {
                return ReviewErrors.JobFunctionInvalid;
            }

            if (!Enum.IsDefined(lengthOfEmployment))
            {
                return ReviewErrors.LengthOfEmploymentInvalid;
            }

            if (string.IsNullOrWhiteSpace(headline))
            {
                return ReviewErrors.HeadlineRequired;
            }

            if(string.IsNullOrWhiteSpace(pros))
            {
                return ReviewErrors.ProsRequired;
            }

            if (string.IsNullOrWhiteSpace(cons))
            {
                return ReviewErrors.ConsRequired;
            }

            if (Helper.CountWords(pros) < CareerLensConstants.InterviewReviewPronsLength)
            {
                return ReviewErrors.ProsTooShort;
            }

            if (Helper.CountWords(cons) < CareerLensConstants.InterviewReviewConsLength)
            {
                return ReviewErrors.ConsTooShort;
            }

            if (careerOpportunities.HasValue && (careerOpportunities < 1 || careerOpportunities > 5))
            {
                return ReviewErrors.CareerOpportunitiesInvalid;
            }

            if (compensationAndBenefits.HasValue && (compensationAndBenefits < 1 || compensationAndBenefits > 5))
            {
                return ReviewErrors.CompensationAndBenefitsInvalid;
            }

            if (cultureAndValues.HasValue && (cultureAndValues < 1 || cultureAndValues > 5))
            {
                return ReviewErrors.CultureAndValuesInvalid;
            }

            if (diversityAndInclusion.HasValue && (diversityAndInclusion < 1 || diversityAndInclusion > 5))
            {
                return ReviewErrors.DiversityAndInclusionInvalid;
            }

            if (seniorManagement.HasValue && (seniorManagement < 1 || seniorManagement > 5))
            {
                return ReviewErrors.SeniorManagementInvalid;
            }

            if (workLifeBalance.HasValue && (workLifeBalance < 1 || workLifeBalance > 5))
            {
                return ReviewErrors.WorkLifeBalanceInvalid;
            }

            return new Review(
                id,
                userId,
                companyId,
                overallRating,
                employmentStatus,
                jobFunction,
                lengthOfEmployment,
                headline,
                pros,
                cons,
                employeeType,
                jobTitle,
                adviceToManagement,
                careerOpportunities,
                compensationAndBenefits,
                cultureAndValues,
                diversityAndInclusion,
                seniorManagement,
                workLifeBalance,
                ceoRating,
                recommendToFriend,
                businessOutlook,
                location,
                ReviewStatus.Pending
            );
        }

        public Result<Updated> Update(int overallRating,
                                      EmploymentStatus employmentStatus,
                                      JobFunction jobFunction,
                                      LengthOfEmployment lengthOfEmployment,
                                      string headline,
                                      string pros,
                                      string cons,
                                      EmployeeType employeeType,
                                      string? jobTitle = null,
                                      string? adviceToManagement = null,
                                      int? careerOpportunities = null,
                                      int? compensationAndBenefits = null,
                                      int? cultureAndValues = null,
                                      int? diversityAndInclusion = null,
                                      int? seniorManagement = null,
                                      int? workLifeBalance = null,
                                      Sentiment? ceoRating = null,
                                      Sentiment? recommendToFriend = null,
                                      Sentiment? businessOutlook = null,
                                      string? location = null)
        {
            if (overallRating < 1 || overallRating > 5)
            {
                return ReviewErrors.OverallRatingInvalid;
            }

            if (!Enum.IsDefined(employeeType))
            {
                return ReviewErrors.EmployeeTypeInvalid;
            }

            if (!Enum.IsDefined(employmentStatus))
            {
                return ReviewErrors.EmploymentStatusInvalid;
            }

            if (!Enum.IsDefined(jobFunction))
            {
                return ReviewErrors.JobFunctionInvalid;
            }

            if (!Enum.IsDefined(lengthOfEmployment))
            {
                return ReviewErrors.LengthOfEmploymentInvalid;
            }

            if (string.IsNullOrWhiteSpace(headline))
            {
                return ReviewErrors.HeadlineRequired;
            }

            if (string.IsNullOrWhiteSpace(pros))
            {
                return ReviewErrors.ProsRequired;
            }

            if (string.IsNullOrWhiteSpace(cons))
            {
                return ReviewErrors.ConsRequired;
            }

            if (Helper.CountWords(pros) < CareerLensConstants.InterviewReviewPronsLength)
            {
                return ReviewErrors.ProsTooShort;
            }

            if (Helper.CountWords(cons) < CareerLensConstants.InterviewReviewConsLength)
            {
                return ReviewErrors.ConsTooShort;
            }

            if (careerOpportunities.HasValue && (careerOpportunities < 1 || careerOpportunities > 5))
            {
                return ReviewErrors.CareerOpportunitiesInvalid;
            }

            if (compensationAndBenefits.HasValue && (compensationAndBenefits < 1 || compensationAndBenefits > 5))
            {
                return ReviewErrors.CompensationAndBenefitsInvalid;
            }

            if (cultureAndValues.HasValue && (cultureAndValues < 1 || cultureAndValues > 5))
            {
                return ReviewErrors.CultureAndValuesInvalid;
            }

            if (diversityAndInclusion.HasValue && (diversityAndInclusion < 1 || diversityAndInclusion > 5))
            {
                return ReviewErrors.DiversityAndInclusionInvalid;
            }

            if (seniorManagement.HasValue && (seniorManagement < 1 || seniorManagement > 5))
            {
                return ReviewErrors.SeniorManagementInvalid;
            }

            if (workLifeBalance.HasValue && (workLifeBalance < 1 || workLifeBalance > 5))
            {
                return ReviewErrors.WorkLifeBalanceInvalid;
            }

            OverallRating = overallRating;
            EmploymentStatus = employmentStatus;
            JobFunction = jobFunction;
            LengthOfEmployment = lengthOfEmployment;
            Headline = headline;
            Pros = pros;
            Cons = cons;
            EmployeeType = employeeType;
            JobTitle = jobTitle;
            AdviceToManagement = adviceToManagement;
            CareerOpportunities = careerOpportunities;
            CompensationAndBenefits = compensationAndBenefits;
            CultureAndValues = cultureAndValues;
            DiversityAndInclusion = diversityAndInclusion;
            SeniorManagement = seniorManagement;
            WorkLifeBalance = workLifeBalance;
            CeoRating = ceoRating;
            RecommendToFriend = recommendToFriend;
            BusinessOutlook = businessOutlook;
            Location = location;

            return Result.Updated;
        }
        private bool IsEditable => Status is ReviewStatus.Pending;
        public Result<Success> Approve()
        {
            if (!IsEditable)
            {
                return ReviewErrors.CannotApproveNonPendingReview;
            }

            Status = ReviewStatus.Approved;
            return Result.Success;
        }
        public Result<Success> Reject()
        {
            if (!IsEditable)
            {
                return ReviewErrors.CannotRejectNonPendingReview;
            }

            Status = ReviewStatus.Rejected;
            return Result.Success;
        }
    }
}
