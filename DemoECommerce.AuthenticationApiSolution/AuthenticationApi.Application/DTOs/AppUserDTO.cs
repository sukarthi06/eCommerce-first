using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs;

public record AppUserDTO(
    int Id,
    [Required] string Name,
    [Required] string TelephoneNumber,
    [Required] string Address,
    [Required] string Email,
    [Required] string Password,
    [Required] string Role
);
