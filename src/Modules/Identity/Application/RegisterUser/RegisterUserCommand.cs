using MediatR;
using SwiftScale.BuildingBlocks;
namespace SwiftScale.Modules.Identity.Application.RegisterUser
{
    public record RegisterUserCommand(string Email,
                                      string Username,
                                      string Password,
                                      string FirstName,
                                      string LastName) : IRequest<Result<Guid>>;
}
