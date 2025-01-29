using BadgerClan.Logic;
namespace Client22;
public class Mode
{
    public IFightStyle style=new Default();
    public List<Move> GetMoves(MoveRequest request)
    {
        return style.GetCurrentMoves(request);
    }
}
public interface IFightStyle
{
    public string Name { get; }
    public List<Move> GetCurrentMoves(MoveRequest request); 
}
public class Hold : IFightStyle
{
    public string Name { get; private set; } = "Hold";
    public List<Move> GetCurrentMoves(MoveRequest request)
    {
        var results = new List<Move>();
        var ownUnits = request.Units.Where(p=>p.Team==request.YourTeamId).ToList();
        var otherUnits = request.Units.Where(p => p.Team != request.YourTeamId).ToList();
        foreach(UnitDto unit in ownUnits)
        {
            double distance=10000000;
            UnitDto target = null;
            foreach(UnitDto enemy in otherUnits)
            {
                if (enemy.Location.Distance(unit.Location) < distance)
                {
                    target = enemy;
                    distance = enemy.Location.Distance(unit.Location);
                }
            }
            results.Add(new Move(MoveType.Attack, unit.Id, target!.Location));
        }
        return results;
    }
}
public class Attack : IFightStyle
{
    public string Name => "Attack";

    public List<Move> GetCurrentMoves(MoveRequest request)
    {
        var results = new List<Move>();
        var ownUnits = request.Units.Where(p => p.Team == request.YourTeamId).ToList();
        var otherUnits = request.Units.Where(p => p.Team != request.YourTeamId).ToList();
        foreach (UnitDto unit in ownUnits)
        {
            double distance = 10000000;
            UnitDto target = null;
            foreach (UnitDto enemy in otherUnits)
            {
                if (enemy.Location.Distance(unit.Location) < distance)
                {
                    target = enemy;
                    distance = enemy.Location.Distance(unit.Location);
                }
            }
            results.Add(new Move(MoveType.Walk, unit.Id, target!.Location));
        }
        return results;
    }
}
public class Default : IFightStyle
{
    public string Name => "Default";

    public List<Move> GetCurrentMoves(MoveRequest request)
    {
        throw new NotImplementedException();
    }
}