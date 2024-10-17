using UnityEngine;

public static class CustomNoise
{
    public static float RemapValues(float value, float initLower, float initUpper, float targetLower, float targetUpper)
    {
        value = (value - initLower) / initUpper; // (0, 1)

        value *= targetUpper - targetLower; // (0, targetUpper - targetLower)

        value += targetLower; // (targetLower, targetUpper)

        return value;
    }

    public static float RemapValueFromNormalRange(float value, float targetLower, float targetUpper)
    {
        return RemapValues(value, 0, 1, targetLower, targetUpper);
    }

    public static float NoiseRedistribution(float noiseValue, NoiseSettings settings)
    {
        return Mathf.Pow(noiseValue * settings.RedistributionModifier, settings.Exponent);
    }

    public static float OctavePerlin(float x, float z, NoiseSettings settings, Vector2Int worldSeedOffset)
    {
        x *= settings.NoiseZoom;
        z *= settings.NoiseZoom;
        x += settings.NoiseZoom;
        z += settings.NoiseZoom;

        float totalNoise = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0;  // Used for normalizing

        for (int i = 0; i < settings.Octaves; i++)
        {
            var noiseX = (settings.Offest.x + worldSeedOffset.x + x) * frequency;
            var noiseY = (settings.Offest.y + worldSeedOffset.y + z) * frequency;
            
            var noiseValue = Mathf.PerlinNoise(noiseX, noiseY) * amplitude;

            totalNoise += noiseValue;
            amplitudeSum += amplitude;
            amplitude *= settings.Persistance;
            frequency *= 2; 
        }

        return totalNoise / amplitudeSum;
    }
}