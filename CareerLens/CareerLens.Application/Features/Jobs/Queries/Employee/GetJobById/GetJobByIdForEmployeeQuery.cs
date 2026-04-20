using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Jobs.Dtos;
using CareerLens.Application.Features.Jobs.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Jobs;
using CareerLens.Domain.Jobs.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Jobs.Queries.Employee.GetJobById
{
    [RequireRole("Employee")]
    public sealed record GetJobByIdForEmployeeQuery(Guid JobId) : IRequest<Result<IJobResponse>>;
    public sealed class GetJobByIdForEmployeeQueryValidator : AbstractValidator<GetJobByIdForEmployeeQuery>
    {
        public GetJobByIdForEmployeeQueryValidator()
        {
            RuleFor(x => x.JobId)
            .NotEmpty()
            .WithMessage("Job ID is required.");
        }
    }
    public sealed class GetJobByIdForEmployeeQueryHandler (IAppDbContext context)
        : IRequestHandler<GetJobByIdForEmployeeQuery, Result<IJobResponse>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<IJobResponse>> Handle(GetJobByIdForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var job = await _context.Jobs
            .AsNoTracking()
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == request.JobId
                                   && j.Status == JobStatus.Published, cancellationToken);

            if (job is null)
            {
                return ApplicationErrors.JobNotFound;
            }

            return job.ToBasicDto();
        }
    }
}
