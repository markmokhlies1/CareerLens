using CareerLens.Application.Features.Jobs.Dtos;
using CareerLens.Domain.Jobs;

namespace CareerLens.Application.Features.Jobs.Mappers
{
    public static class JobMappings
    {
        public static JobBasicDto ToBasicDto(this Job entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new JobBasicDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                Title = entity.Title,
                Description = entity.Description,
                Location = entity.Location,
                EmploymentType = entity.EmploymentType,
                WorkplaceType = entity.WorkplaceType,
                ExperienceLevel = entity.ExperienceLevel,
                MinSalary = entity.MinSalary,
                MaxSalary = entity.MaxSalary,
                PayPeriod = entity.PayPeriod,
                ApplyUrl = entity.ApplyUrl,
                CompanyName = entity.Company?.Name!
            };
        }

        public static JobEmployerDto ToEmployerDto(this Job entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new JobEmployerDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                PostedByUserId = entity.PostedByUserId,
                Title = entity.Title,
                Description = entity.Description,
                Location = entity.Location,
                EmploymentType = entity.EmploymentType,
                WorkplaceType = entity.WorkplaceType,
                ExperienceLevel = entity.ExperienceLevel,
                MinSalary = entity.MinSalary,
                MaxSalary = entity.MaxSalary,
                PayPeriod = entity.PayPeriod,
                Status = entity.Status,
                ApplyUrl = entity.ApplyUrl,
                CompanyName = entity.Company?.Name
            };
        }

        public static List<IJobResponse> ToBasicDtos(this IEnumerable<Job> entities)
        {
            return [.. entities.Select(e => e.ToBasicDto())];
        }

        public static List<IJobResponse> ToEmployerDtos(this IEnumerable<Job> entities)
        {
            return [.. entities.Select(e => e.ToEmployerDto())];
        }
    }
}
