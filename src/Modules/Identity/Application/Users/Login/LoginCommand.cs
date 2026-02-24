using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Identity.Application.Users.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<string>>;
}
