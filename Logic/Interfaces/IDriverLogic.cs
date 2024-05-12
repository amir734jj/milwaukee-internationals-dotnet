using System.Threading.Tasks;
using Models.Entities;
using Models.ViewModels;

namespace Logic.Interfaces;

public interface IDriverLogic : IBasicCrudLogic<Driver>
{
    public Task<Driver> DriverLogin(DriverLoginViewModel driverLoginViewModel);
}