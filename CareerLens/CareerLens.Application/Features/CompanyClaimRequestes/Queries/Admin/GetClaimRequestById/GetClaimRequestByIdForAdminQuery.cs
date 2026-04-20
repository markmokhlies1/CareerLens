using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.CompanyClaimRequests.Dtos;
using CareerLens.Application.Features.CompanyClaimRequests.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequests.Queries.Admin.GetClaimRequestById
{
    [RequireRole("Admin")]
    public sealed record GetClaimRequestByIdForAdminQuery(Guid ClaimRequestId) 
        : IRequest<Result<ICompanyClaimRequestResponse>>;

    public sealed class GetClaimRequestByIdQueryValidator : AbstractValidator<GetClaimRequestByIdForAdminQuery>
    {
        public GetClaimRequestByIdQueryValidator()
        {
            RuleFor(c => c.ClaimRequestId)
            .NotEmpty()
            .WithMessage("Claim Request Id is required.");
        }
    }
    public sealed class GetClaimRequestByIdQueryForAdminHandler (IAppDbContext context)
        : IRequestHandler<GetClaimRequestByIdForAdminQuery, Result<ICompanyClaimRequestResponse>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<ICompanyClaimRequestResponse>> Handle(GetClaimRequestByIdForAdminQuery request, CancellationToken cancellationToken)
        {
            var claimRequest = await _context.CompanyClaimsRequests
            .Include(c => c.Company)  
            .Include(c => c.User)     
            .FirstOrDefaultAsync(c => c.Id == request.ClaimRequestId, cancellationToken);

            if (claimRequest is null)
            {
                return ApplicationErrors.ClaimRequestNotFound;
            }

            return claimRequest.ToAdminDto();
        }
    }
}
