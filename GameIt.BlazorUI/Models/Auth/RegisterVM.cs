using System.ComponentModel.DataAnnotations;

namespace GameIt.BlazorUI.Models.Auth;

public class RegisterVM
{
    [Required(ErrorMessage = "First Name is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    public string LastName { get; set; }

    public string ProfilePictureUrl { get; set; } = "https://gameit.blob.core.windows.net/profile-pictures/default.png";

    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
    public string Password { get; set; }
}
