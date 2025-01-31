using BadgerClan.Logic;
namespace Client22;
public class Mode
{
    public IFightStyle style = new Default();
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
        var ownInfantryUnits = request.Units.Where(p => p.Team == request.YourTeamId).ToList();
        var ownArcherUnits = request.Units.Where(p => p.Team == request.YourTeamId).ToList();
        var otherUnits = request.Units.Where(p => p.Team != request.YourTeamId).ToList();

        foreach (UnitDto unit in ownInfantryUnits)
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
            results.Add(new Move(MoveType.Attack, unit.Id, unit.Location.Toward(target!.Location)));
        }
        return results;
    }
}
public class Attack : IFightStyle
{
    Dictionary<UnitDto, Coordinate> TargetSpots(int direction)
    {

        var result = new Dictionary<UnitDto, Coordinate>();
        if (lastKnown != null)
        {
            var ownUnits = lastKnown.Units.Where(p => p.Team == lastKnown.YourTeamId&&p.Type=="Knight").OrderBy<UnitDto, int>(p => p.Id).ToList();
            var baseLocation = ownUnits[0].Location;
            int left = 0;
            int right = 1;
            bool isRight = false;
            Func<int, Coordinate> rightFunc = direction switch
            {
                0 => baseLocation.MoveSouthEast,
                1 => baseLocation.MoveSouthWest,
                2 => baseLocation.MoveNorthWest,
                3 => baseLocation.MoveNorthEast,
                _ => (p) => new(0, 0)
            };
            Func<int, Coordinate> leftFunc = direction switch
            {
                0 => baseLocation.MoveSouthWest,
                1 => baseLocation.MoveNorthWest,
                2 => baseLocation.MoveNorthEast,
                3 => baseLocation.MoveSouthEast,
                _ => (p) => new(0, 0)
            };
            foreach (var unit in ownUnits) 
            {
                result.Add(unit, isRight switch
                {
                    true=>rightFunc.Invoke(right*2),
                    false => leftFunc.Invoke(left*2),
                });
                if (isRight) right++;
                if(!isRight) left++;
                isRight = !isRight;

            }

        }
        return result;
    }

    public string Name => "Attack";
    MoveRequest? lastKnown;
    public List<Move> GetCurrentMoves(MoveRequest request)
    {
        lastKnown = request;
        var results = new List<Move>();
        var otherUnits = request.Units.Where(p => p.Team != request.YourTeamId).ToList();

        var ownUnits = request.Units.Where(p => p.Team == request.YourTeamId).ToList();
        var col = ownUnits[0].Location.Col - otherUnits[0].Location.Col;
        var row = otherUnits[0].Location.Row - otherUnits[0].Location.Row;
        var direction = 0;
        var moves = new Dictionary<UnitDto, int>();
        if (Math.Abs(col) >= Math.Abs(row))
        {
            if (col >= 0)
                direction = 0;
            else
                direction = 2;
        }
        else
        {
            if(row >= 0)
                direction = 1;
            else
                direction = 3;
        }
        Dictionary<UnitDto, Coordinate> locations=TargetSpots(direction);
        foreach (UnitDto unit in ownUnits)
        {
            otherUnits = otherUnits.OrderBy<UnitDto, int>(p => p.Location.Distance(unit.Location)).ToList();
            moves[unit] = 0;
            if(unit.Health<4)
            {
                results.Add(new(MoveType.Medpac, unit.Id, unit.Location));
                
                moves[unit]++;
            }
        }
        return results;
    }
}
public class Default : IFightStyle
{
    public string Name => "Default";

    public List<Move> GetCurrentMoves(MoveRequest request)
    {
        return new();
    }
}