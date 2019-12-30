using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.ViewModels
{
    public class ProfileViewModel
    {
        public string Fullname { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Username { get; set; }

        [Display(Name = "User Role")]
        public UserRoleEnum Role { get; set; }

        public int Id { get; set; }
    }
}