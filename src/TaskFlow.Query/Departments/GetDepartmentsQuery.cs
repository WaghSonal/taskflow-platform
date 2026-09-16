using Microsoft.EntityFrameworkCore;

public class GetDepartmentsQuery : IRequest<List<DepartmentDto>>
{
    public bool? IsActive { get; set; }
}

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
{
    private readonly IRepository<Department> _departmentRepository;
    private readonly IUserRepository _userRepository;
    public GetDepartmentsQueryHandler(IRepository<Department> departmentRepository, IUserRepository userRepository)
    { _departmentRepository = departmentRepository; _userRepository = userRepository; }

    public async Task<List<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken ct)
    {
        var departments = await _departmentRepository
            .ReadWhere(d => request.IsActive == null || d.IsActive == request.IsActive)
            .ToListAsync(ct);

        var counts = await _userRepository.ReadWhere(u => u.DepartmentId != null)
            .GroupBy(u => u.DepartmentId!.Value)
            .Select(g => new { DepartmentId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return departments
            .Select(d => new DepartmentDto(d.DepartmentId, d.DepartmentName, d.Description, d.IsActive,
                counts.FirstOrDefault(c => c.DepartmentId == d.DepartmentId)?.Count ?? 0))
            .ToList();
    }
}
