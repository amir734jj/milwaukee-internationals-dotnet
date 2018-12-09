﻿using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Models.Entities;

namespace Models.Profiles
{    
    public class HostProfile : Profile
    {
        public HostProfile()
        {
            CreateMap<Host, Host>()
                .EqualityComparison((x, y) => x.Id == y.Id);
        }
    }
}