using System;
using System.Collections.Generic;

public class ChunkQueue
{
    private Queue<ChunkRenderer> _chunkRendererQueue = new ();
    private Func<ChunkData, ChunkRenderer> _generateChunkRenderer;

    public void Init(Func<ChunkData, ChunkRenderer> generateChunkRenderer)
    {
        _generateChunkRenderer = generateChunkRenderer;
    }

    public ChunkRenderer PoolChunk(ChunkData chunk)
    {
        ChunkRenderer pooledChunkRenderer;

        if (_chunkRendererQueue.Count > 0)
        {
            pooledChunkRenderer = _chunkRendererQueue.Dequeue();
            pooledChunkRenderer.gameObject.transform.position = chunk.WorldPosition;
        }
        else
        {
            pooledChunkRenderer = _generateChunkRenderer(chunk);
        }

        pooledChunkRenderer.gameObject.SetActive(true);

        return pooledChunkRenderer;
    }

    public void ReturnChunk(ChunkRenderer chunkRenderer)
    {
        chunkRenderer.gameObject.SetActive(false);
        _chunkRendererQueue.Enqueue(chunkRenderer);
    }
}
