// File: src/Modules/Identity/Domain/User.cs
using SwiftScale.BuildingBlocks;
using System.Data;

namespace SwiftScale.Modules.Identity.Domain;

public class User : Entity
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty; // Added
    public string PasswordHash { get; private set; } = string.Empty; // Added
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public string Role { get; private set; } = "Customer";
    public DateTime CreatedAt { get; private set; }


    private User() { } // Required for EF Core

    public static User Create(string email, string username, string passwordHash, string firstName, string lastName, string role)
    {

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required");
        } 
        // Architect Rule: Validate data BEFORE creating the object
        if (string.IsNullOrWhiteSpace(email)) 
        {
            throw new ArgumentException("Email is required");
        }

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
    }
}