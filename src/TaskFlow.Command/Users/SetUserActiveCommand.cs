public class SetUserActiveCommand : IRequest
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }
}

public class SetUserActiveCommandHandler : IRequestHandler<SetUserActiveCommand>
{
    private readonly IUserRepository _userRepository;
    public SetUserActiveCommandHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task Handle(SetUserActiveCommand request, CancellationToken ct)
    {
        var user = await _userRepository.ReadFirstOrDefaultAsync(u => u.UserId == request.UserId)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        user.IsActive = request.IsActive;
        user.UpdatedDate = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }
}
