using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities
{
    public class User : IdentityUser<int>, IPerson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [PersonalData]
        public override int Id { get; set; }

        public string Fullname { get; set; }
        
        [Display(Name = "User Role")]
        public UserRoleEnum UserRoleEnum { get; set; }
    }
}