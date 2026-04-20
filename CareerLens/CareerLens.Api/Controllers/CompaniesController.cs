using CareerLens.Application.Features.Companies.Commands.Admin.CreateCompany;
using CareerLens.Application.Features.Companies.Commands.Admin.RemoveCompany;
using CareerLens.Application.Features.Companies.Commands.Employer.UpdateCompany;
using CareerLens.Application.Features.Companies.Dtos;
using CareerLens.Application.Features.Companies.Queries.Admin.GetCompanyById;
using CareerLens.Application.Features.Companies.Queries.GetCompanyById;
using CareerLens.Contracts.Requests.Companies;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.Enums;
using CareerLens.Domain.DomainUsers.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerLens.Api.Controllers
{
    [Route("api/v{version:apiVersion}/Companies")]
    [ApiVersion("1.0")]
    [ApiController]
    public sealed class CompaniesController(ISender sender) : ApiController
    {

        #region Create Company
        [HttpPost]
        [Authorize(Roles = nameof(Role.Admin))]
        [EndpointSummary("Create new company.")]
        [EndpointDescription("Create new company.")]
        [EndpointName("GetCustomers")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ICompanyResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCompanyCommand(request.Name, request.Industry, request.Location, request.Website, request.Description, request.FoundedYear, (CompanySize)(int)request.Size);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                onValue: company => CreatedAtAction(
                    nameof(CreateCompany),
                    new { id = company.CompanyId },
                    company),
                onError: Problem);
        }
        #endregion

        #region Delete Company
        [HttpDelete("{companyId:guid}")]
        [Authorize(Roles = nameof(Role.Admin))]
        [EndpointSummary("Remove a company.")]
        [EndpointDescription("Removes an existing company by its Id.")]
        [EndpointName("RemoveCompany")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveCompany(Guid companyId, CancellationToken cancellationToken)
        {
            var command = new RemoveCompanyCommand(companyId);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                onValue: _ => NoContent(),
                onError: Problem);
        }
        #endregion

        #region Update Company

        [HttpPut("{companyId:guid}")]
        [Authorize(Roles = nameof(Role.Employer))]
        [EndpointSummary("Update a company.")]
        [EndpointDescription("Updates an existing company profile by its Id.")]
        [EndpointName("UpdateCompany")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateCompany(Guid companyId,[FromBody] UpdateCompanyRequest request,CancellationToken cancellationToken)
        {
            var command = new UpdateCompanyCommand(
                companyId,
                request.Name,
                request.Industry,
                request.Location,
                request.Website,
                request.Description,
                request.FoundedYear,
                (CompanySize)(int)request.Size);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                onValue: _ => NoContent(),
                onError: Problem);
        }

        #endregion

        #region Get By Id For All

        [HttpGet("{companyId:guid}")]
        [Authorize]
        [EndpointSummary("Get company by Id.")]
        [EndpointDescription("Returns basic company information by its Id.")]
        [EndpointName("GetCompanyById")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CompanyBasicDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCompanyById(
        Guid companyId,
        CancellationToken cancellationToken)
        {
            var query = new GetCompanyByIdQuery(companyId);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                onValue: Ok,
                onError: Problem);
        }

        #endregion

        #region Get By Id For Admin Only

        [HttpGet("{companyId:guid}/admin")]
        [Authorize(Roles = nameof(Role.Admin))]
        [EndpointSummary("Get company by Id for admin.")]
        [EndpointDescription("Returns full company information by its Id for admin.")]
        [EndpointName("GetCompanyByIdForAdmin")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CompanyAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCompanyByIdForAdmin(Guid companyId,CancellationToken cancellationToken)
        {
            var query = new GetCompanyByIdForAdminQuery(companyId);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                onValue: Ok,
                onError: Problem);
        }

        #endregion
    }
}
