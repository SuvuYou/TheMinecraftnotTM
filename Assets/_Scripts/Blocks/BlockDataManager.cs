using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static float tileSizeX = 0.1f, tileSizeY = 0.1f;

    [SerializeField] private BlockDataSO _blockDataSO;
    public static Dictionary<BlockType, TextureData> BlockDataDictionary = new ();

    private void Awake()
    {
        foreach(var item in _blockDataSO.textureDataList)
        {
            if (!BlockDataDictionary.ContainsKey(item.blockType))
            {
                BlockDataDictionary.Add(item.blockType, item);
            }
        }
    }

    public static Vector2[] GetTextureUvsByPositionIndecies(Vector2Int positionIndecies)
    {
        Vector2[] Uvs = new Vector2[4];
        
        Uvs[0] = new Vector2((positionIndecies.x + 1) * tileSizeX, positionIndecies.y * tileSizeY);
        Uvs[1] = new Vector2((positionIndecies.x + 1) * tileSizeX, (positionIndecies.y + 1) * tileSizeY);
        Uvs[2] = new Vector2(positionIndecies.x * tileSizeX, (positionIndecies.y + 1) * tileSizeY);
        Uvs[3] = new Vector2(positionIndecies.x * tileSizeX, positionIndecies.y * tileSizeY);
        
        Uvs[0] = new Vector2(Uvs[0].x - textureOffset, Uvs[0].y + textureOffset);
        Uvs[1] = new Vector2(Uvs[1].x - textureOffset, Uvs[1].y - textureOffset);
        Uvs[2] = new Vector2(Uvs[2].x + textureOffset, Uvs[2].y - textureOffset);
        Uvs[3] = new Vector2(Uvs[3].x + textureOffset, Uvs[3].y + textureOffset);
        
        return Uvs;
    }

    public static List<Vector3> GetTextureVertices(Direction direction, int x, int y, int z)
    {
        List<Vector3> vertices = new ();

        //order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.backward:
                vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                break;
            case Direction.forward:
                vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                break;
            case Direction.left:
                vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                break;
            case Direction.right:
                vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                break;
            case Direction.down:
                vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                break;
            case Direction.up:
                vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                break;
            default:
                break;
        }

        return vertices;
    }
}
