using UnityEngine;

public class DomainWarping : MonoBehaviour
{
    [SerializeField] private NoiseSettings _offsetSettingsX, _offsetSettingsY;
    [SerializeField] private float _amplitudeX, _amplitudeY;

    public float GetNoiseValue(float x, float z, NoiseSettings noiseSettings, Vector2Int worldSeedOffset)
    {
        var domainOffset = GetDomainOffset(x, z, worldSeedOffset);

        return CustomNoise.OctavePerlin(x + domainOffset.x, z + domainOffset.y, noiseSettings, worldSeedOffset);
    }

    private Vector2 GetDomainOffset(float x, float z, Vector2Int worldSeedOffset)
    {
        float xOffset = CustomNoise.OctavePerlin(x, z, _offsetSettingsX, worldSeedOffset) * _amplitudeX;
        float yOffset = CustomNoise.OctavePerlin(x, z, _offsetSettingsY, worldSeedOffset) * _amplitudeY;

        return new Vector2(xOffset, yOffset);
    }

}