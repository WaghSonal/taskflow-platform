public class DepartmentService : IDepartmentService
{
    private readonly Dispatcher _dispatcher;
    public DepartmentService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    public Task<List<DepartmentDto>> GetDepartments(GetDepartmentsQuery query) => _dispatcher.Send(query);
    public Task<int> CreateDepartment(CreateDepartmentCommand command) => _dispatcher.Send(command);
    public Task UpdateDepartment(UpdateDepartmentCommand command) => _dispatcher.Send(command);
}
