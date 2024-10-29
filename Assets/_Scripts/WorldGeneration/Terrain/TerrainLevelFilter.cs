using System.Linq;
using UnityEngine;

public enum TerrainLevel
{
    Air,
    Sea,
    Surface,
    SeaSurface,
    MiddleGround,
    Ground,
    StonePatches,
    Trees
}

public static class TerrainLevelExtention
{
    public static ITerrainLevelFilter GetTerrainLevelFilter(this TerrainLevel terrainLevel)
    {
        return terrainLevel switch 
        {
            TerrainLevel.Air => new AirTerrainLevelFilter(),
            TerrainLevel.Sea => new SeaTerrainLevelFilter(),
            TerrainLevel.Surface => new SurfaceTerrainLevelFilter(),
            TerrainLevel.SeaSurface => new SeaSurfaceTerrainLevelFilter(),
            TerrainLevel.MiddleGround => new MiddleGroundTerrainLevelFilter(),
            TerrainLevel.Ground => new GroundTerrainLevelFilter(),
            TerrainLevel.StonePatches => new StonePatchesTerrainLevelFilter(),
            TerrainLevel.Trees => new TreesTerrainLevelFilter(),
            _ => null
        };
    }    
}

public struct BlockGenerationParameters
{
    public Vector3Int BlockPosition;
    public int GroundLevel;
    public float StonePatchesProbability;
    public ChunkData Chunk;

    public BlockGenerationParameters(ChunkData chunk, Vector3Int blockPosition, int groundLevel, float stonePatchesProbability = 0f)
    {
        Chunk = chunk;
        BlockPosition = blockPosition;
        GroundLevel = groundLevel;
        StonePatchesProbability = stonePatchesProbability;
    }
}

public interface ITerrainLevelFilter
{
    public abstract bool IsBlockInTerrainLevel(BlockGenerationParameters parameters);  
}

public class AirTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y > parameters.GroundLevel && parameters.BlockPosition.y > WorldData.SeaLevel;
    }
}

public class SeaTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y > parameters.GroundLevel && parameters.BlockPosition.y <= WorldData.SeaLevel;
    }
}

public class SurfaceTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y == parameters.GroundLevel && parameters.BlockPosition.y > WorldData.SeaLevel;
    }
}

public class SeaSurfaceTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y == parameters.GroundLevel && parameters.BlockPosition.y <= WorldData.SeaLevel;
    }
}

public class MiddleGroundTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y + 1 == parameters.GroundLevel || parameters.BlockPosition.y + 2 == parameters.GroundLevel || parameters.BlockPosition.y + 3 == parameters.GroundLevel;
    }
}

public class GroundTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y < parameters.GroundLevel;
    }
}

public class TreesTerrainLevelFilter : ITerrainLevelFilter
{
    private int _treeHeight = 5;

    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        var globalBlockPosition = Chunk.GetWorldBlockPosition(parameters.Chunk, parameters.BlockPosition);

        if (!parameters.Chunk.Trees.TreePositions.Contains(new Vector2Int(globalBlockPosition.x, globalBlockPosition.z))) 
            return false;

        if (parameters.BlockPosition.y <= WorldData.SeaLevel) 
            return false;  

        if (parameters.BlockPosition.y <= parameters.GroundLevel || parameters.BlockPosition.y > parameters.GroundLevel + _treeHeight) 
            return false;  

        var groundBlock = Chunk.GetBlock(parameters.Chunk, new Vector3Int(parameters.BlockPosition.x, parameters.GroundLevel, parameters.BlockPosition.z));

        if (groundBlock != BlockType.Grass_Dirt) 
            return false;

        // TODO: this is just meehhh
        if (parameters.BlockPosition.y == parameters.GroundLevel + _treeHeight)
        {
            parameters.Chunk.Trees.LeefPositions = parameters.Chunk.Trees.LeefPositions.Concat(TreesNoise.GetTreeLeeves(parameters.BlockPosition)).ToList();
        }

        return true;
    }
}

public class StonePatchesTerrainLevelFilter : ITerrainLevelFilter
{
    private float _stonePatchesThreshold = 0.5f;

    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y <= parameters.GroundLevel && parameters.BlockPosition.y > WorldData.SeaLevel && parameters.StonePatchesProbability > _stonePatchesThreshold;
    }
}
