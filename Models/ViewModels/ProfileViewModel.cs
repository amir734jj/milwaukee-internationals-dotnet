using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.ViewModels;

public class ProfileViewModel
{
    [Required]
    [MinLength(4)]
    public string Fullname { get; set; }
        
    [Required]
    [EmailAddress]
    public string Email { get; set; }
        
    [Required]
    [Phone]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
    public string PhoneNumber { get; set; }

    [Required]
    [MinLength(6)]
    public string Username { get; set; }

    [Display(Name = "User Role")]
    public UserRoleEnum Role { get; set; }

    public int Id { get; set; }
}