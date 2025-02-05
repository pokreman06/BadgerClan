using BadgerClan.Logic;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
namespace Client22;
public class Mode
{
    public IFightStyle style=new Default();
    public MoveResponse GetMoves(MoveRequest request)
    {
        return new MoveResponse(style.GetCurrentMoves(request));
    }
}
public interface IFightStyle
{
    public string Name { get; }
    public List<Move> GetCurrentMoves(MoveRequest request);
}

public class SuperSimpleExampleBot : IFightStyle
{
    public string Name => "Basic";

    public List<Move> GetCurrentMoves(MoveRequest gameState)
    {
        var myTeamId = gameState.YourTeamId;
        var myUnits = findMyUnits(myTeamId, gameState.Units);
        var enemies = findEnemies(myTeamId, gameState.Units);
        var moves = new List<Move>();

        foreach (var unit in myUnits)
        {
            var closestEnemy = findClosest(unit, enemies);
            bool iCanAttack = closestEnemy.Location.Distance(unit.Location) <= unit.AttackDistance;
            bool iHaveHealthPacksAvailable = gameState.Medpacs > 0;
            bool iNeedHealth = unit.Health < unit.MaxHealth;

            if (iCanAttack)
            {
                //You are allowed two movements per turn.
                moves.Add(new Move(MoveType.Attack, unit.Id, closestEnemy.Location));
                moves.Add(new Move(MoveType.Attack, unit.Id, closestEnemy.Location));
            }
            else if (iNeedHealth && iHaveHealthPacksAvailable)
            {
                moves.Add(new Move(MoveType.Medpac, unit.Id, unit.Location));
            }
            else
            {
                moves.Add(new Move(MoveType.Walk, unit.Id, unit.Location.Toward(closestEnemy.Location)));
            }
        }

        return moves;
    }

    private static List<UnitDto> findMyUnits(int myTeamId, IEnumerable<UnitDto> units)
    {
        var myUnits = new List<UnitDto>();
        foreach (var unit in units)
        {
            if (unit.Team == myTeamId)
            {
                myUnits.Add(unit);
            }
        }

        return myUnits;
    }

    private static List<UnitDto> findEnemies(int myTeamId, IEnumerable<UnitDto> units)
    {
        var enemies = new List<UnitDto>();
        foreach (var unit in units)
        {
            if (unit.Team != myTeamId)
            {
                enemies.Add(unit);
            }
        }

        return enemies;
    }

    private static UnitDto findClosest(UnitDto unit, List<UnitDto> otherUnits)
    {
        var closest = otherUnits[0];
        foreach (var enemy in otherUnits)
        {
            if (enemy.Location.Distance(unit.Location) < closest.Location.Distance(unit.Location))
            {
                closest = enemy;
            }
        }

        return closest;
    }
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
            foreach (var unit in ownUnitsA)
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
        if (ownUnits.Count() == 0)
            return new();
        var otherUnits = request.Units.Where(p => p.Team != request.YourTeamId).OrderBy<UnitDto, int>(p => p.Location.Distance(ownUnits.FirstOrDefault()?.Location ?? new(0,0))).ToList();
        //finding direction
        var col = (ownUnits.FirstOrDefault()?.Location ?? new(0, 0)).Col - (otherUnits.FirstOrDefault()?.Location ?? new(0, 0)).Col;
        var row = (ownUnits.FirstOrDefault()?.Location ?? new(0, 0)).Row -    (otherUnits.FirstOrDefault()?.Location ?? new(0, 0)).Row;
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
            var target = inRange.FirstOrDefault();
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
            while (moves[unit] != unit.MaxMoves && tempLocation[unit] != locations[unit]);
            {
                var occupied = request.Units.Where(p => p.Location == tempLocation[unit].Toward(locations[unit]));

                    results.Add(new(MoveType.Walk, unit.Id, tempLocation[unit].Toward(locations[unit])));
                    tempLocation[unit] = tempLocation[unit].Toward(locations[unit]);
                    moves[unit]++;

            }
            //advance
            target = otherUnits[0];
            while (moves[unit] != unit.MaxMoves)
            {
                var occupied = request.Units.Where(p => p.Location == tempLocation[unit].Toward(target.Location));
                    results.Add(new(MoveType.Walk, unit.Id, tempLocation[unit].Toward(target.Location)));
                    tempLocation[unit] = tempLocation[unit].Toward(locations[unit]);
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
        var results = new List<Move>();
        var ownUnits = request.Units.Where(p => p.Team == request.YourTeamId).ToList();
        var otherUnits = request.Units.Where(p => p.Team != request.YourTeamId).ToList();
        foreach (UnitDto unit in ownUnits)
        {
            results.Add(new Move(MoveType.Walk, unit.Id, unit.Location.Toward(new Coordinate(1000000000,10000000))));
        }
            return results;
    }
}