
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Identity.Application.Interfaces;

namespace SwiftScale.Modules.Identity.Application.Users.Login
{
    internal sealed class LoginCommandHandler(
     IIdentityDbContext context,
     IPasswordHasher passwordHasher,
     IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken ct)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);

            if (user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result<string>.Failure(new Error("Invalid email or password."));
            }

            var token = jwtTokenService.GenerateToken(user);
            return Result<string>.Success(token);
        }
    }
}
