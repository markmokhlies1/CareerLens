using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.CompanyClaimRequests.Dtos;
using CareerLens.Application.Features.CompanyClaimRequests.Mappers;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using FluentValidation.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequests.Queries.Employer.GetEmployerClaimRequestes
{
    [RequireRole("Employer")]
    public sealed record GetEmployerClaimRequestQuery(Guid CompanyId)
        : IRequest<Result<List<ICompanyClaimRequestResponse>>>;

    public sealed class GetEmployerClaimRequestQueryValidator : AbstractValidator<GetEmployerClaimRequestQuery>
    {
        public GetEmployerClaimRequestQueryValidator()
        {
             RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("CompanyId is required.");
        }
    }
    public sealed class GetEmployerClaimRequestQueryHandler(IAppDbContext context,IUser user)
        : IRequestHandler<GetEmployerClaimRequestQuery, Result<List<ICompanyClaimRequestResponse>>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = user;
        public async Task<Result<List<ICompanyClaimRequestResponse>>> Handle(GetEmployerClaimRequestQuery request, CancellationToken cancellationToken)
        {
    

            var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

             var claimRequests = await _context.CompanyClaimsRequests
            .Where(c => c.CompanyId == request.CompanyId && c.UserId == userId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

            return claimRequests.ToEmployerDtos();
        }
    }
}
