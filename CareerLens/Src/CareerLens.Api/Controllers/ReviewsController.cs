using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Reviews.Commands.Employee.CreateReview;
using CareerLens.Application.Features.Reviews.Commands.Employee.RemoveReview;
using CareerLens.Application.Features.Reviews.Commands.Employee.UpdateReview;
using CareerLens.Application.Features.Reviews.Commands.Employer.UpdateReviewState;
using CareerLens.Application.Features.Reviews.Dtos;
using CareerLens.Application.Features.Reviews.Queries.Employee.GetReviewById;
using CareerLens.Application.Features.Reviews.Queries.Employee.GetReviews;
using CareerLens.Application.Features.Reviews.Queries.Employer.GetReviewById;
using CareerLens.Application.Features.Reviews.Queries.Employer.GetReviews;
using CareerLens.Contracts.Requests.Reviews;
using CareerLens.Domain.DomainUsers.Enums;
using CareerLens.Domain.Reviews.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerLens.Api.Controllers
{
    [Authorize]
    [Route("api/v1/companies/{companyId:guid}/reviews")]
    public class ReviewsController : ApiController
    {
        private readonly ISender _sender;

        public ReviewsController(ISender sender)
        {
            _sender = sender;
        }

        #region Create Review
        [HttpPost]
        [Authorize(Roles =(nameof(Role.Employee)))]
        [ProducesResponseType(typeof(IReviewResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> CreateReview(
            [FromRoute] Guid companyId,
            [FromBody] CreateReviewRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateReviewCommand(
                CompanyId: companyId,
                OverallRating: request.OverallRating,
                EmploymentStatus: request.EmploymentStatus,
                JobFunction: request.JobFunction,
                LengthOfEmployment: request.LengthOfEmployment,
                Headline: request.Headline,
                Pros: request.Pros,
                Cons: request.Cons,
                EmployeeType: request.EmployeeType,
                JobTitle: request.JobTitle,
                AdviceToManagement: request.AdviceToManagement,
                CareerOpportunities: request.CareerOpportunities,
                CompensationAndBenefits: request.CompensationAndBenefits,
                CultureAndValues: request.CultureAndValues,
                DiversityAndInclusion: request.DiversityAndInclusion,
                SeniorManagement: request.SeniorManagement,
                WorkLifeBalance: request.WorkLifeBalance,
                CeoRating: request.CeoRating,
                RecommendToFriend: request.RecommendToFriend,
                BusinessOutlook: request.BusinessOutlook,
                Location: request.Location
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(
                    actionName: nameof(CreateReview),
                    routeValues: new { companyId },
                    value: result.Value);
        }
        #endregion

        #region Update Review

        [HttpPut("{reviewId:guid}")]
        [Authorize(Roles = (nameof(Role.Employee)))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateReview(
            [FromRoute] Guid companyId,
            [FromRoute] Guid reviewId,
            [FromBody] UpdateReviewRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateReviewCommand(
                ReviewId: reviewId,
                OverallRating: request.OverallRating,
                EmploymentStatus: request.EmploymentStatus,
                JobFunction: request.JobFunction,
                LengthOfEmployment: request.LengthOfEmployment,
                Headline: request.Headline,
                Pros: request.Pros,
                Cons: request.Cons,
                EmployeeType: request.EmployeeType,
                JobTitle: request.JobTitle,
                AdviceToManagement: request.AdviceToManagement,
                CareerOpportunities: request.CareerOpportunities,
                CompensationAndBenefits: request.CompensationAndBenefits,
                CultureAndValues: request.CultureAndValues,
                DiversityAndInclusion: request.DiversityAndInclusion,
                SeniorManagement: request.SeniorManagement,
                WorkLifeBalance: request.WorkLifeBalance,
                CeoRating: request.CeoRating,
                RecommendToFriend: request.RecommendToFriend,
                BusinessOutlook: request.BusinessOutlook,
                Location: request.Location
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }

        #endregion

        #region Remove Interview

        [HttpDelete("{reviewId:guid}")]
        [Authorize(Roles = (nameof(Role.Employee)))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveReview(
            [FromRoute] Guid companyId,
            [FromRoute] Guid reviewId,
            CancellationToken cancellationToken)
        {
            var command = new RemoveReviewCommand(reviewId);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : NoContent();
        }

        #endregion

        #region Update State

        [HttpPatch("{reviewId:guid}/status")]
        [Authorize(Roles = (nameof(Role.Employer)))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateReviewStatus(
            [FromRoute] Guid companyId,
            [FromRoute] Guid reviewId,
            [FromBody] UpdateReviewStatusRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateReviewStatusCommand(
                ReviewId: reviewId,
                CompanyId: companyId,
                Status: request.Status
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }

        #endregion

        #region Get By Id Public
        [HttpGet("{reviewId:guid}")]
        [ProducesResponseType(typeof(IReviewResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetReviewByIdForEmployee(
            [FromRoute] Guid reviewId,
            CancellationToken cancellationToken)
        {
            var query = new GetReviewByIdForEmployeeQuery(reviewId);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Get By Id Employer
        [HttpGet("{reviewId:guid}/manage")]
        [Authorize(Roles = (nameof(Role.Employer)))]
        [ProducesResponseType(typeof(IReviewResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetReviewByIdForEmployer(
            [FromRoute] Guid companyId,
            [FromRoute] Guid reviewId,
            CancellationToken cancellationToken)
        {
            var query = new GetReviewByIdForEmployerQuery(
                ReviewId: reviewId,
                CompanyId: companyId
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Get All Public
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<IReviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetReviewsForEmployee(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetReviewsForEmployeeQuery(
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

        #region Get All Employer
        [HttpGet("manage")]
        [Authorize(Roles = (nameof(Role.Employer)))]
        [ProducesResponseType(typeof(PaginatedList<IReviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetReviewsForEmployer(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ReviewStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetReviewsForEmployerQuery(
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
