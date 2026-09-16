public interface IProjectService
{
    Task<List<ProjectDto>> GetProjects(GetProjectsQuery query);
    Task<ProjectDto> GetProjectById(int projectId);
    Task<int> CreateProject(CreateProjectCommand command);
    Task UpdateProject(UpdateProjectCommand command);
    Task DeleteProject(int projectId);
    Task<List<ProjectMemberDto>> GetProjectMembers(int projectId);
    Task<int> AddProjectMember(AddProjectMemberCommand command);
    Task RemoveProjectMember(int projectMemberId);
}
