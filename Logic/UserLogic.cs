using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using DAL.Interfaces;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Enums;

namespace Logic;

public class UserLogic : BasicCrudLogicAbstract<User>, IUserLogic
{
    private readonly IBasicCrud<User> _dal;
    private readonly UserManager<User> _userManager;
    private readonly IApiEventService _apiEventService;

    /// <summary>
    /// Constructor dependency injection
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="userManager"></param>
    /// <param name="apiEventService"></param>
    public UserLogic(IEfRepository repository, UserManager<User> userManager, IApiEventService apiEventService)
    {
        _dal = repository.For<User>();
        _userManager = userManager;
        _apiEventService = apiEventService;
    }

    public override async Task<User> Get(int id)
    {
        var fetchUserObservable = base.Get(id)
            .ToObservable()
            .Then(user =>
            {
                var roles = _userManager.GetRolesAsync(user).Result;

                user.UserRoleEnum = roles.Contains(UserRoleEnum.Admin.ToString())
                    ? UserRoleEnum.Admin
                    : UserRoleEnum.Basic;

                return user;
            });

        var result = await Observable.When(fetchUserObservable);

        return result;
    }

    public async Task Disable(int id)
    {
        await _dal.Update(id, x => x.Enable = false);
    }

    public async Task Enable(int id)
    {
        await _dal.Update(id, x => x.Enable = true);
    }

    protected override IBasicCrud<User> Repository()
    {
        return _dal;
    }
        
    protected override IApiEventService ApiEventService()
    {
        return _apiEventService;
    }

    public override async Task<IEnumerable<User>> GetAll(string sortBy = null, bool? descending = null, params Expression<Func<User, bool>>[] filters)
    {
        var fetchUsersObservable = base.GetAll(sortBy, descending, filters)
            .ToObservable()
            .Then(users => users.Select(user =>
            {
                var roles = _userManager.GetRolesAsync(user).Result;

                user.UserRoleEnum = roles.Contains(UserRoleEnum.Admin.ToString())
                    ? UserRoleEnum.Admin
                    : UserRoleEnum.Basic;

                return user;
            }));

        var result = await Observable.When(fetchUsersObservable);

        return result;
    }
}