using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Mesh _mesh;

    private ChunkData _chunkData;
    private bool _shouldShowGizmos;

    public bool IsModifiedByPlayer 
    {
        get => _chunkData.IsModifiedByPlayer;
        set => _chunkData.IsModifiedByPlayer = value;
    }

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = _meshFilter.mesh;
    }

    public void InitChunk(ChunkData chunkData)
    {
        _chunkData = chunkData;
    }

    public void UpdateChunk()
    {
        _renderMesh(Chunk.GetChunkMeshData(_chunkData));
    }

    public void UpdateChunk(MeshData meshData)
    {
        _renderMesh(meshData);
    }

    private void _renderMesh(MeshData meshData)
    {
        _mesh.Clear();

        _mesh.subMeshCount = 2;

        _mesh.vertices = meshData.MainMesh.Vertices.Concat(meshData.WaterMesh.Vertices).ToArray();
        _mesh.SetTriangles(meshData.MainMesh.Triangles.ToArray(), 0); 
        _mesh.SetTriangles(meshData.WaterMesh.Triangles.Select(ver => ver + meshData.MainMesh.Vertices.Count).ToArray(), 1); 

        _mesh.uv = meshData.MainMesh.UV.Concat(meshData.WaterMesh.UV).ToArray();
 
        _mesh.RecalculateNormals();

        _meshCollider.sharedMesh = null;

        Mesh colliderMesh = new () 
        {
            vertices = meshData.MainMesh.ColliderVertices.ToArray(), 
            triangles = meshData.MainMesh.ColliderTriangles.ToArray()
        };

        colliderMesh.RecalculateNormals();

        _meshCollider.sharedMesh = colliderMesh;

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_shouldShowGizmos)
        {
            if (Application.isPlaying && _chunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(transform.position + new Vector3(_chunkData.ChunkSize / 2f, _chunkData.ChunkHeight / 2f, _chunkData.ChunkSize / 2f), new Vector3(_chunkData.ChunkSize, _chunkData.ChunkHeight, _chunkData.ChunkSize));
            }
        }
    }
#endif
}
