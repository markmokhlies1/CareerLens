using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.Enums;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.InterviewQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Companies
{
    public sealed class Company : AuditableEntity
    {
        public string Name { get; private set; } = default!;
        public string Industry { get; private set; } = default!;
        public string Location { get; private set; } = default!;
        public string Website { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public int FoundedYear { get; private set; }
        public CompanySize Size { get; private set; }
        public bool IsClaimed { get; private set; }

        private readonly List<CompanyMember> _members = [];
        public IReadOnlyCollection<CompanyMember> Members => _members.AsReadOnly();

        private readonly List<CompanyClaimRequest> _requests = [];
        public IReadOnlyCollection<CompanyClaimRequest> Requests => _requests.AsReadOnly();
        private Company()
        {
        }
        private Company
            (Guid id, string name, string industry, string location, CompanySize size, string website, string description
            , int foundedYear)
            : base(id)
        {
            Name = name;
            Industry = industry;
            Location = location;
            Website = website;
            Description = description;
            FoundedYear = foundedYear;
            Size = size;
            IsClaimed = false;
        }
        public static Result<Company> Create(Guid id, string name, string industry, string location, CompanySize size
            , string website, string description, int foundedYear)
        {

            if (id == Guid.Empty)
            {
                return CompanyErrors.IdRequired;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return CompanyErrors.NameRequired;
            }

            if (string.IsNullOrWhiteSpace(industry))
            {
                return CompanyErrors.IndustryRequired;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return CompanyErrors.LocationRequired;
            }

            if (!Enum.IsDefined(size))
            {
                return CompanyErrors.SizeInvalid;
            }

            if (string.IsNullOrWhiteSpace(website))
            {
                return CompanyErrors.WebsiteRequired;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return CompanyErrors.DescriptionRequired;
            }

            if (description.Length > CareerLensConstants.CompanyDescriptionMaxLenght)
            {
                return CompanyErrors.DescriptionTooLong;
            }

            if (!Uri.TryCreate(website, UriKind.Absolute, out _))
            {
                return CompanyErrors.WebsiteInvalid;
            }

            if (foundedYear < 1800 || foundedYear > DateTime.UtcNow.Year)
            {
                return CompanyErrors.FoundedYearInvalid;
            }

            return new Company(id, name, industry, location, size, website, description, foundedYear);
        }

        public Result<Updated> Claim()
        {
            if (IsClaimed)
            {
                return CompanyErrors.AlreadyClaimed;
            }

            IsClaimed = true;
            return Result.Updated;
        }

        public Result<Updated> UpdateProfile(string name, string industry, string location, CompanySize size,
            string website, string description, int foundedYear)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return CompanyErrors.NameRequired;
            }

            if (string.IsNullOrWhiteSpace(industry))
            {
                return CompanyErrors.IndustryRequired;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return CompanyErrors.LocationRequired;
            }

            if (!Enum.IsDefined(size))
            {
                return CompanyErrors.SizeInvalid;
            }

            if (string.IsNullOrWhiteSpace(website))
            {
                return CompanyErrors.WebsiteRequired;
            }

            if (!Uri.TryCreate(website, UriKind.Absolute, out _))
            {
                return CompanyErrors.WebsiteInvalid;
            }

            if (foundedYear < 1800 || foundedYear > DateTime.UtcNow.Year)
            {
                return CompanyErrors.FoundedYearInvalid;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return CompanyErrors.DescriptionRequired;
            }

            if (description.Length > CareerLensConstants.CompanyDescriptionMaxLenght)
            {
                return CompanyErrors.DescriptionTooLong;
            }

            Name = name;
            Industry = industry;
            Location = location;
            Size = size;
            Website = website;
            Description = description;
            FoundedYear = foundedYear;

            return Result.Updated;
        }
        public Result<Updated> AddCompanyClaimRequest(CompanyClaimRequest companyClaimRequest)
        {
            _requests.Add(companyClaimRequest);
            return Result.Updated;
        }
    }
}