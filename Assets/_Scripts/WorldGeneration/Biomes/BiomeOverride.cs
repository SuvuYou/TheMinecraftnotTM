using UnityEngine;

public class BiomeOverride : MonoBehaviour
{
    [SerializeField] private NoiseSettings _biomeOverrideNoiseSettings;
    [SerializeField] private DomainWarping _domainWarping;
    [SerializeField] private TerrainBlockSelector _terrainBlockSelector;

    public float GetOverrideProbability(int x, int z, Vector2Int seedMapOffset)
    {
        if (_biomeOverrideNoiseSettings == null)
            return 0f;
            
        var noiseValue = _domainWarping.GetNoiseValue(x, z, _biomeOverrideNoiseSettings, seedMapOffset);
        var probability = CustomNoise.NoiseRedistribution(noiseValue, _biomeOverrideNoiseSettings);

        return probability;
    }

    public BlockType SelectOverrideBlock(BlockGenerationParameters parameters)
    {
        return _terrainBlockSelector.SelectBlock(parameters);
    }  
}
