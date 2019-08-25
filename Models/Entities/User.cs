using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities
{
    public class User : IdentityUser<int>, IPerson
    {        
        public string Fullname { get; set; }
        
        [Display(Name = "User Role")]
        public UserRoleEnum UserRoleEnum { get; set; }
    }
}