using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Interfaces;
using ObjectHashing;
using ObjectHashing.Interfaces;

namespace Models.Entities;

public class Location : ObjectHash<LocationMapping>, IYearlyEntity, IEqualityComparer<Location>
{
    [Key]
    public int Id { get; set; }
    
    public int Rank { get; set; }
    
    public int Year { get; set; }
    
    public string Name { get; set; }
    
    public string Address { get; set; }
    
    public string Description { get; set; }
    
    public List<LocationMapping> LocationMappingsSources { get; set; }
    
    public List<LocationMapping> LocationMappingsSinks { get; set; }

    protected override void ConfigureObjectSha(IConfigureObjectHashConfig<LocationMapping> config)
    {
        config
            .DefaultAlgorithm()
            .Property(x => x.Id)
            .Property(x => x.SourceId)
            .Property(x => x.SinkId)
            .Property(x => x.Year)
            .DefaultSerialization()
            .Build();
    }

    public bool Equals(Location x, Location y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id && x.Year == y.Year && x.Name == y.Name;
    }

    public int GetHashCode(Location obj)
    {
        return HashCode.Combine(obj.Id, obj.Year, obj.Name);
    }
}