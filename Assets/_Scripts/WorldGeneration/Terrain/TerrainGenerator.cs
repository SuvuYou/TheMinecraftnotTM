using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int _seedMapOffset;
    [SerializeField] private StonePatches _stonePatches;
    [SerializeField] private TreeGenerator _treeGenerator;

    [SerializeField] private NoiseSettings _biomeNoiseSettings;
    [SerializeField] private DomainWarpingSettings _biomeDomainWarpingSettings;

    [SerializeField] private List<BiomeData> _biomeGeneratorsData;

    private Dictionary<Vector3Int, float> _biomeTemperatures = new();

    public void GenerateVoxels(ChunkData chunk)
    {
        var chunkBiomes = _getBiomeGeneratorsByPosition(chunk.WorldPosition);
        chunkBiomes[0].BiomeGenerator.SetTreeData(chunk, _seedMapOffset);

        for (int x = 0; x < chunk.ChunkSize; x++)
        {
            for (int z = 0; z < chunk.ChunkSize; z++)
            {
                var blockBiomes = _getBiomeGeneratorsByPosition(new Vector3Int(chunk.WorldPosition.x + x, 0, chunk.WorldPosition.z + z));  
                var groundLevel = _getGroundLevelWithInterpolation(blockBiomes.Take(12).ToList(), chunk.WorldPosition.x + x, chunk.WorldPosition.z + z, chunk.ChunkHeight);
                
                var closestBiome = blockBiomes[0].BiomeGenerator;
                closestBiome.GenerateChunkYColumn(chunk, x, z, groundLevel, _seedMapOffset);
            }   
        }
    }

    public void GenerateBiomesTemperatures(Vector3Int centerPosition)
    {
        _biomeTemperatures.Clear();
        var biomePositions = BiomesManager.GetBiomePositions(centerPosition);

        for (int i = 0; i < biomePositions.Count; i++)
        {
            var offset = DomainWarping.GetDomainOffset(biomePositions[i].x, biomePositions[i].z, _biomeDomainWarpingSettings, _seedMapOffset);
            var offsetInt = Vector2Int.RoundToInt(offset);
            biomePositions[i] += new Vector3Int(offsetInt.x, 0, offsetInt.y);

            var temp = CustomNoise.OctavePerlin(biomePositions[i].x, biomePositions[i].z, _biomeNoiseSettings, _seedMapOffset);
            _biomeTemperatures.Add(biomePositions[i], temp);
        }
    }

    private int _getGroundLevelWithInterpolation(List<ChunkBiomeData> biomes, int x, int z, int chunkHeight)
    {
        float groundLevel = 0;
        
        var invertedDistances = biomes.Select(biome => biome.DistanceToBiomeCenter == 0 ? 1f : 1f / biome.DistanceToBiomeCenter).ToList();
        float cumulativeInvertedDistance = invertedDistances.Sum();

        for (int i = 0; i < biomes.Count; i++)
        {
            var biome = biomes[i];
            var localGroundLevel = biome.BiomeGenerator.GetGroundLevel(x, z, chunkHeight, _seedMapOffset);
            
            float weight = invertedDistances[i] / cumulativeInvertedDistance;
            groundLevel += localGroundLevel * weight;
        }
        
        return (int)groundLevel;
    }

    private List<ChunkBiomeData> _getBiomeGeneratorsByPosition(Vector3Int position)
    {
        var biomes = _biomeTemperatures.Keys
            .Select(biomePos => 
                new ChunkBiomeData(
                        biomeGenerator: _selectBiomeByTemperature(_biomeTemperatures[biomePos]), 
                        distanceToBiomeCenter: Vector3.Distance(position, biomePos)
                    ))
            .OrderBy(biomeData => biomeData.DistanceToBiomeCenter)
            .ToList();

        return biomes;
    }

    private Biome _selectBiomeByTemperature(float temperature)
    {
        var biome = _biomeGeneratorsData.Find(data => temperature >= data.LowerTemperatureBound && temperature < data.TopTemperatureBound);
        
        return biome.BiomeGenerator;
    }
}

[System.Serializable]
public struct BiomeData
{
    [Range(0f, 1f)]
    public float LowerTemperatureBound, TopTemperatureBound;
    public Biome BiomeGenerator;
}

public struct ChunkBiomeData
{
    public Biome BiomeGenerator;
    public float DistanceToBiomeCenter;

    public ChunkBiomeData(Biome biomeGenerator, float distanceToBiomeCenter)
    {
        BiomeGenerator = biomeGenerator;
        DistanceToBiomeCenter = distanceToBiomeCenter;
    }
}