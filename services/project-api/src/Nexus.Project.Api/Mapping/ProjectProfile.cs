using AutoMapper;
using Nexus.ProjectAPI.Entities;
using Nexus.ProjectAPI.Models;
using Nexus.SharedKernel.Contracts.Project;
using ProjectEntity = Nexus.ProjectAPI.Entities.Project;

namespace Nexus.ProjectAPI.Mapping;

[ExcludeFromCodeCoverage]
public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<ProjectEntity, ProjectRequestModel>();
        CreateMap<ProjectRequestModel, ProjectEntity>();
        CreateMap<TodoItemRequestModel, TodoItem>();

        CreateMap<ProjectEntity, ProjectResponseModel>()
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority));
        CreateMap<ProjectResponseModel, ProjectEntity>();
        CreateMap<TodoItem, TodoItemResponseModel>();
    }
}