using BadgerClan.Logic;
using System.Collections.Generic;
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
            var ownUnits = lastKnown.Units.Where(p => p.Team == lastKnown.YourTeamId && p.Type == "Knight").OrderBy<UnitDto, int>(p => p.Id).ToList();
            var ownUnitsA = lastKnown.Units.Where(p => p.Team == lastKnown.YourTeamId && p.Type == "Archer").OrderBy<UnitDto, int>(p => p.Id).ToList();
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
                    true => rightFunc.Invoke(right * 2),
                    false => leftFunc.Invoke(left * 2),
                });
                if (isRight) right++;
                if (!isRight) left++;
                isRight = !isRight;

            }
            foreach (var unit in ownUnits)
            {
                result.Add(unit, baseLocation.Toward(baseLocation.MoveNorthEast(1).MoveNorthWest(1)));


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

        var ownUnits = request.Units.Where(p => p.Team == request.YourTeamId).ToList();
        var otherUnits = request.Units.Where(p => p.Team != request.YourTeamId).OrderBy<UnitDto, int>(p => p.Location.Distance(ownUnits[0].Location)).ToList();
        //finding direction
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
            if (row >= 0)
                direction = 1;
            else
                direction = 3;
        }
        //setup
        Dictionary<UnitDto, Coordinate> locations = TargetSpots(direction);
        Dictionary<UnitDto, int> healths = new Dictionary<UnitDto, int>();
        Dictionary<UnitDto, Coordinate> tempLocation = new Dictionary<UnitDto, Coordinate>();
        int Medpacs = request.Medpacs;
        foreach (UnitDto unit in request.Units)
        {
            healths.Add(unit, unit.Health);
            tempLocation.Add(unit, unit.Location);
            moves[unit] = 0;
        }
        //calc moves
        foreach (UnitDto unit in ownUnits)
        {
            //unit setup
            otherUnits = otherUnits.OrderBy<UnitDto, int>(p => p.Location.Distance(unit.Location)).ToList();
            var inRange = otherUnits.Where(p => p.Location.Distance(unit.Location) < unit.AttackDistance).OrderBy<UnitDto, int>(p => p.Location.Distance(unit.Location)).ToList();
            //heal
            if (unit.Health < 4 && Medpacs != 0)
            {
                results.Add(new(MoveType.Medpac, unit.Id, unit.Location));
                Medpacs--;
                moves[unit]++;
            }
            //enemy in range
            var target = inRange[0];
            while (moves[unit] != unit.MaxMoves && inRange.Count != 0)
            {
                if (healths[target] <= 0)
                {
                    otherUnits.Remove(target);
                    inRange.Remove(target);
                    if (inRange.Count != 0)
                        target = inRange[0];
                    else
                        break;
                }
                results.Add(new(MoveType.Attack, unit.Id, target.Location));
                moves[unit]++;
                healths[target] -= unit.Attack;
            }
            //move into position
            while (moves[unit] != unit.MaxMoves && tempLocation[unit] != locations[unit]) ;
            {
                var occupied = request.Units.Where(p => p.Location == tempLocation[unit].Toward(locations[unit]));
                if (occupied.Count() == 0 || healths[occupied.First()] == 0)
                {
                    results.Add(new(MoveType.Walk, unit.Id, tempLocation[unit].Toward(locations[unit])));
                    tempLocation[unit] = tempLocation[unit].Toward(locations[unit]);
                    moves[unit]++;
                }
                else break;
            }
            //advance
            target = otherUnits[0];
            while (moves[unit] != unit.MaxMoves)
            {
                var occupied = request.Units.Where(p => p.Location == tempLocation[unit].Toward(target.Location));
                if (occupied.Count() == 0 || healths[occupied.First()] == 0)
                {
                    results.Add(new(MoveType.Walk, unit.Id, tempLocation[unit].Toward(target.Location)));
                    tempLocation[unit] = tempLocation[unit].Toward(locations[unit]);
                    moves[unit]++;
                }
                else break;
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