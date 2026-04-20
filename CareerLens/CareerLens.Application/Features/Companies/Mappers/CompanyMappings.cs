using CareerLens.Application.Features.Companies.Dtos;
using CareerLens.Application.Features.CompanyClaimRequests.Dtos;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Companies.Mappers
{
    public static class CompanyMappings
    {
        public static CompanyAdminDto ToAdminDto(this Company entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new CompanyAdminDto
            {
                CompanyId = entity.Id,
                Name = entity.Name!,
                Industry = entity.Industry!,
                Location = entity.Location!,
                Website = entity.Website!,
                Description = entity.Description!,
                FoundedYear = entity.FoundedYear,
                Size = entity.Size!,
                IsClaimed = entity.IsClaimed,
                CreatedAt = entity.CreatedAtUtc
            };
        }

        public static CompanyBasicDto ToBasicDto(this Company entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new CompanyBasicDto
            {
                CompanyId = entity.Id,
                Name = entity.Name!,
                Industry = entity.Industry!,
                Location = entity.Location!,
                Website = entity.Website!,
                Description = entity.Description!,
                FoundedYear = entity.FoundedYear,
                Size = entity.Size!,
            };
        }
        public static List<ICompanyResponse> ToAdminDtos(this IEnumerable<Company> entities)
        {
            return [.. entities.Select(e => e.ToAdminDto())];
        }
        public static List<ICompanyResponse> ToBasicDtos(this IEnumerable<Company> entities)
        {
            return [.. entities.Select(e => e.ToBasicDto())];
        }
    }
}
