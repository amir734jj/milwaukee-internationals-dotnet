using System.Linq;
using Logic.Interfaces;

namespace Logic.Extensions
{
    public static class BasicCrudLogicExtensions
    {
        /// <summary>
        /// Returns the first or default match given hashcode
        /// </summary>
        /// <param name="basicCrudLogic"></param>
        /// <param name="hashcode"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetByHashcode<T>(this IBasicCrudLogic<T> basicCrudLogic, int hashcode) =>
            basicCrudLogic.GetAll().Result.FirstOrDefault(x => x.GetHashCode() == hashcode);
    }
}