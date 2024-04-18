using System.ComponentModel.DataAnnotations;

namespace MxmIdentityfbBackend.Domain.Dtos;

public class UserLoginDto
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
