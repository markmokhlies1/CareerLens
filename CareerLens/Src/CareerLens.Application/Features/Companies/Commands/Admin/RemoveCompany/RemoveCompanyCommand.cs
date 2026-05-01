using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Companies.Commands.Admin.RemoveCompany
{
    [RequireRole("Admin")]
    public sealed record RemoveCompanyCommand(Guid CompanyId) : IRequest<Result<Deleted>>;

    public class RemoveCompanyCommandValidator : AbstractValidator<RemoveCompanyCommand>
    {
        public RemoveCompanyCommandValidator()  
        {
             RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company Id is required.");
        }
    }
    public class RemoveCompanyCommandHandler(IAppDbContext context) : IRequestHandler<RemoveCompanyCommand, Result<Deleted>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<Deleted>> Handle(RemoveCompanyCommand command, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FindAsync(command.CompanyId,cancellationToken);

            if (company is null)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Deleted;
        }
    }
}
