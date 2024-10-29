using UnityEngine;

public static class DomainWarping
{
    public static float GetNoiseValue(float x, float z, DomainWarpingSettings domainWarpingSettings, NoiseSettings noiseSettings, Vector2Int worldSeedOffset)
    {
        var domainOffset = GetDomainOffset(x, z, domainWarpingSettings, worldSeedOffset);

        return CustomNoise.OctavePerlin(x + domainOffset.x, z + domainOffset.y, noiseSettings, worldSeedOffset);
    }

    public static Vector2 GetDomainOffset(float x, float z, DomainWarpingSettings domainWarpingSettings, Vector2Int worldSeedOffset)
    {
        float xOffset = CustomNoise.OctavePerlin(x, z, domainWarpingSettings.OffsetSettingsX, worldSeedOffset) * domainWarpingSettings.AmplitudeX;
        float yOffset = CustomNoise.OctavePerlin(x, z, domainWarpingSettings.OffsetSettingsY, worldSeedOffset) * domainWarpingSettings.AmplitudeY;

        return new Vector2(xOffset, yOffset);
    }
}