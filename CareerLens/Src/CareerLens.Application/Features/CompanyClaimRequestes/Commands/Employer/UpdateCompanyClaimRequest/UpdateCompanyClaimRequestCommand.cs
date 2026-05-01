using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequests.Commands.Employer.UpdateCompanyClaimRequest
{
    [RequireRole("Employer")]
    public sealed record UpdateCompanyClaimRequestCommand(Guid CompanyRequestId,
                                                          string AdminNote,
                                                          CompanyMemberRole CompanyMemberRole) : IRequest<Result<Updated>>;

    public class UpdateCompanyClaimRequestCommandValidator : AbstractValidator<UpdateCompanyClaimRequestCommand>
    {
        public UpdateCompanyClaimRequestCommandValidator()
        {
            RuleFor(x => x.CompanyRequestId)
            .NotEmpty()
            .WithMessage("CompanyRequestId is required.");

            RuleFor(x => x.AdminNote)
                .NotEmpty()
                .WithMessage("AdminNote is required.")
                .MaximumLength(2000)
                .WithMessage("AdminNote is too long. Maximum length is 2000 characters.");

            RuleFor(x => x.CompanyMemberRole)
                .IsInEnum()
                .WithMessage("CompanyMemberRole is invalid.");
        }
    }

    public sealed class UpdateCompanyClaimRequestCommandHandler (IAppDbContext context,IUser currentUser)
        : IRequestHandler<UpdateCompanyClaimRequestCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;
        public async Task<Result<Updated>> Handle(UpdateCompanyClaimRequestCommand command, CancellationToken cancellationToken)
        {
            
            var claimRequest = await _context.CompanyClaimsRequests
            .FirstOrDefaultAsync(c => c.Id == command.CompanyRequestId, cancellationToken);

            if (claimRequest is null)
            {
                return ApplicationErrors.ClaimRequestNotFound;
            }
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            if (claimRequest.UserId != userId)
            {
                return ApplicationErrors.NotClaimRequestOwner;
            }

            var updatedClaimRequestResult = claimRequest.Update(command.AdminNote, command.CompanyMemberRole);

            if(updatedClaimRequestResult.IsError)
            {
                return updatedClaimRequestResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Updated;

        }
    }
}
