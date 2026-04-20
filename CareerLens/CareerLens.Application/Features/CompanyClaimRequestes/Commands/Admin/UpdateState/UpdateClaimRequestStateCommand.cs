using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using CareerLens.Domain.Companies.CompanyClaimRequests.Events;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequests.Commands.Admin.UpdateState
{
    [RequireRole("Admin")]
    public sealed record UpdateClaimRequestStateCommand(Guid ClaimRequestId, ClaimStatus ClaimStatus) 
        : IRequest<Result<Updated>>;
    public sealed class UpdateClaimRequestStateCommandValidator : AbstractValidator<UpdateClaimRequestStateCommand>
    {
        public UpdateClaimRequestStateCommandValidator()
        {
            RuleFor(x => x.ClaimRequestId)
            .NotEmpty() 
            .WithMessage("ClaimRequestId is required.");

            RuleFor(x => x.ClaimStatus)
                .IsInEnum()
                .WithMessage("ClaimStatus is invalid.")
                .NotEqual(ClaimStatus.Pending)
                .WithMessage("Cannot set status back to Pending.");
        }
    }
    public sealed class UpdateClaimRequestStateCommandHandler (IAppDbContext context)
        : IRequestHandler<UpdateClaimRequestStateCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Updated>> Handle(UpdateClaimRequestStateCommand command, CancellationToken cancellationToken)
        {
            var claimRequest = await _context.CompanyClaimsRequests
            .FirstOrDefaultAsync(c => c.Id == command.ClaimRequestId, cancellationToken);

            if (claimRequest is null)
            {
                return ApplicationErrors.ClaimRequestNotFound;
            }

            var updateResult = claimRequest.UpdateState(command.ClaimStatus);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }

            if (command.ClaimStatus == ClaimStatus.Approved)
            {
                claimRequest.AddDomainEvent
                    (new CompanyClaimRequestApproved(claimRequest.Id, claimRequest.CompanyId, claimRequest.UserId, claimRequest.Company.Name!, claimRequest.CompanyMemberRole));
            }

            if(command.ClaimStatus == ClaimStatus.Rejected)
            {
                claimRequest.AddDomainEvent
                    (new CompanyClaimRequestRejected(claimRequest.Id, claimRequest.CompanyId, claimRequest.UserId, claimRequest.Company.Name!, claimRequest.CompanyMemberRole));
            }
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
