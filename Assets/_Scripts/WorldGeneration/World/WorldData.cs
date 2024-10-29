using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldData
{
    private static readonly int RenderDistance = 16;
    public static readonly int SeaLevel = 6;
    public static readonly int ChunkSize = 16;
    public static readonly int ChunkHeight = 100;
    public static readonly int BiomeSize = ChunkSize * 4;

    public static ChunksGenerationData GetChunksToGenerate(Vector3Int centerPosition, Dictionary<Vector3Int, ChunkRenderer> currentChunkRenderers, Dictionary<Vector3Int, ChunkData> currentChunksData)
    {
        var chunkRenderersToCreateAtPosition = _getChunkPositionsAroundCenterPoint(centerPosition, renderDistance: RenderDistance);
        var chunkRenderersToRemove = _getChunkRenderersToRemove(chunkRenderersToCreateAtPosition, currentChunkRenderers);

        chunkRenderersToCreateAtPosition = _getExcludedChunkPositions(chunkRenderersToCreateAtPosition, currentChunkRenderers.Keys.ToList());
        chunkRenderersToCreateAtPosition = _orderChunkPositionsByDistance(chunkRenderersToCreateAtPosition, centerPosition);

        var chunksDataToCreateAtPosition = _getChunkPositionsAroundCenterPoint(centerPosition, renderDistance: RenderDistance + 1);
        var chunkDataToRemove = _getChunkDataToRemove(chunksDataToCreateAtPosition, currentChunksData);

        chunksDataToCreateAtPosition = _getExcludedChunkPositions(chunksDataToCreateAtPosition, currentChunksData.Keys.ToList());
        chunksDataToCreateAtPosition = _orderChunkPositionsByDistance(chunksDataToCreateAtPosition, centerPosition);
        

        return new ChunksGenerationData () 
        { 
            ChunkDataPositionsToGenerate = chunksDataToCreateAtPosition, 
            ChunkRendererPositionsToGenerate = chunkRenderersToCreateAtPosition, 
            ChunkDataPositionsToRemove = chunkDataToRemove, 
            ChunkRendererPositionsToRemove = chunkRenderersToRemove, 
        };
    }

    private static List<Vector3Int> _getChunkRenderersToRemove(List<Vector3Int> chunkRenderersToCreateAtPosition, Dictionary<Vector3Int, ChunkRenderer> currentChunkRenderers)
    {
        var chunkRenderersToRemove = _getExcludedChunkPositions(currentChunkRenderers.Keys.ToList(), chunkRenderersToCreateAtPosition);

        return chunkRenderersToRemove;
    }

    private static List<Vector3Int> _getChunkDataToRemove(List<Vector3Int> chunksDataToCreateAtPosition, Dictionary<Vector3Int, ChunkData> currentChunksData)
    {
        var chunkDataToRemove = _getExcludedChunkPositions(currentChunksData.Keys.ToList(), chunksDataToCreateAtPosition);
        chunkDataToRemove = _getUnmodifiedChunkData(chunkDataToRemove, currentChunksData);

        return chunkDataToRemove;
    }

    private static List<Vector3Int> _getChunkPositionsAroundCenterPoint(Vector3Int centerPosition, int renderDistance)
    {
        List<Vector3Int> chunkPositions = new();

        int startPosX = centerPosition.x - renderDistance * ChunkSize;
        int startPosZ = centerPosition.z - renderDistance * ChunkSize;

        int endPosX = centerPosition.x + renderDistance * ChunkSize;
        int endPosZ = centerPosition.z + renderDistance * ChunkSize;

        for (int x = startPosX; x < endPosX; x += ChunkSize)
        {
            for (int z = startPosZ; z < endPosZ; z += ChunkSize)
            {
                chunkPositions.Add(new Vector3Int(x, 0, z));
            }
        }

        return chunkPositions;
    }

    private static List<Vector3Int> _orderChunkPositionsByDistance(List<Vector3Int> positions, Vector3Int referencePosition)
    {
        return positions.OrderBy(pos => Vector3Int.Distance(referencePosition, pos)).ToList();
    }

    private static List<Vector3Int> _getExcludedChunkPositions(List<Vector3Int> availableChunkpositions, List<Vector3Int> neededChunkpositions)
    {
        return availableChunkpositions.Except(neededChunkpositions).ToList();
    }

    private static List<Vector3Int> _getUnmodifiedChunkData(List<Vector3Int> availableChunkpositions, Dictionary<Vector3Int, ChunkData> currentChunksData)
    {
        return availableChunkpositions.Where(pos => currentChunksData[pos].IsModifiedByPlayer == false).ToList();
    }
}

public struct ChunksGenerationData
{
    public List<Vector3Int> ChunkDataPositionsToGenerate;
    public List<Vector3Int> ChunkRendererPositionsToGenerate;
    public List<Vector3Int> ChunkDataPositionsToRemove;
    public List<Vector3Int> ChunkRendererPositionsToRemove;
}