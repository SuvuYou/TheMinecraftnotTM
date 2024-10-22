using System;
using System.Collections.Generic;
using UnityEngine;

public static class Chunk
{
    public static void SetBlock(ChunkData chunk, Vector3Int position, BlockType block)
    {
        if (!IsInRange(position.x, chunk.ChunkSize) || !IsInRange(position.z, chunk.ChunkSize) || !IsInRange(position.y, chunk.ChunkHeight))
        {
            return;
        }

        int index = GetIndexFromBlockPosition(position: position, width: chunk.ChunkSize, length: chunk.ChunkSize);
        chunk.Blocks[index] = block;
    }

    public static BlockType GetBlock(ChunkData chunk, Vector3Int position)
    {
        if (IsInRange(position.x, chunk.ChunkSize) && IsInRange(position.z, chunk.ChunkSize) && IsInRange(position.y, chunk.ChunkHeight))
        {
            int index = GetIndexFromBlockPosition(position: position, width: chunk.ChunkSize, length: chunk.ChunkSize);

            return chunk.Blocks[index];
        }

        var worldBlockPosition = GetWorldBlockPosition(chunk, chunkPosition: position);
        var neighbourChunkPosition = GetChunkPositionFromWorldBlockPosition(worldBlockPosition);

        if (chunk.WorldReference.TryGetChunkData(neighbourChunkPosition, out ChunkData neighbourChunk))
        {
            var localBlockPosition = GetLocalBlockPosition(neighbourChunk, worldBlockPosition);
            int index = GetIndexFromBlockPosition(position: localBlockPosition, width: neighbourChunk.ChunkSize, length: neighbourChunk.ChunkSize);

            return neighbourChunk.Blocks[index];
        }

        return BlockType.Nothing;
    }

    public static List<Vector3Int> GetNeighbouringChunkPositions(ChunkData chunk, Vector3Int blockLocalPosition)
    {
        var neighbours = new List<Vector3Int>();

        if (blockLocalPosition.x == 0) neighbours.Add(Vector3Int.left);
        if (blockLocalPosition.x == WorldData.ChunkSize - 1) neighbours.Add(Vector3Int.right);  
        if (blockLocalPosition.z == 0) neighbours.Add(Vector3Int.back);
        if (blockLocalPosition.z == WorldData.ChunkSize - 1) neighbours.Add(Vector3Int.forward);

        for(int i = 0; i < neighbours.Count; i++) 
        {
            neighbours[i] = chunk.WorldPosition + neighbours[i] * WorldData.ChunkSize;
        }

        return neighbours;
    }

    public static Vector3Int GetBlockPositionFromSurfacePoint(Vector3 surfacePoint, Vector3 normal)
    {
        if (normal.x == 1) surfacePoint.x -= 0.5f;
        if (normal.x == -1) surfacePoint.x += 0.5f;

        if (normal.y == 1 ) surfacePoint.y -= 0.5f;
        if (normal.y == -1) surfacePoint.y += 0.5f;

        if (normal.z == 1) surfacePoint.z -= 0.5f;
        if (normal.z == -1) surfacePoint.z += 0.5f;

        return  Vector3Int.RoundToInt(surfacePoint);
    }

    public static MeshData GetChunkMeshData(ChunkData chunk) 
    {
        var meshData = new MeshData();

        LoopThoughBlocks(chunk, action: (blockPosition) => BlockHelper.GetMeshData(chunk, blockPosition, meshData));
        
        return meshData;
    }

    public static Vector3Int GetWorldBlockPosition(ChunkData chunk, Vector3Int chunkPosition)
    {
        return new Vector3Int()
        {
            x = chunkPosition.x + chunk.WorldPosition.x, 
            y = chunkPosition.y + chunk.WorldPosition.y, 
            z = chunkPosition.z + chunk.WorldPosition.z   
        };
    }

    public static Vector3Int GetLocalBlockPosition(ChunkData chunk, Vector3Int worldPosition)
    {
        return new Vector3Int()
        {
            x = worldPosition.x - chunk.WorldPosition.x, 
            y = worldPosition.y - chunk.WorldPosition.y, 
            z = worldPosition.z - chunk.WorldPosition.z   
        };
    }

    private static void LoopThoughBlocks(ChunkData chunk, Action<Vector3Int> action)
    {
        for (int i = 0; i < chunk.Blocks.Length; i++)
        {
            Vector3Int pos = GetBlockPositionFromIndex(index: i, width: chunk.ChunkSize, length: chunk.ChunkHeight);
            action(pos);
        }
    }

    public static Vector3Int GetBlockPositionFromIndex(int index, int width, int length)
    {
        int x = index % width;
        int z = index / (width * length);
        int y = (index / width) % length;

        return new Vector3Int(x, y, z);
    }

    public static Vector3Int GetChunkPositionFromWorldBlockPosition(Vector3Int worldBlockPosition)
    {
        return new ()
        {
            x = Mathf.FloorToInt(worldBlockPosition.x / (float)WorldData.ChunkSize) * WorldData.ChunkSize,
            y = Mathf.FloorToInt(worldBlockPosition.y / (float)WorldData.ChunkHeight) * WorldData.ChunkHeight,
            z = Mathf.FloorToInt(worldBlockPosition.z / (float)WorldData.ChunkSize) * WorldData.ChunkSize
        };
    }

    public static int GetIndexFromBlockPosition(Vector3Int position, int width, int length)
    {
        return position.x + (position.z * length) + (position.y * length * width);
    }

    private static bool IsInRange(int coordinat, int maxCoordinate)
    {
        return coordinat >= 0 && coordinat < maxCoordinate;
    }

    public static bool IsBlockOnChunkEdge(Vector3Int blockLocalCoordinats)
    {
        return blockLocalCoordinats.x == 0 || 
                blockLocalCoordinats.x == WorldData.ChunkSize - 1 || 
                blockLocalCoordinats.z == 0 || 
                blockLocalCoordinats.z == WorldData.ChunkSize - 1;
    }

}
