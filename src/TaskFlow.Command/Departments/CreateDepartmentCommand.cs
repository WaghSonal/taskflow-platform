public class CreateDepartmentCommand : IRequest<int>
{
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
{
    private readonly IRepository<Department> _departmentRepository;
    public CreateDepartmentCommandHandler(IRepository<Department> departmentRepository) => _departmentRepository = departmentRepository;

    public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken ct)
    {
        var department = new Department { DepartmentName = request.DepartmentName, Description = request.Description, IsActive = true };
        await _departmentRepository.CreateAsync(department);
        return department.DepartmentId;
    }
}
