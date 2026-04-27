using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Interviews.Commands.Employee.CreateInterview;
using CareerLens.Application.Features.Interviews.Commands.Employee.RemoveInterview;
using CareerLens.Application.Features.Interviews.Commands.Employee.UpdateInterview;
using CareerLens.Application.Features.Interviews.Commands.Employer.UpdateState;
using CareerLens.Application.Features.Interviews.Dtos;
using CareerLens.Application.Features.Interviews.Queries.Employee.GetInterviewById;
using CareerLens.Application.Features.Interviews.Queries.Employee.GetInterviews;
using CareerLens.Application.Features.Interviews.Queries.Employer.GetInterviewById;
using CareerLens.Application.Features.Interviews.Queries.Employer.GetInterviews;
using CareerLens.Contracts.Requests.Interviewes;
using CareerLens.Contracts.Requests.Interviews;
using CareerLens.Domain.DomainUsers.Enums;
using CareerLens.Domain.Interviews.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerLens.Api.Controllers
{
    [Authorize]
    [Route("api/v1/interviews")]
    public sealed class InterviewsController : ApiController
    {
        private readonly ISender _sender;

        public InterviewsController(ISender sender)
        {
            _sender = sender;
        }

        #region Create Interview
        [HttpPost]
        [Authorize(Roles = (nameof(Role.Employee)))]
        [ProducesResponseType(typeof(InterviewBasicDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateInterview(
            [FromBody] CreateInterviewRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateInterviewCommand(
                CompanyId: request.CompanyId,
                OverallExperience: request.OverallExperience,
                InterviewDifficulty: request.InterviewDifficulty,
                GettingOffer: request.GettingOffer,
                JobTitle: request.JobTitle!,
                Description: request.Description!,
                Source: request.Source,
                HelpingLevel: request.HelpingLevel,
                Location: request.Location,
                DurationValue: request.DurationValue,
                DurationUnit: request.DurationUnit,
                DateYear: request.DateYear,
                DateMonth: request.DateMonth,
                Stages: request.Stages,
                Questions: request.Questions
                                            .Select(q => new CreateInterviewQuestionCommand(
                                                q.QuestionText!,
                                                q.Answer))
                                            .ToList()
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(
                    actionName: nameof(CreateInterview),
                    value: result.Value);
        }
        #endregion

        #region Delete Interview
        [Authorize(Roles =(nameof(Role.Employee)))]
        [HttpDelete("{interviewId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveInterview(
            [FromRoute] Guid interviewId,
            CancellationToken cancellationToken)
        {
            var command = new RemoveInterviewCommand(interviewId);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : NoContent();
        }
        #endregion

        #region Update Interview
        [Authorize(Roles =(nameof (Role.Employee)))]
        [HttpPut("{interviewId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateInterview(
            [FromRoute] Guid interviewId,
            [FromBody] UpdateInterviewRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateInterviewCommand(
                InterviewId: interviewId,
                OverallExperience: request.OverallExperience,
                InterviewDifficulty: request.InterviewDifficulty,
                GettingOffer: request.GettingOffer,
                JobTitle: request.JobTitle,
                Description: request.Description,
                Source: request.Source,
                HelpingLevel: request.HelpingLevel,
                Questions: request.Questions
                                            .Select(q => new UpdateInterviewQuestionCommand(
                                                q.QuestionId,
                                                q.QuestionText,
                                                q.Answer))
                                            .ToList(),
                Location: request.Location,
                DurationValue: request.DurationValue,
                DurationUnit: request.DurationUnit,
                DateYear: request.DateYear,
                DateMonth: request.DateMonth,
                Stages: request.Stages
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }
        #endregion

        #region Update Interview state
        [Authorize(Roles = (nameof(Role.Employer)))]
        [HttpPatch("{interviewId:guid}/companies/{companyId:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateInterviewState(
            [FromRoute] Guid interviewId,
            [FromRoute] Guid companyId,
            [FromBody] UpdateInterviewStateRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateInterviewStateCommand(
                InterviewId: interviewId,
                CompanyId: companyId,
                InterviewStatus: request.InterviewStatus
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }
        #endregion

        #region Get Interview By Id for Employee
        [Authorize(Roles =(nameof(Role.Employee)))]
        [HttpGet("{interviewId:guid}")]
        [ProducesResponseType(typeof(InterviewBasicDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetInterviewByIdForEmployee(
            [FromRoute] Guid interviewId,
            CancellationToken cancellationToken)
        {
            var query = new GetInterviewByIdForEmployeeQuery(interviewId);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Get Interview By Id for Employee
        [HttpGet("{interviewId:guid}/companies/{companyId:guid}")]
        [ProducesResponseType(typeof(InterviewEmployerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetInterviewByIdForEmployer(
            [FromRoute] Guid interviewId,
            [FromRoute] Guid companyId,
            CancellationToken cancellationToken)
        {
            var query = new GetInterviewByIdForEmployerQuery(
                CompanyId: companyId,
                InterviewId: interviewId
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Get All approved interview 
        [Authorize(Roles =(nameof(Role.Employee)))]
        [HttpGet("companies/{companyId:guid}")]
        [ProducesResponseType(typeof(PaginatedList<IInterviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetInterviewsForEmployee(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetInterviewsForEmployeeQuery(
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

        #region Get All  interview for employer
        [Authorize(Roles = (nameof(Role.Employer)))]
        [HttpGet("companies/{companyId:guid}/manage")]
        [ProducesResponseType(typeof(PaginatedList<IInterviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetInterviewsForEmployer(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] InterviewStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetInterviewsForEmployerQuery(
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
