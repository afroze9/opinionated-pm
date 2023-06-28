using AutoMapper;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Entities;
using Nexus.CompanyAPI.Model;

namespace Nexus.CompanyAPI.Mapping;

[ExcludeFromCodeCoverage]
public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, CompanySummaryDto>();
        CreateMap<Company, CompanyDto>();
        CreateMap<CompanySummaryDto, Company>();
        CreateMap<CompanyDto, CompanyResponseModel>();

        CreateMap<CompanyRequestModel, Company>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(x => new TagDto { Name = x })));

        CreateMap<Company, CompanyResponseModel>();

        CreateMap<CompanySummaryDto, CompanyResponseModel>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(x => x.Name)));

        CreateMap<CompanySummaryDto, CompanySummaryResponseModel>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(x => x.Name)));

        CreateMap<CompanySummaryResponseModel, CompanySummaryDto>();

        CreateMap<CompanySummaryDto, CompanyResponseModel>();
        CreateMap<CompanyResponseModel, CompanySummaryDto>();

        CreateMap<Tag, TagDto>();
        CreateMap<TagDto, Tag>();
        CreateMap<TagDto, TagResponseModel>();

        CreateMap<ProjectResponseModel, ProjectSummaryDto>()
            .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.TodoItems.Count));

        CreateMap<ProjectSummaryDto, ProjectSummaryResponseModel>();
    }
}