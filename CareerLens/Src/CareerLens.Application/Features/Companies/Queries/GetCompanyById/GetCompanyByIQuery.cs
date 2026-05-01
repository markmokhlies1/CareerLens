using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Companies.Dtos;
using CareerLens.Application.Features.Companies.Mappers;
using CareerLens.Application.Features.Companies.Queries.Admin.GetCompanyById;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Companies.Queries.GetCompanyById
{
    public sealed record GetCompanyByIdQuery(Guid CompanyId) : IRequest<Result<CompanyBasicDto>>;
    public sealed class GetCompanyByIdQueryValidator : AbstractValidator<GetCompanyByIdQuery>
    {
        public GetCompanyByIdQueryValidator()
        {
            RuleFor(x => x.CompanyId)
           .NotEmpty()
           .WithMessage("Company Id is required.");
        }
    }
    public sealed class GetCompanyByIdForAdminQueryHandler(IAppDbContext context) 
        : IRequestHandler<GetCompanyByIdQuery, Result<CompanyBasicDto>>
    {
        private readonly IAppDbContext _context = context;
        public async Task<Result<CompanyBasicDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.CompanyId, cancellationToken);

            if (company is null)
            {
                return ApplicationErrors.CompanyNotFound;
            }

            return company.ToBasicDto();
        }
    }
}
