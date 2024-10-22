using UnityEngine;

public class BlockHelper
{
    private static Direction[] _directions = 
    {
        Direction.forward,
        Direction.backward,
        Direction.right,
        Direction.left,
        Direction.up,
        Direction.down
    };

    public static MeshData GetMeshData(ChunkData chunk, Vector3Int blockPosition, MeshData meshData)
    {
        var blockType = chunk.Blocks[Chunk.GetIndexFromBlockPosition(blockPosition, chunk.ChunkSize, chunk.ChunkSize)];
        if (blockType == BlockType.Air || blockType == BlockType.Nothing)
            return meshData;

        foreach (Direction direction in _directions)
        {
            var neighbourBlockCoordinates = blockPosition + direction.GetDirectionVector();
            var neighbourBlockType = Chunk.GetBlock(chunk, neighbourBlockCoordinates);

            if (neighbourBlockType != BlockType.Nothing && BlockDataManager.BlockDataDictionary[neighbourBlockType].isSolid == false)
            {
                if (blockType == BlockType.Water)
                {
                    if (neighbourBlockType == BlockType.Air)
                    {
                        meshData.WaterMesh = PushTextureVertices(direction, blockPosition, meshData.WaterMesh, blockType);
                    }  
                }
                else
                {
                    meshData.MainMesh = PushTextureVertices(direction, blockPosition, meshData.MainMesh, blockType);
                }
            }
        }

        return meshData;
    }

    public static InternalMeshData PushTextureVertices(Direction direction, Vector3Int position, InternalMeshData meshData, BlockType blockType)
    {
        bool isGeneratingCollider = BlockDataManager.BlockDataDictionary[blockType].generatesCollider;

        foreach (var vertex in BlockDataManager.GetTextureVertices(direction, position.x, position.y, position.z))
        {
            meshData.AddVertex(vertex, isGeneratingCollider);
        }

        meshData.AddQuadTriangles(isGeneratingCollider);
        meshData.UV.AddRange(GetTextureUVs(blockType, direction));

        return meshData;
    }

    private static Vector2[] GetTextureUVs(BlockType blockType, Direction direction)
    {
        Vector2Int texturePositionIndecies = GetTexturePositionByDirection(blockType, direction);

        return BlockDataManager.GetTextureUvsByPositionIndecies(texturePositionIndecies);
    } 

    private static Vector2Int GetTexturePositionByDirection(BlockType blockType, Direction direction)
    {
        return direction switch 
        {
            Direction.up => BlockDataManager.BlockDataDictionary[blockType].up,
            Direction.down => BlockDataManager.BlockDataDictionary[blockType].down,
            _ => BlockDataManager.BlockDataDictionary[blockType].side,
        };
    } 
}
