using CareerLens.Application.Features.CompanyClaimRequests.Commands.Admin.UpdateState;
using CareerLens.Application.Features.CompanyClaimRequests.Commands.Employer.CreateCompanyClaimRequest;
using CareerLens.Application.Features.CompanyClaimRequests.Commands.Employer.UpdateCompanyClaimRequest;
using CareerLens.Application.Features.CompanyClaimRequests.Dtos;
using CareerLens.Application.Features.CompanyClaimRequests.Queries.Admin.GetClaimRequestById;
using CareerLens.Application.Features.CompanyClaimRequests.Queries.Admin.GetClaimRequestes;
using CareerLens.Application.Features.CompanyClaimRequests.Queries.Employer.GetClaimRequestById;
using CareerLens.Application.Features.CompanyClaimRequests.Queries.Employer.GetEmployerClaimRequestes;
using CareerLens.Contracts.Requests.Companies;
using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.DomainUsers.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerLens.Api.Controllers
{
    [Route("api/v{version:apiVersion}/ClaimRequests")]
    [ApiVersion("1.0")]
    [ApiController]
    public sealed class ClaimRequestsController(ISender sender) : ApiController
    {

        #region Create a Company Claim Request

        [HttpPost]
        [Authorize(Roles = nameof(Role.Employer))]
        [EndpointSummary("Create a company claim request.")]
        [EndpointDescription("Submits a new claim request to join a company with a specific role.")]
        [EndpointName("CreateCompanyClaimRequest")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ICompanyClaimRequestResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateClaimRequest([FromBody] CreateCompanyClaimRequestRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCompanyClaimRequestCommand(
                request.CompanyId,
                request.AdminNote,
                (CompanyMemberRole)(int)request.CompanyMemberRole);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                onValue: claimRequest => CreatedAtAction(
                    nameof(GetClaimRequestById),
                    new { claimRequestId = claimRequest.Id },
                    claimRequest),
                onError: Problem);
        }

        #endregion

        #region Update a Company Claim Request

        [HttpPut("{claimRequestId:guid}")]
        [Authorize(Roles = nameof(Role.Employer))]
        [EndpointSummary("Update a company claim request.")]
        [EndpointDescription("Updates a pending claim request.")]
        [EndpointName("UpdateClaimRequest")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateClaimRequest(Guid claimRequestId, [FromBody] UpdateCompanyClaimRequestRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCompanyClaimRequestCommand(
                claimRequestId,
                request.AdminNote,
                (CompanyMemberRole)(int)request.CompanyMemberRole);

            var result = await sender.Send(command, cancellationToken);
            return result.Match(onValue: _ => NoContent(), onError: Problem);
        }

        #endregion

        #region Update claim request state

        [HttpPatch("{claimRequestId:guid}/state")]
        [Authorize(Roles = nameof(Role.Admin))]
        [EndpointSummary("Update claim request state.")]
        [EndpointDescription("Approves or rejects a pending claim request.")]
        [EndpointName("UpdateClaimRequestState")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateClaimRequestState(Guid claimRequestId, [FromBody] UpdateClaimRequestStateRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateClaimRequestStateCommand(
                claimRequestId,
                (ClaimStatus)(int)request.ClaimStatus);

            var result = await sender.Send(command, cancellationToken);
            return result.Match(onValue: _ => NoContent(), onError: Problem);
        }

        #endregion

        #region Get Claim Request By For Emloyer

        [HttpGet("{claimRequestId:guid}")]
        [Authorize(Roles = nameof(Role.Employer))]
        [EndpointSummary("Get claim request by Id.")]
        [EndpointDescription("Returns claim request details for the employer.")]
        [EndpointName("GetClaimRequestById")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ICompanyClaimRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetClaimRequestById(Guid claimRequestId,CancellationToken cancellationToken)
        {
            var query = new GetClaimRequestByIdForEmployerQuery(claimRequestId);
            var result = await sender.Send(query, cancellationToken);
            return result.Match(onValue: Ok, onError: Problem);
        }

        #endregion

        #region Get Claim Request By For Admin

        [HttpGet("{claimRequestId:guid}/details")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get claim request by Id for admin.")]
        [EndpointDescription("Returns full claim request details for admin.")]
        [EndpointName("GetClaimRequestByIdForAdmin")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ICompanyClaimRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetClaimRequestByIdForAdmin(Guid claimRequestId, CancellationToken cancellationToken)
        {
            var query = new GetClaimRequestByIdForAdminQuery(claimRequestId);
            var result = await sender.Send(query, cancellationToken);
            return result.Match(onValue: Ok, onError: Problem);
        }

        #endregion

        #region Get All Claim Request For a Company for Employer

        [HttpGet]
        [Authorize(Roles = nameof(Role.Employer))]
        [EndpointSummary("Get my claim requests.")]
        [EndpointDescription("Returns all claim requests for the current employer by company Id.")]
        [EndpointName("GetEmployerClaimRequests")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(List<ICompanyClaimRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEmployerClaimRequests([FromQuery] Guid companyId,CancellationToken cancellationToken)
        {
            var query = new GetEmployerClaimRequestQuery(companyId);
            var result = await sender.Send(query, cancellationToken);
            return result.Match(onValue: Ok, onError: Problem);
        }

        #endregion

        #region Get All Claim Request For a Company for Admin

        [HttpGet("all")]
        [Authorize(Roles = nameof(Role.Admin))]
        [EndpointSummary("Get all claim requests for a company.")]
        [EndpointDescription("Returns all claim requests for a specific company for admin.")]
        [EndpointName("GetClaimRequests")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(List<ICompanyClaimRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetClaimRequests([FromQuery] Guid companyId, CancellationToken cancellationToken)
        {
            var query = new GetClaimRequestsQuery(companyId);
            var result = await sender.Send(query, cancellationToken);
            return result.Match(onValue: Ok, onError: Problem);
        }

        #endregion

    }
}
