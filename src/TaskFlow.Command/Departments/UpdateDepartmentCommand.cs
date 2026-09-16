public class UpdateDepartmentCommand : IRequest
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand>
{
    private readonly IRepository<Department> _departmentRepository;
    public UpdateDepartmentCommandHandler(IRepository<Department> departmentRepository) => _departmentRepository = departmentRepository;

    public async Task Handle(UpdateDepartmentCommand request, CancellationToken ct)
    {
        var department = await _departmentRepository.ReadFirstOrDefaultAsync(d => d.DepartmentId == request.DepartmentId)
            ?? throw new NotFoundException($"Department {request.DepartmentId} not found.");

        department.DepartmentName = request.DepartmentName;
        department.Description = request.Description;
        department.IsActive = request.IsActive;

        await _departmentRepository.UpdateAsync(department);
    }
}
