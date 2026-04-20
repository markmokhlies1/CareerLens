using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Salaries.Dtos;
using CareerLens.Application.Features.Salaries.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Salaries;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Salaries.Queries.Employee.GetSalaryById
{
    [RequireRole("Employee")]
    public sealed record GetSalaryByIdForEmployeeQuery(Guid SalaryId)
    : IRequest<Result<ISalaryResponse>>;

    public sealed class GetSalaryByIdForEmployeeQueryValidator
    : AbstractValidator<GetSalaryByIdForEmployeeQuery>
    {
        public GetSalaryByIdForEmployeeQueryValidator()
        {
            RuleFor(x => x.SalaryId)
                .NotEmpty()
                .WithMessage("Salary ID is required.");
        }
    }

    public sealed class GetSalaryByIdForEmployeeQueryHandler(IAppDbContext context)
    : IRequestHandler<GetSalaryByIdForEmployeeQuery, Result<ISalaryResponse>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<ISalaryResponse>> Handle(GetSalaryByIdForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var salary = await _context.Salaries
                .Include(s => s.Company)
                .FirstOrDefaultAsync(s => s.Id == request.SalaryId,cancellationToken);

            if (salary is null)
            {
                return ApplicationErrors.SalaryNotFound;
            }

            return salary.ToBasicDto();
        }
    }
}
