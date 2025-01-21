
using System.Security.AccessControl;
using BadgerClan.Logic;

namespace BadgerClan.Test;

public class CoordinateTests
{

    [Theory]
    [InlineData(2, 2, 3, 2, 1)]
    [InlineData(1, 1, 3, 1, 2)]
    [InlineData(1, 1, 5, 4, 5)]
    public void Distance(int a1, int a2, int b1, int b2, int expected)
    {
        var p1 = Coordinate.Offset(a1, a2);
        var p2 = Coordinate.Offset(b1, b2);
        var distance = p1.Distance(p2);
        Assert.Equal(expected, distance);
    }

    [Fact]
    public void MoveEast()
    {
        var p1 = Coordinate.Offset(3, 3);
        var result = p1.MoveEast(1);
        Assert.Equal(4, result.Col);
        Assert.Equal(3, result.Row);
    }

    [Fact]
    public void MoveWest()
    {
        var p1 = Coordinate.Offset(3, 3);
        var result = p1.MoveWest(1);
        Assert.Equal(2, result.Col);
        Assert.Equal(3, result.Row);
    }

    [Fact]
    public void MoveSouthEast()
    {
        var p1 = Coordinate.Offset(3, 3);
        var result = p1.MoveSouthEast(1);
        Assert.Equal(4, result.Col);
        Assert.Equal(4, result.Row);
    }

    [Fact]
    public void MoveNorthWest()
    {
        var p1 = Coordinate.Offset(3, 3);
        var result = p1.MoveNorthWest(1);
        Assert.Equal(3, result.Col);
        Assert.Equal(2, result.Row);
    }

    [Fact]
    public void MoveSouthWest()
    {
        var p1 = Coordinate.Offset(3, 3);
        Assert.Equal(2, p1.Q);
        Assert.Equal(3, p1.R);
        var result = p1.MoveSouthWest(1);
        Assert.Equal(1, result.Q); //-1
        Assert.Equal(4, result.R); //4

        Assert.Equal(3, result.Col);
        Assert.Equal(4, result.Row);
    }

    [Fact]
    public void MoveNorthEast()
    {
        var p1 = Coordinate.Offset(3, 3);
        var result = p1.MoveNorthEast(1);
        Assert.Equal(4, result.Col);
        Assert.Equal(2, result.Row);
    }

    [Fact]
    public void AreEqualTest()
    {
        var p1 = new Coordinate(5, 12);
        var p2 = new Coordinate(5, 12);
        Assert.Equal(p1, p2);
    }

    [Fact]
    public void GetNeighbors()
    {
        var p1 = new Coordinate(4, 4);
        var neighbors = p1.Neighbors();
        Assert.Equal(6, neighbors.Count);
    }

    [Fact]
    public void TowardTest()
    {
        var point = Coordinate.Offset(1,1);
        var toward = point.Toward(Coordinate.Offset(3,4));
        var expected = Coordinate.Offset(2,2);
        Assert.Equal(expected, toward);
    }
}