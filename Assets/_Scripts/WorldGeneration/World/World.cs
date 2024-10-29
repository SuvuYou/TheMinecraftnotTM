using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;

public class World : MonoBehaviour
{
    [SerializeField] private ChunkRenderer _chunkRendererPrefab;
    [SerializeField] private TerrainGenerator _terrainGenerator;
    [SerializeField] private GameObject _chunkParent;

    private Dictionary<Vector3Int, ChunkData> _chunks = new();
    private Dictionary<Vector3Int, ChunkRenderer> _chunkRenderers = new();

    private WorldAsyncHelpers _worldAsyncHelper;
    private CancellationTokenSource _cancellationTokenSource = new();

    private ChunkQueue _chunkQueue = new();

    public event Action OnWorldGenerated;

    private bool _isUpdating = false;

    private void Start()
    {
        _worldAsyncHelper = new WorldAsyncHelpers();
        _worldAsyncHelper.Init(_terrainGenerator, this, _cancellationTokenSource);
        _chunkQueue.Init(generateChunkRenderer: (chunk) => Instantiate(_chunkRendererPrefab, chunk.WorldPosition, Quaternion.identity, _chunkParent.transform));
    }

    public void GenerateWorld()
    {
        _generateWorld(Vector3Int.zero, shouldTriggerOnWorldGeneratedEvent: true);
    }

    private async void _generateWorld(Vector3Int centerPosition, bool shouldTriggerOnWorldGeneratedEvent = false)
    {
        var chunksGenerationData = WorldData.GetChunksToGenerate(centerPosition, _chunkRenderers, _chunks);

        foreach(var chunkPosition in chunksGenerationData.ChunkRendererPositionsToRemove)
        {
            if (_chunkRenderers.ContainsKey(chunkPosition))
            {
                _chunkQueue.ReturnChunk(_chunkRenderers[chunkPosition]);
                _chunkRenderers.Remove(chunkPosition);
            }
        }

        _terrainGenerator.GenerateBiomesTemperatures(centerPosition);

        ConcurrentDictionary<Vector3Int, ChunkData> concurrentChunksData;
        ConcurrentDictionary<Vector3Int, MeshData> concurrentChunkMeshes;

        try
        {
            concurrentChunksData = await _worldAsyncHelper.GenerateChunkVoxelsAsync(chunksGenerationData.ChunkDataPositionsToGenerate);
        } 
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }

        foreach(var chunkPosition in concurrentChunksData.Keys)
        {
            _chunks.Add(chunkPosition, concurrentChunksData[chunkPosition]);
        }

        foreach(var chunkPosition in _chunks.Keys)
        {
            foreach(var leefPosition in _chunks[chunkPosition].Trees.LeefPositions)
            {
                Chunk.SetBlock(_chunks[chunkPosition], leefPosition, BlockType.TreeLeafsSolid);
            }
        }

        try
        {
            concurrentChunkMeshes = await _worldAsyncHelper.GenerateChunkMeshesAsync(chunksGenerationData.ChunkRendererPositionsToGenerate, _chunks);
        } 
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }

        StartCoroutine(_renderChunksCoroutine(concurrentChunkMeshes, referencePosition: centerPosition, shouldTriggerOnWorldGeneratedEvent: shouldTriggerOnWorldGeneratedEvent));
    }

    private IEnumerator _renderChunksCoroutine(ConcurrentDictionary<Vector3Int, MeshData> chunkMeshes, Vector3Int referencePosition, bool shouldTriggerOnWorldGeneratedEvent = false)
    {
        foreach(var chunkMesh in chunkMeshes.Keys.ToList().OrderBy(pos => Vector3Int.Distance(referencePosition, pos)))
        {
            var chunk = _chunks[chunkMesh];

            if (_chunkRenderers.ContainsKey(chunk.WorldPosition))
            {
                continue;
            }

            var chunkRenderer = _chunkQueue.PoolChunk(chunk);
            chunkRenderer.InitChunk(chunk);
            chunkRenderer.UpdateChunk();
            _chunkRenderers.Add(chunk.WorldPosition, chunkRenderer);

            yield return new WaitForEndOfFrame();
        }

        if (shouldTriggerOnWorldGeneratedEvent)
        {
            OnWorldGenerated?.Invoke();
        }
    }

    public void UpdateLoadChunks(Vector3 playerPosition)
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        var centerPosition = Chunk.GetChunkPositionFromWorldBlockPosition(Vector3Int.RoundToInt(playerPosition));
        _generateWorld(centerPosition);
        _isUpdating = false;
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

    private void OnDisable() => _cancellationTokenSource.Cancel();
}
