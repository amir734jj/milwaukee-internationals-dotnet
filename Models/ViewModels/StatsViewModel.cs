using System.Collections.Generic;

namespace Models.ViewModels;

public class StatsViewModel
{
    public int CountStudents { get; set; }
    
    public int CountDependents { get; set; }
    
    public int CountDrivers { get; set; }
    
    public int CountHosts { get; set; }
    
    public int Year { get; set; }
    
    public int CountDistinctCountries { get; set; }
    
    public bool CurrentYear { get; set; }
    
    public bool ActiveYear { get; set; }
    
    public Dictionary<string, int> CountryDistribution { get; set; }
    
    public int CountPresentStudents { get; set; }
}