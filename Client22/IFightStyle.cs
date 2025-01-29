using BadgerClan.Logic;
namespace Client22;
public class Mode
{
    public IFightStyle style=new Default();
    public List<Move> GetMoves(MoveRequest request)
    {
        return style.GetCurrentMoves();
    }
}
public interface IFightStyle
{
    public string Name { get; }
    public List<Move> GetCurrentMoves(); 
}
public class Gay : IFightStyle
{
    public string Name { get; private set; } = "Gay";
    public List<Move> GetCurrentMoves()
    {
        throw new NotImplementedException();
    }
}
public class Straight : IFightStyle
{
    public string Name => "Straight";

    public List<Move> GetCurrentMoves()
    {
        throw new NotImplementedException();
    }
}
public class Default : IFightStyle
{
    public string Name => "Default";

    public List<Move> GetCurrentMoves()
    {
        throw new NotImplementedException();
    }
}