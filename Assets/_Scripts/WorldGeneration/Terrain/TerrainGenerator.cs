using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int _seedMapOffset;
    [SerializeField] private Biome _biome;
    [SerializeField] private BiomeOverride _biomeOverride;

    public void GenerateVoxels(ChunkData chunk)
    {
        for (int x = 0; x < chunk.ChunkSize; x++)
        {
            for (int z = 0; z < chunk.ChunkSize; z++)
            {
                var groundLevel = _biome.GetGroundLevel(chunk.WorldPosition.x + x, chunk.WorldPosition.z + z, chunk.ChunkHeight, _seedMapOffset);

                var stonePatchesProbability = _biomeOverride.GetOverrideProbability(chunk.WorldPosition.x + x, chunk.WorldPosition.z + z, _seedMapOffset);
     
                for (int y = 0; y < chunk.ChunkHeight; y++)
                {
                    var blockPosition = new Vector3Int(x, y, z);

                    var block = _biome.SelectBlock(new BlockGenerationParameters(blockPosition, groundLevel));
                    var overrideBlock = _biomeOverride.SelectOverrideBlock(new BlockGenerationParameters(blockPosition, groundLevel, stonePatchesProbability));

                    if (overrideBlock != BlockType.Nothing)
                        block = overrideBlock;
                        
                    Chunk.SetBlock(chunk, blockPosition, block);
                }
            }
        }
    }
}
