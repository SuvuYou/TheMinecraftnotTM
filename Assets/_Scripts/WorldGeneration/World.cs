using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private ChunkRenderer _chunkRendererPrefab;
    [SerializeField] private TerrainGenerator _terrainGenerator;

    private Dictionary<Vector3Int, ChunkData> _chunks = new();
    private Dictionary<Vector3Int, ChunkRenderer> _chunkRenderers = new();

    public event Action OnWorldGenerated;

    public void GenerateWorld()
    {
        _generateWorld(Vector3Int.zero);
        OnWorldGenerated?.Invoke();
    }

    private void _generateWorld(Vector3Int centerPosition)
    {
        var chunksGenerationData = WorldData.GetChunksToGenerate(centerPosition, _chunkRenderers, _chunks);

        foreach(var chunkPosition in chunksGenerationData.ChunkRendererPositionsToRemove)
        {
            if (_chunkRenderers.ContainsKey(chunkPosition))
            {
                _chunkRenderers[chunkPosition].gameObject.SetActive(false);
                _chunkRenderers.Remove(chunkPosition);
            }
        }

        foreach(var chunkPosition in chunksGenerationData.ChunkDataPositionsToGenerate)
        {
            var chunk = new ChunkData(WorldData.ChunkSize, WorldData.ChunkHeight, chunkPosition, this);
            _chunks.Add(chunkPosition, chunk);
            _terrainGenerator.GenerateVoxels(chunk);
        }

        foreach(var chunkPosition in chunksGenerationData.ChunkRendererPositionsToGenerate)
        {
            var chunk = _chunks[chunkPosition];
            var meshData = Chunk.GetChunkMeshData(chunk);
            var chunkRenderer = Instantiate(_chunkRendererPrefab, chunk.WorldPosition, Quaternion.identity);
            _chunkRenderers.Add(chunk.WorldPosition, chunkRenderer);
            chunkRenderer.InitChunk(chunk);
            chunkRenderer.UpdateChunk(meshData);
        }
    }

    public bool TryGetChunkData(Vector3Int chunkPosition, out ChunkData chunk)
    {  
        if (_chunks.ContainsKey(chunkPosition))
        {
            chunk = _chunks[chunkPosition];
            return true;
        }

        chunk = null;
        return false;
    }

    public bool TryGetChunkRenderer(Vector3Int chunkPosition, out ChunkRenderer chunk)
    {  
        if (_chunkRenderers.ContainsKey(chunkPosition))
        {
            chunk = _chunkRenderers[chunkPosition];
            return true;
        }

        chunk = null;
        return false;
    }

    public void UpdateLoadChunks(Vector3 playerPosition)
    {
        var centerPosition = Chunk.GetChunkPositionFromWorldBlockPosition(Vector3Int.RoundToInt(playerPosition));
        _generateWorld(centerPosition);
    }

    public void ModifyBlock(Vector3Int blockWorldPosition, BlockType blockType)
    {
        var chunkPos = Chunk.GetChunkPositionFromWorldBlockPosition(blockWorldPosition);

        if (TryGetChunkData(chunkPos, out ChunkData chunk) && TryGetChunkRenderer(chunkPos, out ChunkRenderer chunkRenderer))
        {
            var blockLocalPosition = Chunk.GetLocalBlockPosition(chunk, blockWorldPosition);

            Chunk.SetBlock(chunk, blockLocalPosition, blockType);
            chunk.IsModifiedByPlayer = true;
            chunkRenderer.UpdateChunk();

            if (Chunk.IsBlockOnChunkEdge(blockLocalPosition))
            {
                var neighbours = Chunk.GetNeighbouringChunkPositions(chunk, blockLocalPosition);

                foreach (var neighbour in neighbours) 
                {
                    if (_chunkRenderers.ContainsKey(neighbour))
                        _chunkRenderers[neighbour].UpdateChunk();
                }
            }
        }
    }
}
