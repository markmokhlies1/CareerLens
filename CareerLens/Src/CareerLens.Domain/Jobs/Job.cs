using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Jobs.Enums;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Jobs
{
    public sealed class Job : AuditableEntity 
    {
        public Guid CompanyId { get; private set; }
        public Company? Company { get; private set; }
        public Guid PostedByUserId { get; private set; }
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public string? Location { get; private set; }
        public EmploymentType EmploymentType { get; private set; }
        public WorkplaceType WorkplaceType { get; private set; }
        public ExperienceLevel ExperienceLevel { get; private set; }
        public decimal? MinSalary { get; private set; }
        public decimal? MaxSalary { get; private set; }
        public PayPeriod? PayPeriod { get; private set; }
        public JobStatus Status { get; private set; }
        public string? ApplyUrl { get; private set; }
        private Job() { }

        private Job(Guid id, Guid companyId, Guid postedByUserId, string title, string description, string location,
            EmploymentType employmentType, WorkplaceType workplaceType, ExperienceLevel experienceLevel,
            decimal? minSalary, decimal? maxSalary, PayPeriod? payPeriod,string applyUrl, JobStatus jobStatus)
        : base(id)
        {
            CompanyId = companyId;
            PostedByUserId = postedByUserId;
            Title = title;
            Description = description;
            Location = location;
            EmploymentType = employmentType;
            WorkplaceType = workplaceType;
            ExperienceLevel = experienceLevel;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            PayPeriod = payPeriod;
            ApplyUrl = applyUrl;
            Status = JobStatus.Published;
        }
        public static Result<Job> Create(Guid id, Guid companyId, Guid postedByUserId, string title, string description,
                string location, EmploymentType employmentType, WorkplaceType workplaceType, ExperienceLevel experienceLevel,
                decimal minSalary, decimal maxSalary, PayPeriod payPeriod, string applyUrl)
        {
            if (id == Guid.Empty)
            {
                return JobErrors.IdRequired;
            }

            if (companyId == Guid.Empty)
            {
                return JobErrors.CompanyIdRequired;
            }

            if (postedByUserId == Guid.Empty)
            {
                return JobErrors.PostedByUserRequired;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return JobErrors.TitleRequired;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return JobErrors.DescriptionRequired;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return JobErrors.LocationRequired;
            }

            if (!Enum.IsDefined(employmentType))
            {
                return JobErrors.EmploymentTypeInvalid;
            }

            if (!Enum.IsDefined(workplaceType))
            {
                return JobErrors.WorkplaceTypeInvalid;
            }

            if (!Enum.IsDefined(experienceLevel))
            {
                return JobErrors.ExperienceLevelInvalid;
            }

            if (!Enum.IsDefined(payPeriod))
            {
                return JobErrors.PayPeriodInvalid;
            }

            if (minSalary <= 0 || maxSalary <= 0 || minSalary > maxSalary)
            {
                return JobErrors.InvalidSalaryRange;
            }

            if (string.IsNullOrWhiteSpace(applyUrl))
            {
                return JobErrors.ApplyUrlRequired;
            }

            if (!Uri.TryCreate(applyUrl, UriKind.Absolute, out _))
            {
                return JobErrors.ApplyUrlInvalid;
            }

            return new Job(
                id,
                companyId,
                postedByUserId,
                title,
                description,
                location,
                employmentType,
                workplaceType,
                experienceLevel,
                minSalary,
                maxSalary,
                payPeriod,
                applyUrl,
                JobStatus.Published
            );
        }

        public Result<Updated> Update(string title,
                                      string description,
                                      string location,
                                      EmploymentType employmentType,
                                      WorkplaceType workplaceType,
                                      ExperienceLevel experienceLevel,
                                      decimal minSalary,
                                      decimal maxSalary,
                                      PayPeriod payPeriod,
                                      string applyUrl)
        {
            if (Status == JobStatus.Closed)
            {
                return JobErrors.CannotModifyClosedJob;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return JobErrors.TitleRequired;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return JobErrors.DescriptionRequired;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return JobErrors.LocationRequired;
            }

            if (!Enum.IsDefined(employmentType))
            {
                return JobErrors.EmploymentTypeInvalid;
            }

            if (!Enum.IsDefined(workplaceType))
            {
                return JobErrors.WorkplaceTypeInvalid;
            }

            if (!Enum.IsDefined(experienceLevel))
            {
                return JobErrors.ExperienceLevelInvalid;
            }

            if (!Enum.IsDefined(payPeriod))
            {
                return JobErrors.PayPeriodInvalid;
            }

            if (minSalary <= 0 || maxSalary <= 0 || minSalary > maxSalary)
            {
                return JobErrors.InvalidSalaryRange;
            }

            if (string.IsNullOrWhiteSpace(applyUrl))
            {
                return JobErrors.ApplyUrlRequired;
            }

            if (!Uri.TryCreate(applyUrl, UriKind.Absolute, out _))
            {
                return JobErrors.ApplyUrlInvalid;
            }

            Title = title;
            Description = description;
            Location = location;
            EmploymentType = employmentType;
            WorkplaceType = workplaceType;
            ExperienceLevel = experienceLevel;
            MinSalary = minSalary;
            MaxSalary = maxSalary;
            PayPeriod = payPeriod;
            ApplyUrl = applyUrl;

            return Result.Updated;
        }

        public Result<Updated> UpdateStatus(JobStatus newStatus)
        {
            if (newStatus == Status)
            {
                return JobErrors.StatusAlreadySet;
            }

            switch (Status, newStatus)
            {

                case (JobStatus.Draft, JobStatus.Published):
                case (JobStatus.Published, JobStatus.Draft):
                case (JobStatus.Published, JobStatus.Closed):

                    break;

                case (JobStatus.Draft, JobStatus.Closed):
                    return JobErrors.JobNotPublished;

                case (JobStatus.Closed, _):
                    return JobErrors.CannotModifyClosedJob;

                default:
                    return JobErrors.InvalidStatusTransition;
            }

            Status = newStatus;
            return Result.Updated;
        }
    }
}
