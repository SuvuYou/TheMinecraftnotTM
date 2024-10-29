using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private NoiseSettings _treeGeneratorNoiseSettings;
    [SerializeField] private DomainWarpingSettings _domainWarpingSettings;

    public List<Vector2Int> GenerateTrees(ChunkData chunk, Vector2Int seedMapOffset)
    {
        return TreesNoise.GetTreeData(chunk, _domainWarpingSettings, _treeGeneratorNoiseSettings, seedMapOffset);
    }  
}
