using System.Threading.Tasks;
using Models.Entities;

namespace Logic.Interfaces
{
    public interface IDriverLogic : IBasicCrudLogic<Driver>
    {
        public Task<Driver> FindByDriverId(string driverId);
    }
}