namespace Models.Interfaces;

public interface IYearlyEntity : IEntity
{
    public int Year { get; set; }
}