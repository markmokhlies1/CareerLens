using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Companies.Enums;
using CareerLens.Domain.DomainUsers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Companies.Commands.Employer.UpdateCompany
{
    [RequireRole("Employer")]
    public sealed record UpdateCompanyCommand(Guid CompanyId,
                                              string Name,
                                              string Industry,
                                              string Location,
                                              string Website,
                                              string Description,
                                              int FoundedYear,
                                              CompanySize Size) : IRequest<Result<Updated>>, IRequireCompanyAccess
    {
        public CompanyMemberRole[] AllowedRoles =>
        [
            CompanyMemberRole.Hr,
            CompanyMemberRole.Moderator
        ];
    }

    public sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyCommandValidator() 
        {

             RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company Id is required.");

            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

            RuleFor(x => x.Industry)
                .NotEmpty()
                .WithMessage("Industry is required.");

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage("Location is required.");

            RuleFor(x => x.Size)
                .IsInEnum()
                .WithMessage("Size is invalid.");

            RuleFor(x => x.Website)
                .NotEmpty()
                .WithMessage("Website is required.")
                .Must(BeValidAbsoluteUrl)
                .WithMessage("Website is invalid.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(CareerLensConstants.CompanyDescriptionMaxLenght)
                .WithMessage($"Description is too long. Maximum length is {CareerLensConstants.CompanyDescriptionMaxLenght} characters.");

            RuleFor(x => x.FoundedYear)
                .InclusiveBetween(1800, DateTime.UtcNow.Year)
                .WithMessage($"FoundedYear must be between 1800 and {DateTime.UtcNow.Year}.");
        }
        private static bool BeValidAbsoluteUrl(string website)
            => Uri.TryCreate(website, UriKind.Absolute, out _); 
    }

    public class UpdateCompanyCommandHandler(IAppDbContext context) : IRequestHandler<UpdateCompanyCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Updated>> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == command.CompanyId, cancellationToken);

            if (company is null)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            var updatedCompanyResult = company.UpdateProfile
                (command.Name, command.Industry, command.Location, command.Size, command.Website, command.Description, command.FoundedYear);

            if (updatedCompanyResult.IsError)
            {
                return updatedCompanyResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }

    }
}
