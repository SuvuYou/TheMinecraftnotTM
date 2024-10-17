using UnityEngine;

public enum TerrainLevel
{
    Air,
    Sea,
    Surface,
    SeaSurface,
    MiddleGround,
    Ground,
    StonePatches
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
            _ => null
        };
    }    
}

public struct BlockGenerationParameters
{
    public Vector3Int BlockPosition;
    public int GroundLevel;
    public float StonePatchesProbability;

    public BlockGenerationParameters(Vector3Int blockPosition, int groundLevel, float stonePatchesProbability = 0f)
    {
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
        return parameters.BlockPosition.y > parameters.GroundLevel && parameters.BlockPosition.y > World.SeaLevel;
    }
}

public class SeaTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y > parameters.GroundLevel && parameters.BlockPosition.y <= World.SeaLevel;
    }
}

public class SurfaceTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y == parameters.GroundLevel && parameters.BlockPosition.y > World.SeaLevel;
    }
}

public class SeaSurfaceTerrainLevelFilter : ITerrainLevelFilter
{
    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y == parameters.GroundLevel && parameters.BlockPosition.y <= World.SeaLevel;
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

public class StonePatchesTerrainLevelFilter : ITerrainLevelFilter
{
    private float _stonePatchesThreshold = 0.5f;

    public bool IsBlockInTerrainLevel(BlockGenerationParameters parameters)
    {
        return parameters.BlockPosition.y <= parameters.GroundLevel && parameters.BlockPosition.y > World.SeaLevel && parameters.StonePatchesProbability > _stonePatchesThreshold;
    }
}
