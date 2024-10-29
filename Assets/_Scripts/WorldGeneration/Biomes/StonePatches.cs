using UnityEngine;

public class StonePatches : MonoBehaviour
{
    [SerializeField] private NoiseSettings _biomeOverrideNoiseSettings;
    [SerializeField] private DomainWarpingSettings _domainWarpingSettings;

    public float GetProbability(int x, int z, Vector2Int seedMapOffset)
    {
        if (_biomeOverrideNoiseSettings == null)
            return 0f;
            
        var noiseValue = DomainWarping.GetNoiseValue(x, z, _domainWarpingSettings, _biomeOverrideNoiseSettings, seedMapOffset);
        var probability = CustomNoise.NoiseRedistribution(noiseValue, _biomeOverrideNoiseSettings);

        return probability;
    }
}
