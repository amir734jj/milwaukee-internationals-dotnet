using EfCoreRepository.Interfaces;

namespace DAL.Interfaces
{
    public interface IBasicCrudDal<T> : IBasicCrudType<T, int> where T : class, IEntity<int>
    {
        
    }
}