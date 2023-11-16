using AutoMapper;
using Grpc.Core;
using Nexus.ProjectAPI.Data;
using Nexus.SharedKernel.GRPC;
using ProjectEntity = Nexus.ProjectAPI.Entities.Project;

namespace Nexus.ProjectAPI.Services;

public class ProjectGrpcService : SharedKernel.GRPC.ProjectGrpcService.ProjectGrpcServiceBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectGrpcService(UnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public override async Task<GetProjectsByCompanyIdResponse> GetProjectsByCompanyId(
        GetProjectsByCompanyIdRequest request, 
        ServerCallContext context)
    {
        List<ProjectEntity> projects = await _unitOfWork.Projects.GetAllByCompanyIdAsync(request.CompanyId, true);
        List<Project> mappedProjects =
            projects.Count == 0 ? new List<Project>() : _mapper.Map<List<Project>>(projects);

        GetProjectsByCompanyIdResponse response = new ()
        {
            Projects = { mappedProjects },
        };

        return response;
    }
}