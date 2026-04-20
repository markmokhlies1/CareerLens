using Bogus.Bson;
using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Companies.Mappers;
using CareerLens.Application.Features.CompanyClaimRequests.Dtos;
using CareerLens.Application.Features.CompanyClaimRequests.Mappers;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyClaimRequests.Enums;
using CareerLens.Domain.Companies.CompanyClaimRequests.Events;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.DomainUsers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequests.Commands.Employer.CreateCompanyClaimRequest
{
    [RequireRole("Employer")]
    public sealed record CreateCompanyClaimRequestCommand(Guid CompanyId,
                                                          string AdminNote,
                                                          CompanyMemberRole CompanyMemberRole) : IRequest<Result<CompanyClaimRequestEmployerDto>>;

    public sealed class CreateCompanyClaimRequestCommandValidator : AbstractValidator<CreateCompanyClaimRequestCommand>
    {
        public CreateCompanyClaimRequestCommandValidator()
        { 
             RuleFor(x => x.CompanyId) 
            .NotEmpty()
            .WithMessage("CompanyId is required.");

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
    public class CreateCompanyClaimRequestCommandHandler(IAppDbContext context,IUser currentUser) : IRequestHandler<CreateCompanyClaimRequestCommand, Result<CompanyClaimRequestEmployerDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;
        public async Task<Result<CompanyClaimRequestEmployerDto>> Handle(CreateCompanyClaimRequestCommand command, CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == command.CompanyId, cancellationToken);

            if(company is null)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var hasPendingRequest = await _context.CompanyClaimsRequests
            .AnyAsync(r => r.CompanyId == command.CompanyId
                        && r.UserId == userId
                        && r.Status == ClaimStatus.Pending
                        && r.CompanyMemberRole == command.CompanyMemberRole,
                      cancellationToken);

            if (hasPendingRequest)
            {
                return ApplicationErrors.AlreadyHasPendingRequest;
            }

            var isAlreadyMember = await _context.CompanyMembers
            .AnyAsync(m => m.CompanyId == command.CompanyId && m.UserId == userId && m.Role== command.CompanyMemberRole,
                      cancellationToken);

            if (isAlreadyMember)
            {
                return ApplicationErrors.AlreadyMember;
            }

            var claimRequestResult = CompanyClaimRequest.Create
                (Guid.NewGuid(), command.CompanyId, userId, command.AdminNote, command.CompanyMemberRole);

            if (claimRequestResult.IsError)
            {
                return claimRequestResult.Errors;
            }

            var claimRequest = claimRequestResult.Value;
            company.AddCompanyClaimRequest(claimRequest);

            claimRequest.AddDomainEvent(new CompanyClaimRequestCreated(claimRequest.Id, claimRequest.CompanyId, claimRequest.UserId, company.Name!, claimRequest.CompanyMemberRole));

            await _context.SaveChangesAsync(cancellationToken);

            return claimRequest.ToEmployerDto();
        }
    }
}
