namespace Models.Interfaces
{
    public interface IPerson : IBasicModel
    {   
        string Fullname { get; set; }
    }
}