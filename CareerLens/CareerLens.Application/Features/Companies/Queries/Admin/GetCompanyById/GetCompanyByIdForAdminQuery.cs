using CareerLens.Application.Common.Behaviors;
using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Companies.Dtos;
using CareerLens.Application.Features.Companies.Mappers;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using FluentValidation.Validators;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Companies.Queries.Admin.GetCompanyById
{
    [RequireRole("Admin")]
    public sealed record GetCompanyByIdForAdminQuery(Guid CompanyId) : IRequest<Result<CompanyAdminDto>>;
    public sealed class GetCompanyByIdForAdminQueryValidator : AbstractValidator<GetCompanyByIdForAdminQuery>
    {
        public GetCompanyByIdForAdminQueryValidator()
        {
             RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company Id is required.");
        }
    }
    public sealed class GetCompanyByIdForAdminQueryHandler(IAppDbContext context) : IRequestHandler<GetCompanyByIdForAdminQuery, Result<CompanyAdminDto>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<CompanyAdminDto>> Handle(GetCompanyByIdForAdminQuery request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.CompanyId, cancellationToken);

            if (company is null)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            return company.ToAdminDto();
        }
    }
}
