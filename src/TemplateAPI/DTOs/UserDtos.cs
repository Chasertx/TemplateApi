using System.ComponentModel.DataAnnotations;
using TemplateAPI.Models;

namespace TemplateAPI.DTOs;

public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static UserResponse FromEntity(User user) => new(
        user.Id, user.FirstName, user.LastName, user.Email,
        user.PhoneNumber, user.CreatedAt, user.UpdatedAt);
}

public record CreateUserRequest(
    [Required, StringLength(100)] string FirstName,
    [Required, StringLength(100)] string LastName,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Phone, StringLength(30)] string? PhoneNumber);

public record UpdateUserRequest(
    [Required, StringLength(100)] string FirstName,
    [Required, StringLength(100)] string LastName,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Phone, StringLength(30)] string? PhoneNumber);
