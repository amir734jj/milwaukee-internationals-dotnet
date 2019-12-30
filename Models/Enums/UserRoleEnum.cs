using System.Collections.Generic;
using System.Linq;

namespace Models.Enums
{
    public enum UserRoleEnum
    {
        Basic = 0,
        Admin = 1
    }

    public static class UserRoleEnumExtension
    {
        public static string JoinToString(this UserRoleEnum userRoleEnums)
        {
            return JoinToString(new[] {userRoleEnums});
        }
        
        public static string JoinToString(this IEnumerable<UserRoleEnum> userRoleEnums)
        {
            return string.Join(',', userRoleEnums.Concat(new[] {UserRoleEnum.Basic}).ToHashSet().Select(x => x.ToString()));
        }
    }
}