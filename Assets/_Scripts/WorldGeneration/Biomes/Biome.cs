using UnityEngine;

public class Biome : MonoBehaviour
{
    [SerializeField] private NoiseSettings _biomeNoiseSettings;
    [SerializeField] private DomainWarping _domainWarping;
    [SerializeField] private TerrainBlockSelector _terrainBlockSelector;
    [SerializeField] private bool UseWarping = false;

    public int GetGroundLevel(int x, int z, float chunkHeight, Vector2Int seedMapOffset)
    {
        float noiseValue;

        if (UseWarping)
        {
            noiseValue = _domainWarping.GetNoiseValue(x, z, _biomeNoiseSettings, seedMapOffset);
        }
        else
        {
            noiseValue = CustomNoise.OctavePerlin(x, z, _biomeNoiseSettings, seedMapOffset);
        }
        
        noiseValue = CustomNoise.NoiseRedistribution(noiseValue, _biomeNoiseSettings);

        return (int)CustomNoise.RemapValueFromNormalRange(noiseValue, 0, chunkHeight);
    }

    public BlockType SelectBlock(BlockGenerationParameters parameters)
    {
        return _terrainBlockSelector.SelectBlock(parameters);
    } 
}