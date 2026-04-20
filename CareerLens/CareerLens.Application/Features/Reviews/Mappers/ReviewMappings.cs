using CareerLens.Application.Features.Reviews.Dtos;
using CareerLens.Domain.Reviews;

namespace CareerLens.Application.Features.Reviews.Mappers
{
    #region DTO&MAPPINGS

    public static class ReviewMappings
    {
        public static ReviewBasicDto ToBasicDto(this Review entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(entity.Company, nameof(entity.Company));

            return new ReviewBasicDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                CompanyName = entity.Company.Name,
                OverallRating = entity.OverallRating,
                EmploymentStatus = entity.EmploymentStatus,
                Headline = entity.Headline,
                Pros = entity.Pros,
                Cons = entity.Cons,
                JobFunction = entity.JobFunction,
                EmployeeType = entity.EmployeeType,
                JobTitle = entity.JobTitle,
                AdviceToManagement = entity.AdviceToManagement,
                CareerOpportunities = entity.CareerOpportunities,
                CompensationAndBenefits = entity.CompensationAndBenefits,
                CultureAndValues = entity.CultureAndValues,
                DiversityAndInclusion = entity.DiversityAndInclusion,
                SeniorManagement = entity.SeniorManagement,
                WorkLifeBalance = entity.WorkLifeBalance,
                CeoRating = entity.CeoRating,
                RecommendToFriend = entity.RecommendToFriend,
                BusinessOutlook = entity.BusinessOutlook,
                Location = entity.Location,
                LengthOfEmployment = entity.LengthOfEmployment
            };
        }

        public static ReviewEmployerDto ToEmployerDto(this Review entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(entity.Company, nameof(entity.Company));

            return new ReviewEmployerDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CompanyId = entity.CompanyId,
                CompanyName = entity.Company.Name,
                OverallRating = entity.OverallRating,
                EmploymentStatus = entity.EmploymentStatus,
                Headline = entity.Headline,
                Pros = entity.Pros,
                Cons = entity.Cons,
                JobFunction = entity.JobFunction,
                EmployeeType = entity.EmployeeType,
                JobTitle = entity.JobTitle,
                AdviceToManagement = entity.AdviceToManagement,
                CareerOpportunities = entity.CareerOpportunities,
                CompensationAndBenefits = entity.CompensationAndBenefits,
                CultureAndValues = entity.CultureAndValues,
                DiversityAndInclusion = entity.DiversityAndInclusion,
                SeniorManagement = entity.SeniorManagement,
                WorkLifeBalance = entity.WorkLifeBalance,
                CeoRating = entity.CeoRating,
                RecommendToFriend = entity.RecommendToFriend,
                BusinessOutlook = entity.BusinessOutlook,
                Location = entity.Location,
                LengthOfEmployment = entity.LengthOfEmployment,
                Status = entity.Status
            };
        }

        public static List<IReviewResponse> ToBasicDtos(this IEnumerable<Review> entities)
        {
            return [.. entities.Select(e => e.ToBasicDto())];
        }

        public static List<IReviewResponse> ToEmployerDtos(this IEnumerable<Review> entities)
        {
            return [.. entities.Select(e => e.ToEmployerDto())];
        }
    }

    #endregion
}
