using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private ChunkRenderer _chunkRendererPrefab;
    [SerializeField] private float _noiseScale = 0.03f;

    private int _worldSizeInChunks = 16;
    public static int ChunkSize = 16;
    public static int ChunkHeight = 100;
    private int _seaLevel = 50;

    private Dictionary<Vector3Int, ChunkData> _chunks = new();
    private Dictionary<Vector3Int, ChunkRenderer> _chunkRenderers = new();

    public void GenerateWorld()
    {
        foreach(var renderer in _chunkRenderers.Values)
        {
            Destroy(renderer.gameObject);
        }

        _chunks.Clear();
        _chunkRenderers.Clear();

        for (int x = 0; x < _worldSizeInChunks; x++)
        {
            for (int z = 0; z < _worldSizeInChunks; z++)
            {
                var newChunk = new ChunkData(ChunkSize, ChunkHeight, new Vector3Int(x * ChunkSize, 0, z * ChunkSize), this);
                _chunks.Add(newChunk.WorldPosition, newChunk);
                Chunk.GenerateVoxels(newChunk, _noiseScale, _seaLevel);
            }
        }

        foreach(var chunk in _chunks.Values)
        {
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
}
