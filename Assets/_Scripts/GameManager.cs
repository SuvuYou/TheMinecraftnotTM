
using System.Collections;
using Cinemachine;
using UnityEngine;

class GameManager : MonoBehaviour
{
    [SerializeField] private World _world;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private PlayerCharacter _playerPrefab;

    private PlayerCharacter _activePlayerReference;
    private ChunkData _currentChunk;

    private const float CHUNK_LOADING_CYCLE_TIMEOUT = 1;

    private void Start()
    {
        _world.OnWorldGenerated += _spawnPlayer;
    }

    private void _spawnPlayer()
    {
        if (_activePlayerReference != null) return;

        Vector3Int spawnPositionRaycast = new (WorldData.ChunkSize / 2, 100, WorldData.ChunkSize / 2);
        
        if (Physics.Raycast(spawnPositionRaycast, Vector3.down, out RaycastHit hit, 200f))  
        {
            _activePlayerReference = Instantiate(_playerPrefab, hit.point + Vector3.up, Quaternion.identity);
            _virtualCamera.Follow = _activePlayerReference.CameraHolder.transform;
            _startUpdatingChunksLive();
        }
    }

    private void _startUpdatingChunksLive()
    {
        _findCurrentChunkFromPlayerPosition();
        StopAllCoroutines();
        StartCoroutine(_checkShouldUpdateChunks());
    }

    private IEnumerator _checkShouldUpdateChunks()
    {
        while (true)
        {
            if (!_isPlayerInCurrentChunk())
            {
                _world.UpdateLoadChunks(Vector3Int.RoundToInt(_activePlayerReference.transform.position));
                _findCurrentChunkFromPlayerPosition();
            }
            
            yield return new WaitForSeconds(CHUNK_LOADING_CYCLE_TIMEOUT);
        }
    }

    private bool _isPlayerInCurrentChunk()
    {
        if (Mathf.Abs(_currentChunk.ChunkCenterPosition.x - _activePlayerReference.transform.position.x) > (_currentChunk.ChunkSize / 2))
        {
            return false;
        }

        if (Mathf.Abs(_currentChunk.ChunkCenterPosition.z - _activePlayerReference.transform.position.z) > (_currentChunk.ChunkSize / 2))
        {
            return false;
        }

        return true;    
    }

    private void _findCurrentChunkFromPlayerPosition()
    {
        var currentChunkPosition = Chunk.GetChunkPositionFromWorldBlockPosition(Vector3Int.RoundToInt(_activePlayerReference.transform.position));
        
        if (_world.TryGetChunkData(currentChunkPosition, out ChunkData chunk)) 
        {
            _currentChunk = chunk;
        }
    }
}