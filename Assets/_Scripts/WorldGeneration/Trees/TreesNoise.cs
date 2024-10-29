using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TreesNoise
{
    public static List<Vector3Int> TREE_LEEVES_POSITIONS = new ()
    {
        new (-2, 0, -2),
        new (-2, 0, -1),
        new (-2, 0, 0),
        new (-2, 0, 1),
        new (-2, 0, 2),
        new (-1, 0, -2),
        new (-1, 0, -1),
        new (-1, 0, 0),
        new (-1, 0, 1),
        new (-1, 0, 2),
        new (0, 0, -2),
        new (0, 0, -1),
        new (0, 0, 0),
        new (0, 0, 1),
        new (0, 0, 2),
        new (1, 0, -2),
        new (1, 0, -1),
        new (1, 0, 0),
        new (1, 0, 1),
        new (1, 0, 2),
        new (2, 0, -2),
        new (2, 0, -1),
        new (2, 0, 0),
        new (2, 0, 1),
        new (2, 0, 2),
        new (-1, 1, -1),
        new (-1, 1, 0),
        new (-1, 1, 1),
        new (0, 1, -1),
        new (0, 1, 0),
        new (0, 1, 1),
        new (1, 1, -1),
        new (1, 1, 0),
        new (1, 1, 1),
        new (0, 2, 0)
    };

    private static readonly List<Vector2Int> NEIGHBOUR_DIRECTIONS = new ()
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
        };

    public static List<Vector2Int> GetTreeData(ChunkData chunk, DomainWarpingSettings domainWarpingSettings, NoiseSettings settings, Vector2Int worldSeedOffset)
    {
        Dictionary<Vector2Int, float> noiseValues = _getTreesNoise(chunk, domainWarpingSettings, settings, worldSeedOffset);

        List<Vector2Int> treesPositions = _findLocalMaxima(noiseValues);

        return treesPositions;
    }

    public static List<Vector3Int> GetTreeLeeves(Vector3Int treeTopPosition)
    {
        List<Vector3Int> leevesPositions = new ();

        foreach (Vector3Int leafPosition in TREE_LEEVES_POSITIONS)
        {
            leevesPositions.Add(new Vector3Int(treeTopPosition.x + leafPosition.x, treeTopPosition.y + leafPosition.y, treeTopPosition.z + leafPosition.z));
        }

        return leevesPositions;
    }

    private static Dictionary<Vector2Int, float> _getTreesNoise(ChunkData chunk, DomainWarpingSettings domainWarpingSettings, NoiseSettings settings, Vector2Int worldSeedOffset)
    {
        Dictionary<Vector2Int, float> noiseValues = new ();

        var chunkStartX = chunk.WorldPosition.x;
        var chunkEndX = chunk.WorldPosition.x + WorldData.ChunkSize;

        var chunkStartZ = chunk.WorldPosition.z;
        var chunkEndZ = chunk.WorldPosition.z + WorldData.ChunkSize;

        for (int x = chunkStartX; x < chunkEndX; x++)
        {
            for (int z = chunkStartZ; z < chunkEndZ; z++)
            {
                noiseValues.Add(new Vector2Int(x, z), DomainWarping.GetNoiseValue(x, z, domainWarpingSettings, settings, worldSeedOffset));
            }
        }

        return noiseValues;
    }

    private static List<Vector2Int> _findLocalMaxima(Dictionary<Vector2Int, float> values)
    {
        List<Vector2Int> localMaxima = new ();

        foreach (var position in values.Keys)
        {
            var localMaximaValue = values[position];

            if (NEIGHBOUR_DIRECTIONS.Where(direction => values.ContainsKey(position + direction)).All(direction => values[position + direction] < localMaximaValue))
            {
                localMaxima.Add(position);
            }
        }
        
        return localMaxima;
    }
}