using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Identities
{
    /// <summary>
    ///     Register view model
    /// </summary>
    public class RegisterViewModel
    {
        [Required]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(4)]
        public string Fullname { get; set; }

        [Required]
        [MinLength(6)]
        public string Username { get; set; }
        
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "Password should contain lower and upper case alphanumeric characters + special character")]
        public string Password { get; set; }
        
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "Password should contain lower and upper case alphanumeric characters + special character")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}