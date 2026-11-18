using Microsoft.EntityFrameworkCore;
using TemplateAPI.Data;
using TemplateAPI.DTOs;
using TemplateAPI.Models;

namespace TemplateAPI.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct = default);
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
}

public class UserService(AppDbContext db) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct = default) =>
        await db.Users
            .AsNoTracking()
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Select(u => new UserResponse(u.Id, u.FirstName, u.LastName, u.Email,
                u.PhoneNumber, u.CreatedAt, u.UpdatedAt))
            .ToListAsync(ct);

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        return user is null ? null : UserResponse.FromEntity(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return UserResponse.FromEntity(user);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return null;

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = request.Email.Trim().ToLowerInvariant();
        user.PhoneNumber = request.PhoneNumber?.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return UserResponse.FromEntity(user);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var deleted = await db.Users.Where(u => u.Id == id).ExecuteDeleteAsync(ct);
        return deleted > 0;
    }

    public Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return db.Users.AnyAsync(u => u.Email == normalized && (excludeId == null || u.Id != excludeId), ct);
    }
}
