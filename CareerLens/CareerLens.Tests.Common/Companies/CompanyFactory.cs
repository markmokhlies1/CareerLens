using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.Enums;
using CareerLens.Tests.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Tests.Common.Companies
{
    public static class CompanyFactory
    {
        public static Result<Company> CreateCompany(
            Guid? id = null,
            string? name = null,
            string? industry = null,
            string? location = null,
            CompanySize? size = null,
            string? website = null,
            string? description = null,
            int? foundedYear = null)
        {
            return Company.Create(
                id: id ?? Guid.NewGuid(),
                name: name ?? "Meta",
                industry: industry ?? "Technology",
                location: location ?? TestConstants.DefaultLocation,
                size: size ?? CompanySize.Large_501_1000,
                website: website ?? "https://meta.com",
                description: description ?? "Meta builds technologies that help people connect.",
                foundedYear: foundedYear ?? 2004
            );
        }

        
        public static Company CreateClaimedCompany()
        {
            var company = CreateCompany().Value;
            company.Claim();
            return company;
        }

        
        public static Result<Company> CreateCompanyWithMaxDescription()
        {
            var maxDescription = new string('A', CareerLensConstants.CompanyDescriptionMaxLenght);
            return CreateCompany(description: maxDescription);
        }
    }
}
