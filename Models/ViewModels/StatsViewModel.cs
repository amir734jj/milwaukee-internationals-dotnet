namespace Models.ViewModels;

public class StatsViewModel
{
    public int CountStudents { get; set; }
    
    public int CountDependents { get; set; }
    
    public int CountDrivers { get; set; }
    
    public int CountHosts { get; set; }
    
    public int CountNavigators { get; set; }
    
    public int Year { get; set; }
    
    public int CountDistinctCountries { get; set; }
    
    public bool CurrentYear { get; set; }
    
    public bool ActiveYear { get; set; }
}