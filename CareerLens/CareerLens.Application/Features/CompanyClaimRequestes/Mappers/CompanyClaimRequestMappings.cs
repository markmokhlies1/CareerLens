using CareerLens.Application.Features.CompanyClaimRequests.Dtos;
using CareerLens.Domain.Companies.CompanyClaimRequests;

namespace CareerLens.Application.Features.CompanyClaimRequests.Mappers
{
    public static class CompanyClaimRequestMappings
    {
        public static CompanyClaimRequestAdminDto ToAdminDto(this CompanyClaimRequest entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new CompanyClaimRequestAdminDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                UserId = entity.UserId,
                Status = entity.Status,
                AdminNote = entity.AdminNote!,
                CompanyMemberRole = entity.CompanyMemberRole,
                CompanyName = entity.Company.Name,
                UserName = entity.User!.FirstName!+" "+entity.User.LastName!,
                UserEmail = entity.User.Email,
                CreatedAt = entity.CreatedAtUtc,
                UpdatedAt = entity.LastModifiedUtc
            };
        }
        public static List<ICompanyClaimRequestResponse> ToAdminDtos(this IEnumerable<CompanyClaimRequest> entities)
        {
            return [.. entities.Select(e => e.ToAdminDto())];
        }
        public static CompanyClaimRequestEmployerDto ToEmployerDto(this CompanyClaimRequest entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new CompanyClaimRequestEmployerDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Status = entity.Status,
                AdminNote = entity.AdminNote!,
                CompanyMemberRole = entity.CompanyMemberRole,
                UserName = entity.User!.FirstName! + " " + entity.User.LastName!,
                UserEmail = entity.User.Email,
                CreatedAt = entity.CreatedAtUtc,
            };
        }
        public static List<ICompanyClaimRequestResponse> ToEmployerDtos(this IEnumerable<CompanyClaimRequest> entities)
        {
            return [.. entities.Select(e => e.ToEmployerDto())];
        }
    }
}
