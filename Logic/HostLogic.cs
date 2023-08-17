using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Interfaces;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Constants;
using Models.Entities;
using static Logic.Utilities.RegistrationUtility;

namespace Logic;

public class HostLogic : BasicCrudLogicAbstract<Host>, IHostLogic
{
    private readonly IBasicCrud<Host> _dal;
    private readonly GlobalConfigs _globalConfigs;
    private readonly IApiEventService _apiEventService;

    /// <summary>
    /// Constructor dependency injection
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="globalConfigs"></param>
    /// <param name="apiEventService"></param>
    public HostLogic(IEfRepository repository, GlobalConfigs globalConfigs, IApiEventService apiEventService)
    {
        _dal = repository.For<Host>();
        _globalConfigs = globalConfigs;
        _apiEventService = apiEventService;
    }

    public override Task<Host> Save(Host instance)
    {
        // Set the year
        instance.Year = DateTime.UtcNow.Year;
            
        // Normalize phone number
        instance.Phone = NormalizePhoneNumber(instance.Phone);
            
        return base.Save(instance);
    }

    protected override IBasicCrud<Host> Repository()
    {
        return _dal;
    }
        
    protected override IApiEventService ApiEventService()
    {
        return _apiEventService;
    }

    public override async Task<IEnumerable<Host>> GetAll(string sortBy = null, bool? descending = null, Func<object, string, object> sortByModifier = null, params Expression<Func<Host, bool>>[] filters)
    {
        Expression<Func<Host, bool>> yearFilterExpr = x => x.Year == _globalConfigs.YearValue;
        
        return await base.GetAll(sortBy, descending, null, filters.Concat(new[] { yearFilterExpr }).ToArray());
    }

    public override Task<Host> Update(int id, Host host)
    {
        // Update only subset of properties
        return base.Update(id, x =>
        {
            x.Fullname = host.Fullname;
            x.Email = host.Email;
            x.Phone = host.Phone;
            x.Address = host.Address;
        });
    }
}