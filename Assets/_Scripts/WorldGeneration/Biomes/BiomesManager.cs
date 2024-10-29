using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BiomesManager
{
    private static readonly List<Vector2Int> BIOME_EDGE_NEIGHBORS = new ()
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private static readonly List<Vector2Int> BIOME_CORNER_NEIGHBORS = new()
    {
        Vector2Int.up + Vector2Int.left,
        Vector2Int.up + Vector2Int.right,
        Vector2Int.down + Vector2Int.left,
        Vector2Int.down + Vector2Int.right  
    };

    public static List<Vector3Int> GetBiomePositions(Vector3Int centerBiomePosition)
    {
        var currentBiomeLocalPosition = GetBiomeLocalPositionFromWorldPosition(centerBiomePosition);

        List<Vector3Int> biomePositions = new ();

        foreach (var direction in BIOME_EDGE_NEIGHBORS)
        {
            biomePositions.Add(new Vector3Int(currentBiomeLocalPosition.x + direction.x, 0, currentBiomeLocalPosition.z + direction.y));
            biomePositions.Add(new Vector3Int(currentBiomeLocalPosition.x + (direction.x * 2), 0, currentBiomeLocalPosition.z + (direction.y * 2)));
        }

        foreach (var direction in BIOME_CORNER_NEIGHBORS)
        {
            biomePositions.Add(new Vector3Int(currentBiomeLocalPosition.x + direction.x, 0, currentBiomeLocalPosition.z + direction.y));
            biomePositions.Add(new Vector3Int(currentBiomeLocalPosition.x + (direction.x * 2), 0, currentBiomeLocalPosition.z + (direction.y * 2)));
            biomePositions.Add(new Vector3Int(currentBiomeLocalPosition.x + direction.x, 0, currentBiomeLocalPosition.z + (direction.y * 2)));
            biomePositions.Add(new Vector3Int(currentBiomeLocalPosition.x + (direction.x * 2), 0, currentBiomeLocalPosition.z + direction.y));
        }

        return _convertBiomeLocalPositionToWorldPosition(biomePositions);
    }

    private static List<Vector3Int> _convertBiomeLocalPositionToWorldPosition(List<Vector3Int> localPosition)
    {
        return localPosition.Select(pos => pos * WorldData.BiomeSize).ToList();
    }

    public static Vector3Int GetBiomeLocalPositionFromWorldPosition(Vector3Int worldPosition)
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPosition.x / WorldData.BiomeSize) * WorldData.BiomeSize, 
            0, 
            Mathf.FloorToInt(worldPosition.z / WorldData.BiomeSize) * WorldData.BiomeSize
            );
    }
}