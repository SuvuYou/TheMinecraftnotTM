using UnityEngine;

public enum Direction
{
    forward,
    backward,
    right,
    left,
    up,
    down
}

public static class DirectionExtension
{
    public static Vector3Int GetDirectionVector(this Direction direction)
    {
        return direction switch
        {
            Direction.forward => Vector3Int.forward,
            Direction.backward  => Vector3Int.back,
            Direction.right  => Vector3Int.right,
            Direction.left  => Vector3Int.left,
            Direction.up  => Vector3Int.up,
            Direction.down  => Vector3Int.down,
            _ => throw new System.ArgumentException("Invalid direction: " + direction)
        };
    }
}