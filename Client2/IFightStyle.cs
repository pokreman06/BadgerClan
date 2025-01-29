using BadgerClan.Logic;
namespace BadgerClan.Client;

public interface IFightStyle
{
    public string Name { get; }
    public List<Move> GetCurrentMoves(); 
}
public class Gay : IFightStyle
{
    public string Name { get; set; }
    public List<Move> GetCurrentMoves()
    {
        throw new NotImplementedException();
    }
}