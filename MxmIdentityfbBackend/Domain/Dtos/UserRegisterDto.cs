using System.ComponentModel.DataAnnotations;

namespace MxmIdentityfbBackend.Domain.Dtos;

public class UserRegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string PasswordConfirmation { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters.")]
    public string FirstName { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
    public string LastName { get; set; }

}
