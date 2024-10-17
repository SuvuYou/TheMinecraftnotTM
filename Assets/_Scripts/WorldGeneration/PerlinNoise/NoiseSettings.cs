using UnityEngine;

[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Noise/NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
    public float NoiseZoom;
    public int Octaves;
    public Vector2Int Offest;
    public float Persistance;
    public float RedistributionModifier;
    public float Exponent;
}