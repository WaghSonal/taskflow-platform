public class ProjectService : IProjectService
{
    private readonly Dispatcher _dispatcher;
    public ProjectService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    public Task<List<ProjectDto>> GetProjects(GetProjectsQuery query) => _dispatcher.Send(query);
    public Task<ProjectDto> GetProjectById(int projectId) => _dispatcher.Send(new GetProjectByIdQuery { ProjectId = projectId });
    public Task<int> CreateProject(CreateProjectCommand command) => _dispatcher.Send(command);
    public Task UpdateProject(UpdateProjectCommand command) => _dispatcher.Send(command);
    public Task DeleteProject(int projectId) => _dispatcher.Send(new DeleteProjectCommand { ProjectId = projectId });
    public Task<List<ProjectMemberDto>> GetProjectMembers(int projectId) => _dispatcher.Send(new GetProjectMembersQuery { ProjectId = projectId });
    public Task<int> AddProjectMember(AddProjectMemberCommand command) => _dispatcher.Send(command);
    public Task RemoveProjectMember(int projectMemberId) => _dispatcher.Send(new RemoveProjectMemberCommand { ProjectMemberId = projectMemberId });
}
