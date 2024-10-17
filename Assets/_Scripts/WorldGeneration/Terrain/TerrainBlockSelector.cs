using System.Collections.Generic;
using UnityEngine;

public class TerrainBlockSelector : MonoBehaviour
{
    [SerializeField] private List<TerrainLevel> _terrainLevels = new();
    [SerializeField] private List<BlockType> _terrainBlocks = new();
    // [SerializeField] private Dictionary<TerrainLevel, BlockType> TerrainLevelBlocks;
    private Dictionary<TerrainLevel, ITerrainLevelFilter> _terrainLevelFilters = new();

    private void Start()
    {
        InitAllFilters();
    }

    public BlockType SelectBlock(BlockGenerationParameters parameters)   
    {
        var block = BlockType.Nothing;

        for (int i = 0; i < _terrainLevels.Count; i++)
        {
            var terrainLevel = _terrainLevels[i];
            var filter = _terrainLevelFilters[terrainLevel];

            if (filter.IsBlockInTerrainLevel(parameters))
            {
                block = _terrainBlocks[i];

                break;
            }
        }

        return block;
    }

    private void InitAllFilters()
    {
        foreach (var level in _terrainLevels)
        {
            _terrainLevelFilters.Add(level, level.GetTerrainLevelFilter());
        }
    }
}
