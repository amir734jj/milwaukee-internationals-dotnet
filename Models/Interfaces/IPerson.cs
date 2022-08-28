namespace Models.Interfaces
{
    public interface IPerson : IEntity
    {
        string Fullname { get; set; }
    }
}