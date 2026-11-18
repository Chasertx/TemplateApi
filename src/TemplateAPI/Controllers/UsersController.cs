using Microsoft.AspNetCore.Mvc;
using TemplateAPI.DTOs;
using TemplateAPI.Services;

namespace TemplateAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>Get all users.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken ct) =>
        Ok(await userService.GetAllAsync(ct));

    /// <summary>Get a user by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var user = await userService.GetByIdAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>Create a user.</summary>
    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request, CancellationToken ct)
    {
        if (await userService.EmailExistsAsync(request.Email, ct: ct))
            return Conflict(new ProblemDetails { Title = "A user with this email already exists." });

        var user = await userService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>Update a user.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Update(Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        if (await userService.EmailExistsAsync(request.Email, excludeId: id, ct: ct))
            return Conflict(new ProblemDetails { Title = "A user with this email already exists." });

        var user = await userService.UpdateAsync(id, request, ct);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>Delete a user.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await userService.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
