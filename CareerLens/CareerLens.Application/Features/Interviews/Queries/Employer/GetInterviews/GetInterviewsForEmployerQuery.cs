using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Common.Models;
using CareerLens.Application.Features.Interviews.Dtos;
using CareerLens.Application.Features.Interviews.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Interviews.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.Queries.Employer.GetInterviews
{
    [RequireRole("Employer")]
    public sealed record GetInterviewsForEmployerQuery(Guid CompanyId,
                                                       int Page,
                                                       int PageSize,
                                                       InterviewStatus? Status = null)
    : IRequest<Result<PaginatedList<IInterviewResponse>>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles =>
        [
            CompanyMemberRole.Moderator
        ];
    }

    public sealed class GetInterviewsForEmployerQueryValidator
        : AbstractValidator<GetInterviewsForEmployerQuery>
    {
        private static readonly InterviewStatus[] AllowedStatuses =
        [
            InterviewStatus.Pending,
            InterviewStatus.Approved
        ];

        public GetInterviewsForEmployerQueryValidator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company ID is required.");

            RuleFor(x => x.Status)
                .Must(s => AllowedStatuses.Contains(s!.Value))
                .When(x => x.Status.HasValue)
                .WithMessage("Status filter must be Pending or Approved.");
        }
    }

    public sealed class GetInterviewsForEmployerQueryHandler(IAppDbContext context)
        : IRequestHandler<GetInterviewsForEmployerQuery, Result<PaginatedList<IInterviewResponse>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<PaginatedList<IInterviewResponse>>> Handle(
            GetInterviewsForEmployerQuery request,
            CancellationToken cancellationToken)
        {

            var baseQuery = _context.Interviews
                .Where(i => i.CompanyId == request.CompanyId);

            if (request.Status.HasValue)
            {
                baseQuery = baseQuery
                    .Where(i => i.InterviewStatus == request.Status.Value);
            }
            else
            {
                baseQuery = baseQuery
                    .Where(i => i.InterviewStatus == InterviewStatus.Approved);
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return new PaginatedList<IInterviewResponse>
                {
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Items = []
                };
            }

            var interviews = await baseQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(i => i.User)
                .Include(i => i.InterviewQuestions)
                .ToListAsync(cancellationToken);

            var items = interviews
                .Select(i => (IInterviewResponse)i.ToEmployerDto())
                .ToList()
                .AsReadOnly();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedList<IInterviewResponse>
            {
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items
            };
        }
    }
}
