using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

public class WorldAsyncHelpers
{
    private TerrainGenerator _terrainGenerator;
    private World _worldReference;
    private CancellationTokenSource _cancellationTokenSource;

    public void Init(TerrainGenerator terrainGenerator, World worldReference, CancellationTokenSource cancellationTokenSource)
    {
        _terrainGenerator = terrainGenerator;
        _worldReference = worldReference;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public Task<ConcurrentDictionary<Vector3Int, ChunkData>> GenerateChunkVoxelsAsync(List<Vector3Int> chunkDataPositionsToGenerate)
    {
        ConcurrentDictionary<Vector3Int, ChunkData> data = new ();

        return Task.Run
        (
            () => 
            {
                foreach(var chunkPosition in chunkDataPositionsToGenerate)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    var chunk = new ChunkData(WorldData.ChunkSize, WorldData.ChunkHeight, chunkPosition, _worldReference);
                    _terrainGenerator.GenerateVoxels(chunk);
                    
                    data.TryAdd(chunkPosition, chunk);
                }

                return data;
            },
            _cancellationTokenSource.Token
        );
    }

    public Task<ConcurrentDictionary<Vector3Int, MeshData>> GenerateChunkMeshesAsync (List<Vector3Int> chunkRendererPositionsToGenerate, Dictionary<Vector3Int, ChunkData> chunksData)
    {
        ConcurrentDictionary<Vector3Int, MeshData> data = new ();
        return Task.Run
        (
            () => 
            {
                foreach(var chunkPosition in chunkRendererPositionsToGenerate)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    var chunk = chunksData[chunkPosition];
                    
                    data.TryAdd(chunkPosition, Chunk.GetChunkMeshData(chunk));
                }

                return data;
            },
            _cancellationTokenSource.Token
        );
    }
}
