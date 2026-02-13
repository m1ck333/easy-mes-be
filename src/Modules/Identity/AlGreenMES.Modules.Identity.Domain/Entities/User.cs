using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;

namespace AlGreenMES.Modules.Identity.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public Guid? ProcessId { get; private set; }
    public bool CanIncludeWithdrawnInAnalysis { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private User()
    {
    }

    public static User Create(Guid tenantId, string email, string passwordHash, string firstName, string lastName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("USER_EMAIL_REQUIRED", "User email is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("USER_PASSWORD_REQUIRED", "User password is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("USER_FIRST_NAME_REQUIRED", "User first name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("USER_LAST_NAME_REQUIRED", "User last name is required.");

        var user = new User
        {
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Role = role,
            IsActive = true
        };

        return user;
    }

    public void Update(string firstName, string lastName, UserRole role, bool isActive, bool canIncludeWithdrawnInAnalysis = false)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("USER_FIRST_NAME_REQUIRED", "User first name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("USER_LAST_NAME_REQUIRED", "User last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Role = role;
        IsActive = isActive;
        CanIncludeWithdrawnInAnalysis = canIncludeWithdrawnInAnalysis;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("USER_PASSWORD_REQUIRED", "User password is required.");

        PasswordHash = newPasswordHash;
    }

    public void AssignToProcess(Guid? processId)
    {
        ProcessId = processId;
    }
}
