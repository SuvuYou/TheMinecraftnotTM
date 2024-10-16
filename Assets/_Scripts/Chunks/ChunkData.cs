using UnityEngine;

public class ChunkData
{
    public BlockType[] Blocks { get; private set; }
    public Vector3Int WorldPosition { get; private set; }
    public int ChunkSize { get; private set; }
    public int ChunkHeight { get; private set; }
    public World WorldReference { get; private set; }

    public bool IsModifiedByPlayer = false;

    public ChunkData(int chunkSize, int chunkHeight, Vector3Int worldPosition, World worldReference)
    {
        ChunkSize = chunkSize;
        ChunkHeight = chunkHeight;
        WorldPosition = worldPosition;
        WorldReference = worldReference;
        Blocks = new BlockType[ChunkSize * ChunkSize * ChunkHeight];
    }
}
