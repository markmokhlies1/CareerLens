using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.Enums;
using CareerLens.Tests.Common.Companies;
using CareerLens.Tests.Common.CompanyClaimRequestes;
using CareerLens.Tests.Common.Constants;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Tests.Domain.Companies
{
    public sealed class CompanyTests
    {

        [Fact]
        public void CreateCompany_ShouldSucceed_WithValidData()
        {
            var id = Guid.NewGuid();
            const string name = "Meta";
            const string industry = "Technology";
            const string location = "Menlo Park, CA";
            const string website = "https://meta.com";
            const string description = "Meta builds technologies that help people connect.";
            const int foundedYear = 2004;
            const CompanySize size = CompanySize.Large_501_1000;

            var result = CompanyFactory.CreateCompany(
                id: id,
                name: name,
                industry: industry,
                location: location,
                size: size,
                website: website,
                description: description,
                foundedYear: foundedYear);

            Assert.False(result.IsError);

            var company = result.Value;
            Assert.IsType<Company>(company);
            Assert.NotNull(company);
            Assert.Equal(id, company.Id);
            Assert.Equal(name, company.Name);
            Assert.Equal(industry, company.Industry);
            Assert.Equal(location, company.Location);
            Assert.Equal(size, company.Size);
            Assert.Equal(website, company.Website);
            Assert.Equal(description, company.Description);
            Assert.Equal(foundedYear, company.FoundedYear);
            Assert.False(company.IsClaimed);
            Assert.Empty(company.Members);
            Assert.Empty(company.Requests);
        }

       

        [Fact]
        public void CreateCompany_ShouldFail_WhenIdIsEmpty()
        {
            var result = CompanyFactory.CreateCompany(id: Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.IdRequired, result.TopError);
        }


        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateCompany_ShouldFail_WhenNameIsInvalid(string? name)
        {
            var result = Company.Create(
                id: Guid.NewGuid(),
                name: name!,
                industry: "Technology",
                location: "Cairo, Egypt",
                size: CompanySize.Large_501_1000,
                website: "https://meta.com",
                description: "Meta builds technologies that help people connect.",
                foundedYear: 2004);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.NameRequired, result.Errors[0]);
        }


        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateCompany_ShouldFail_WhenIndustryIsInvalid(string? industry)
        {
            var result = Company.Create(
                id: Guid.NewGuid(),
                name: "Meta",
                industry: industry!,
                location: "Cairo, Egypt",
                size: CompanySize.Large_501_1000,
                website: "https://meta.com",
                description: "Meta builds technologies that help people connect.",
                foundedYear: 2004);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.IndustryRequired, result.Errors[0]);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateCompany_ShouldFail_WhenLocationIsInvalid(string? location)
        {
            var result = Company.Create(
                id: Guid.NewGuid(),
                name: "Meta",
                industry: "Technology",
                location: location!,
                size: CompanySize.Large_501_1000,
                website: "https://meta.com",
                description: "Meta builds technologies that help people connect.",
                foundedYear: 2004);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.LocationRequired, result.Errors[0]);
        }



        [Fact]
        public void CreateCompany_ShouldFail_WhenSizeIsInvalid()
        {
            var result = CompanyFactory.CreateCompany(size: (CompanySize)999);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.SizeInvalid, result.TopError);
        }


        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateCompany_ShouldFail_WhenWebsiteIsEmpty(string? website)
        {
            var result = Company.Create(
                id: Guid.NewGuid(),
                name: "Meta",
                industry: "Technology",
                location: "Cairo, Egypt",
                size: CompanySize.Large_501_1000,
                website: website!,
                description: "Meta builds technologies that help people connect.",
                foundedYear: 2004);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.WebsiteRequired, result.Errors[0]);
        }

        [Theory]
        [InlineData("not-a-url")]
        [InlineData("www.meta.com")]
        [InlineData("ftp//meta.com")]
        [InlineData("meta")]
        public void CreateCompany_ShouldFail_WhenWebsiteUrlIsInvalid(string website)
        {
            var result = CompanyFactory.CreateCompany(website: website);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.WebsiteInvalid, result.TopError);
        }

        [Theory]
        [InlineData("https://meta.com")]
        [InlineData("http://meta.com")]
        [InlineData("https://www.meta.com")]
        public void CreateCompany_ShouldSucceed_WithValidWebsiteUrl(string website)
        {
            var result = CompanyFactory.CreateCompany(website: website);

            Assert.False(result.IsError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateCompany_ShouldFail_WhenDescriptionIsEmpty(string? description)
        {
            var result = Company.Create(
                id: Guid.NewGuid(),
                name: "Meta",
                industry: "Technology",
                location: "Cairo, Egypt",
                size: CompanySize.Large_501_1000,
                website: "https://meta.com",
                description: description!,
                foundedYear: 2004);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.DescriptionRequired, result.Errors[0]);
        }

        [Fact]
        public void CreateCompany_ShouldFail_WhenDescriptionExceedsMaxLength()
        {
            var tooLong = new string('A', CareerLensConstants.CompanyDescriptionMaxLenght + 1);

            var result = CompanyFactory.CreateCompany(description: tooLong);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.DescriptionTooLong, result.TopError);
        }

        [Fact]
        public void CreateCompany_ShouldSucceed_WhenDescriptionIsAtMaxLength()
        {
            var maxLength = new string('A', CareerLensConstants.CompanyDescriptionMaxLenght);

            var result = CompanyFactory.CreateCompany(description: maxLength);

            Assert.False(result.IsError);
        }

        [Theory]
        [InlineData(1799)]
        [InlineData(1000)]
        [InlineData(0)]
        [InlineData(-1)]
        public void CreateCompany_ShouldFail_WhenFoundedYearIsBefore1800(int year)
        {
            var result = CompanyFactory.CreateCompany(foundedYear: year);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.FoundedYearInvalid, result.TopError);
        }

        [Fact]
        public void CreateCompany_ShouldFail_WhenFoundedYearIsInFuture()
        {
            var futureYear = DateTime.UtcNow.Year + 1;

            var result = CompanyFactory.CreateCompany(foundedYear: futureYear);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.FoundedYearInvalid, result.TopError);
        }

        [Fact]
        public void CreateCompany_ShouldSucceed_WhenFoundedYearIsCurrentYear()
        {
            var result = CompanyFactory.CreateCompany(foundedYear: DateTime.UtcNow.Year);

            Assert.False(result.IsError);
        }

        [Fact]
        public void CreateCompany_ShouldSucceed_WhenFoundedYearIs1800()
        {
            var result = CompanyFactory.CreateCompany(foundedYear: 1800);

            Assert.False(result.IsError);
        }

        [Fact]
        public void Claim_ShouldSucceed_WhenCompanyIsNotClaimed()
        {
            var company = CompanyFactory.CreateCompany().Value;

            var result = company.Claim();

            Assert.False(result.IsError);
            Assert.True(company.IsClaimed);
        }

        [Fact]
        public void Claim_ShouldFail_WhenCompanyIsAlreadyClaimed()
        {
            var company = CompanyFactory.CreateCompany().Value;
            company.Claim();

            var result = company.Claim();

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.AlreadyClaimed, result.TopError);
        }


        [Fact]
        public void UpdateProfile_ShouldSucceed_WithValidData()
        {
            var company = CompanyFactory.CreateCompany().Value;

            var result = company.UpdateProfile(
                name: "Google",
                industry: "Technology",
                location: "Mountain View, CA",
                size: CompanySize.Large_501_1000,
                website: "https://google.com",
                description: "Google is a global technology company.",
                foundedYear: 1998);

            Assert.False(result.IsError);
            Assert.Equal("Google", company.Name);
            Assert.Equal("Technology", company.Industry);
            Assert.Equal("Mountain View, CA", company.Location);
            Assert.Equal(CompanySize.Large_501_1000, company.Size);
            Assert.Equal("https://google.com", company.Website);
            Assert.Equal("Google is a global technology company.", company.Description);
            Assert.Equal(1998, company.FoundedYear);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void UpdateProfile_ShouldFail_WhenNameIsInvalid(string? name)
        {
            var company = CompanyFactory.CreateCompany().Value;

            var result = company.UpdateProfile(
                name: name!,
                industry: "Technology",
                location: TestConstants.DefaultLocation,
                size: CompanySize.Large_501_1000,
                website: "https://google.com",
                description: "Some description.",
                foundedYear: 2000);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.NameRequired, result.TopError);
        }

        [Theory]
        [InlineData("not-a-url")]
        [InlineData("www.google.com")]
        public void UpdateProfile_ShouldFail_WhenWebsiteIsInvalid(string website)
        {
            var company = CompanyFactory.CreateCompany().Value;

            var result = company.UpdateProfile(
                name: "Google",
                industry: "Technology",
                location: TestConstants.DefaultLocation,
                size: CompanySize.Large_501_1000,
                website: website,
                description: "Some description.",
                foundedYear: 2000);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.WebsiteInvalid, result.TopError);
        }

        [Fact]
        public void UpdateProfile_ShouldFail_WhenDescriptionExceedsMaxLength()
        {
            var company = CompanyFactory.CreateCompany().Value;
            var tooLong = new string('A', CareerLensConstants.CompanyDescriptionMaxLenght + 1);

            var result = company.UpdateProfile(
                name: "Google",
                industry: "Technology",
                location: TestConstants.DefaultLocation,
                size: CompanySize.Large_501_1000,
                website: "https://google.com",
                description: tooLong,
                foundedYear: 2000);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.DescriptionTooLong, result.TopError);
        }

        [Fact]
        public void UpdateProfile_ShouldFail_WhenFoundedYearIsInvalid()
        {
            var company = CompanyFactory.CreateCompany().Value;

            var result = company.UpdateProfile(
                name: "Google",
                industry: "Technology",
                location: TestConstants.DefaultLocation,
                size: CompanySize.Large_501_1000,
                website: "https://google.com",
                description: "Some description.",
                foundedYear: 1799);

            Assert.True(result.IsError);
            Assert.Equal(CompanyErrors.FoundedYearInvalid, result.TopError);
        }


        [Fact]
        public void AddCompanyClaimRequest_ShouldAddRequestToCollection()
        {
            var company = CompanyFactory.CreateCompany().Value;
            var claimRequest = CompanyClaimRequestFactory.CreateCompanyClaimRequest();

            var result = company.AddCompanyClaimRequest(claimRequest.Value);

            Assert.False(result.IsError);
            Assert.Single(company.Requests);
        }

        [Fact]
        public void AddCompanyClaimRequest_ShouldAddAllRequests_WhenMultipleAdded()
        {
            var company = CompanyFactory.CreateCompany().Value;

            company.AddCompanyClaimRequest(CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value);
            company.AddCompanyClaimRequest(CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value);
            company.AddCompanyClaimRequest(CompanyClaimRequestFactory.CreateCompanyClaimRequest().Value);

            Assert.Equal(3, company.Requests.Count);
        }
    }
}
