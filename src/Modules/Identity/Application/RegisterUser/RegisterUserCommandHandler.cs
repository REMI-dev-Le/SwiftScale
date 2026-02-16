using MediatR;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Identity.Application.Interfaces;
using SwiftScale.Modules.Identity.Domain;

namespace SwiftScale.Modules.Identity.Application.RegisterUser
{
    public sealed class RegisterUserCommandHandler(IIdentityDbContext context, IPasswordHasher passwordHasher) : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {

        public async Task<Result<Guid>> Handle(RegisterUserCommand request,CancellationToken cancellationToken)
        {
            // 1. Hash the password BEFORE creating the domain entity
            string passwordHash = passwordHasher.HashPassword(request.Password);

            // 2. Create the User with the Hash (never the raw password)
            var user = User.Create(request.Email,
                                   request.Username,
                                   passwordHash,
                                   request.FirstName,
                                   request.LastName);

            // 3. Persist
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}
