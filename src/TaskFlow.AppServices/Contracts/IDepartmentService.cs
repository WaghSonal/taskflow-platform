public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetDepartments(GetDepartmentsQuery query);
    Task<int> CreateDepartment(CreateDepartmentCommand command);
    Task UpdateDepartment(UpdateDepartmentCommand command);
}
