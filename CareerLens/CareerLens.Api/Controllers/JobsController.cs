using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Jobs.Commands.Employer.CreateJob;
using CareerLens.Application.Features.Jobs.Commands.Employer.UpdateJob;
using CareerLens.Application.Features.Jobs.Commands.Employer.UpdateState;
using CareerLens.Application.Features.Jobs.Dtos;
using CareerLens.Application.Features.Jobs.Queries.Employee.GetCompanyJobs;
using CareerLens.Application.Features.Jobs.Queries.Employee.GetJobById;
using CareerLens.Application.Features.Jobs.Queries.Employer.GetCompanyJobs;
using CareerLens.Application.Features.Jobs.Queries.Employer.GetJobById;
using CareerLens.Contracts.Requests.Jobs;
using CareerLens.Domain.DomainUsers.Enums;
using CareerLens.Domain.Jobs.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerLens.Api.Controllers
{
    [Authorize]
    [Route("api/v1/companies/{companyId:guid}/jobs")]
    public sealed class JobsController : ApiController
    {
        private readonly ISender _sender;

        public JobsController(ISender sender)
        {
            _sender = sender;
        }

        #region Create Job
        [Authorize(Roles =(nameof(Role.Employer)))]
        [HttpPost]
        [ProducesResponseType(typeof(IJobResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> CreateJob(
            [FromRoute] Guid companyId,
            [FromBody] CreateJobRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateJobCommand(
                CompanyId: companyId,
                Title: request.Title,
                Description: request.Description,
                Location: request.Location,
                EmploymentType: request.EmploymentType,
                WorkplaceType: request.WorkplaceType,
                ExperienceLevel: request.ExperienceLevel,
                MinSalary: request.MinSalary,
                MaxSalary: request.MaxSalary,
                PayPeriod: request.PayPeriod,
                ApplyUrl: request.ApplyUrl
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(
                    actionName: nameof(CreateJob),
                    routeValues: new { companyId },
                    value: result.Value);
        }

        #endregion

        #region Update Job
        [Authorize(Roles = (nameof(Role.Employer)))]
        [HttpPut("{jobId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateJob(
            [FromRoute] Guid companyId,
            [FromRoute] Guid jobId,
            [FromBody] UpdateJobRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateJobCommand(
                JobId: jobId,
                CompanyId: companyId,
                Title: request.Title,
                Description: request.Description,
                Location: request.Location,
                EmploymentType: request.EmploymentType,
                WorkplaceType: request.WorkplaceType,
                ExperienceLevel: request.ExperienceLevel,
                MinSalary: request.MinSalary,
                MaxSalary: request.MaxSalary,
                PayPeriod: request.PayPeriod,
                ApplyUrl: request.ApplyUrl
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }

        #endregion

        #region Update State

        [Authorize(Roles =(nameof(Role.Employer)))]
        [HttpPatch("{jobId:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateJobStatus(
            [FromRoute] Guid companyId,
            [FromRoute] Guid jobId,
            [FromBody] UpdateJobStatusRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateJobStatusCommand(
                JobId: jobId,
                CompanyId: companyId,
                Status: request.Status
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }
        #endregion

        #region Get Job By Id Employee

        [HttpGet("{jobId:guid}")]
        [ProducesResponseType(typeof(IJobResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetJobByIdForEmployee(
            [FromRoute] Guid jobId,
            CancellationToken cancellationToken)
        {
            var query = new GetJobByIdForEmployeeQuery(jobId);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }

        #endregion

        #region Get Job By Id Employer

        [HttpGet("{jobId:guid}/manage")]
        [ProducesResponseType(typeof(IJobResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetJobByIdForEmployer(
            [FromRoute] Guid companyId,
            [FromRoute] Guid jobId,
            CancellationToken cancellationToken)
        {
            var query = new GetJobByIdForEmployerQuery(
                JobId: jobId,
                CompanyId: companyId
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }

        #endregion

        #region Get Jobs By Employee

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<IJobResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCompanyJobsForEmployee(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCompanyJobsForEmployeeQuery(
                CompanyId: companyId,
                Page: page,
                PageSize: pageSize
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }

        #endregion

        #region Get Jobs By Employer

        [HttpGet("manage")]
        [ProducesResponseType(typeof(PaginatedList<IJobResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCompanyJobsForEmployer(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] JobStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCompanyJobsForEmployerQuery(
                CompanyId: companyId,
                Page: page,
                PageSize: pageSize,
                Status: status
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }

        #endregion
    }
}
