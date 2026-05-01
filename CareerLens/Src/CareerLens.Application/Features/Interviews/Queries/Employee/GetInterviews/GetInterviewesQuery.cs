using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Interviews.Dtos;
using CareerLens.Application.Features.Interviews.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Interviews.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.Queries.Employee.GetInterviews
{
    [RequireRole("Employee")]
    public sealed record GetInterviewsForEmployeeQuery(Guid CompanyId, int Page, int PageSize)
        : IRequest<Result<PaginatedList<IInterviewResponse>>>;


    public sealed class GetInterviewsForEmployerQueryValidator
    : AbstractValidator<GetInterviewsForEmployeeQuery>
    {
        public GetInterviewsForEmployerQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");
        }
    }

    public class GetInterviewsForEmployeeQueryQueryHandler(IAppDbContext context)
        : IRequestHandler<GetInterviewsForEmployeeQuery, Result<PaginatedList<IInterviewResponse>>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<PaginatedList<IInterviewResponse>>> Handle(GetInterviewsForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Interviews
                .AsNoTracking()
                .Include(i => i.User)
                .Include(i => i.InterviewQuestions)
                .Where(i => i.CompanyId == request.CompanyId)
                .Where(i => i.InterviewStatus == InterviewStatus.Approved);

            var totalCount = await query.CountAsync(cancellationToken);

            var interviews = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = interviews
                .Select(i => (IInterviewResponse)i.ToBasicDto())
                .ToList()
                .AsReadOnly();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var result = new PaginatedList<IInterviewResponse>
            {
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            };

            return result;
        }
    }
}
