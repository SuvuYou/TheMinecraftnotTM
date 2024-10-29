using UnityEngine;

public class Biome : MonoBehaviour
{
    [SerializeField] private NoiseSettings _biomeNoiseSettings;
    [SerializeField] private DomainWarpingSettings _domainWarpingSettings;
    [SerializeField] private TerrainBlockSelector _terrainBlockSelector;

    [SerializeField] private TreeGenerator _treeGenerator;
    [SerializeField] private StonePatches _stonePatches;

    public void GenerateChunkYColumn(ChunkData chunk, int x, int z, int groundLevel, Vector2Int seedMapOffset)
    {
        var stonePatchesProbability = _getStonePatchesProbability(chunk.WorldPosition.x + x, chunk.WorldPosition.z + z, seedMapOffset);

        for (int y = 0; y < chunk.ChunkHeight; y++)
        {
            var blockPosition = new Vector3Int(x, y, z);

            var block = _terrainBlockSelector.SelectBlock(new BlockGenerationParameters(chunk, blockPosition, groundLevel, stonePatchesProbability));
                
            Chunk.SetBlock(chunk, blockPosition, block);
        }
     
    }

    public void SetTreeData(ChunkData chunk, Vector2Int seedMapOffset)
    {
        if (_treeGenerator == null)
        {
            chunk.Trees.TreePositions = new ();

            return;
        }

        chunk.Trees.TreePositions = _treeGenerator.GenerateTrees(chunk, seedMapOffset);
    }

    private float _getStonePatchesProbability(int x, int z, Vector2Int seedMapOffset)
    {
        if (_stonePatches == null) return 0;

        return _stonePatches.GetProbability(x, z, seedMapOffset);
    }

    public int GetGroundLevel(int x, int z, float chunkHeight, Vector2Int seedMapOffset)
    {
        float noiseValue;

        noiseValue = DomainWarping.GetNoiseValue(x, z, _domainWarpingSettings, _biomeNoiseSettings, seedMapOffset);

        noiseValue = CustomNoise.NoiseRedistribution(noiseValue, _biomeNoiseSettings);

        return (int)CustomNoise.RemapValueFromNormalRange(noiseValue, 0, chunkHeight);
    }
}