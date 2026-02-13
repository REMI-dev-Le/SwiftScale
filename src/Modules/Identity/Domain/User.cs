// File: src/Modules/Identity/Domain/User.cs
namespace SwiftScale.Modules.Identity.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private User() { } // Required for EF Core

    public static User Create(string email, string firstName, string lastName)
    {
        // Architect Rule: Validate data BEFORE creating the object
        if (string.IsNullOrWhiteSpace(email)) 
        {
            throw new ArgumentException("Email is required");
        } 

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };
    }
}