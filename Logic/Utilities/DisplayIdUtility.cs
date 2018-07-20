using System;
using Models.Interfaces;

namespace Logic.Utilities
{
    public static class DisplayIdUtility
    {
        /// <summary>
        /// Generates a display Id
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static string GenerateDisplayId(IPerson person)
        {
            return person.Fullname.Substring(0, 3).Replace(" ", string.Empty).ToUpper() + Guid.NewGuid().ToString().Substring(0, 3);
        }
    }
}