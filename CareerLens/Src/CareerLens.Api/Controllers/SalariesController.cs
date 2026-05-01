using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Salaries.Commands.Employee.CreateSalary;
using CareerLens.Application.Features.Salaries.Commands.Employee.RemoveSalary;
using CareerLens.Application.Features.Salaries.Commands.Employee.UpdateSalary;
using CareerLens.Application.Features.Salaries.Commands.Employer.UpdateSalaryState;
using CareerLens.Application.Features.Salaries.Dtos;
using CareerLens.Application.Features.Salaries.Queries.Employee.GetCompanySalaries;
using CareerLens.Application.Features.Salaries.Queries.Employee.GetSalaryById;
using CareerLens.Application.Features.Salaries.Queries.Employer.GetCompanySalries;
using CareerLens.Application.Features.Salaries.Queries.Employer.GetSalaryById;
using CareerLens.Contracts.Requests.Salaries;
using CareerLens.Domain.DomainUsers.Enums;
using CareerLens.Domain.Salaries;
using CareerLens.Domain.Salaries.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerLens.Api.Controllers
{
    [Authorize]
    [Route("api/v1/companies/{companyId:guid}/salaries")]
    public class SalariesController : ApiController
    {
        private readonly ISender _sender;

        public SalariesController(ISender sender)
        {
            _sender = sender;
        }

        #region Create Salary
        [HttpPost]
        [Authorize(Roles =(nameof(Role.Employee)))]
        [ProducesResponseType(typeof(ISalaryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateSalary(
            [FromRoute] Guid companyId,
            [FromBody] CreateSalaryRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateSalaryCommand(
                CompanyId: companyId,
                JobTitle: request.JobTitle,
                EmployeeType: request.EmployeeType,
                EmploymentStatus: request.EmploymentStatus,
                LengthOfEmployment: request.LengthOfEmployment,
                Location: request.Location,
                BasePay: ToMoneyDto(request.BasePay)!,
                SalaryPeriod: request.SalaryPeriod,
                Year: request.Year,
                Bonus: ToMoneyDto(request.Bonus),
                Stock: ToMoneyDto(request.Stock),
                ProfitSharing: ToMoneyDto(request.ProfitSharing),
                Tips: ToMoneyDto(request.Tips),
                Commission: ToMoneyDto(request.Commission)
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(
                    actionName: nameof(CreateSalary),
                    routeValues: new { companyId },
                    value: result.Value);
        }
        #endregion

        #region Update Salary
        [HttpPut("{salaryId:guid}")]
        [Authorize(Roles = (nameof(Role.Employee)))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSalary(
            [FromRoute] Guid companyId,
            [FromRoute] Guid salaryId,
            [FromBody] UpdateSalaryRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateSalaryCommand(
                SalaryId: salaryId,
                JobTitle: request.JobTitle,
                EmployeeType: request.EmployeeType,
                EmploymentStatus: request.EmploymentStatus,
                LengthOfEmployment: request.LengthOfEmployment,
                Location: request.Location,
                BasePay: ToMoneyDto(request.BasePay)!,
                SalaryPeriod: request.SalaryPeriod,
                Year: request.Year,
                Bonus: ToMoneyDto(request.Bonus),
                Stock: ToMoneyDto(request.Stock),
                ProfitSharing: ToMoneyDto(request.ProfitSharing),
                Tips: ToMoneyDto(request.Tips),
                Commission: ToMoneyDto(request.Commission)
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }
        #endregion

        #region Remove Salary
        [HttpDelete("{salaryId:guid}")]
        [Authorize(Roles = (nameof(Role.Employee)))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveSalary(
            [FromRoute] Guid companyId,
            [FromRoute] Guid salaryId,
            CancellationToken cancellationToken)
        {
            var command = new RemoveSalaryCommand(
                SalaryId: salaryId,
                CompanyId: companyId
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : NoContent();
        }
        #endregion

        #region Update State
        [HttpPatch("{salaryId:guid}/status")]
        [Authorize(Roles = (nameof(Role.Employer)))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSalaryStatus(
            [FromRoute] Guid companyId,
            [FromRoute] Guid salaryId,
            [FromBody] UpdateSalaryStatusRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateSalaryStatusCommand(
                SalaryId: salaryId,
                CompanyId: companyId,
                Status: request.Status
            );

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok();
        }
        #endregion

        #region Get All 

        [HttpGet]
        [Authorize(Roles = (nameof(Role.Employer)))]
        [ProducesResponseType(typeof(PaginatedList<ISalaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSalariesForEmployee(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetSalariesForEmployeeQuery(
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
        [ProducesResponseType(typeof(PaginatedList<ISalaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> GetSalariesForEmployer(
            [FromRoute] Guid companyId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] SalaryStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetSalariesForEmployerQuery(
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

        #region Get By Id Employee

        [HttpGet("{salaryId:guid}")]
        [ProducesResponseType(typeof(ISalaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSalaryByIdForEmployee(
            [FromRoute] Guid salaryId,
            CancellationToken cancellationToken)
        {
            var query = new GetSalaryByIdForEmployeeQuery(salaryId);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Get By Id Employer 

        [HttpGet("{salaryId:guid}/manage")]
        [ProducesResponseType(typeof(ISalaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSalaryByIdForEmployer(
            [FromRoute] Guid companyId,
            [FromRoute] Guid salaryId,
            CancellationToken cancellationToken)
        {
            var query = new GetSalaryByIdForEmployerQuery(
                CompanyId: companyId,
                SalaryId: salaryId
            );

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion
        private static MoneyDto? ToMoneyDto(MoneyRequest? request) =>
            request is null
                ? null
                : new MoneyDto(request.Amount, new Currency(request.CurrencyCode));
    }
}
