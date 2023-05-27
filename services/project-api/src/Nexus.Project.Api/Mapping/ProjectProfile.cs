using AutoMapper;
using Nexus.ProjectAPI.Entities;
using Nexus.ProjectAPI.Models;

namespace Nexus.ProjectAPI.Mapping;

[ExcludeFromCodeCoverage]
public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<Project, ProjectRequestModel>();
        CreateMap<ProjectRequestModel, Project>();
        CreateMap<TodoItemRequestModel, TodoItem>();
    }
}