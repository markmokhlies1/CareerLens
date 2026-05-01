using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.CompanyClaimRequests.Dtos;
using CareerLens.Application.Features.CompanyClaimRequests.Mappers;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequests.Queries.Admin.GetClaimRequestes
{
    [RequireRole("Admin")]
    public sealed record GetClaimRequestsQuery(Guid CompanyId) 
        : IRequest<Result<List<ICompanyClaimRequestResponse>>>;

    public sealed class GetClaimRequestsQueryValidator : AbstractValidator<GetClaimRequestsQuery>
    {
        public GetClaimRequestsQueryValidator() 
        {
            RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("CompanyId is required.");
        }
    }
    public sealed class GetClaimRequestsQueryHandler(IAppDbContext context)
        : IRequestHandler<GetClaimRequestsQuery, Result<List<ICompanyClaimRequestResponse>>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<List<ICompanyClaimRequestResponse>>> Handle(GetClaimRequestsQuery request, CancellationToken cancellationToken)
        {
            var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var claimRequests = await _context.CompanyClaimsRequests
                .Include(c => c.Company)
                .Include(c => c.User)
                .Where(c => c.CompanyId == request.CompanyId)
                .OrderByDescending(c => c.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return claimRequests.ToAdminDtos();
        }
    }
}
