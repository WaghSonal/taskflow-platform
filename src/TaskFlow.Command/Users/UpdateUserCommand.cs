public class UpdateUserCommand : IRequest
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int? DepartmentId { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    public UpdateUserCommandHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await _userRepository.ReadFirstOrDefaultAsync(u => u.UserId == request.UserId)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.DepartmentId = request.DepartmentId;
        user.UpdatedDate = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }
}
