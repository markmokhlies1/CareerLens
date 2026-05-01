using CareerLens.Domain.Companies.Enums;

namespace CareerLens.Application.Features.Companies.Dtos
{
    public class CompanyBasicDto : ICompanyResponse
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int FoundedYear { get; set; }
        public CompanySize Size { get; set; }
    }

}
