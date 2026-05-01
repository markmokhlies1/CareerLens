using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Companies.Dtos;
using CareerLens.Application.Features.Companies.Mappers;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerLens.Application.Features.Companies.Commands.Admin.CreateCompany
{
    [RequireRole("Admin")]
    public sealed record CreateCompanyCommand(string Name, string Industry, string Location, string Website,
                                              string Description, int FoundedYear, CompanySize Size) 
        : IRequest<Result<CompanyAdminDto>>;

    public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator() 
        {
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

    public class CreateCompanyCommandHandler(IAppDbContext context) : IRequestHandler<CreateCompanyCommand, Result<CompanyAdminDto>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<CompanyAdminDto>> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
        {
            var exists = await _context.Companies
                        .AnyAsync(c => c.Name == command.Name, cancellationToken);

            if (exists)
            {
                return ApplicationErrors.NameAlreadyExists;
            }

            var createCompanyResult = Company.Create
                (Guid.NewGuid(),command.Name,command.Industry,command.Location,command.Size,command.Website,command.Description,command.FoundedYear);

            if(createCompanyResult.IsError)
            {
                return createCompanyResult.Errors;
            }

            var company = createCompanyResult.Value;
            _context.Companies.Add(company);
            await _context.SaveChangesAsync(cancellationToken);

            return company.ToAdminDto();
        }
    }
}
