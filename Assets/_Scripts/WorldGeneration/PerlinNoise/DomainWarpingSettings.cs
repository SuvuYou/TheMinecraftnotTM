using UnityEngine;

[CreateAssetMenu(fileName = "DomainWarpingSettings", menuName = "Noise/DomainWarpingSettings")]
public class DomainWarpingSettings : ScriptableObject
{
    public NoiseSettings OffsetSettingsX, OffsetSettingsY;
    public float AmplitudeX, AmplitudeY;
}