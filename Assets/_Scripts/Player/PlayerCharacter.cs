using Cinemachine;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [field: SerializeField] public GameObject CameraHolder { get; private set; } 

    private Camera _mainCamera;

    [SerializeField] private PlayerInput _playerInput;
    // Ground check
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private World _world;

    private float _rayDistance = 5f;

    private void Start()
    {
        _world = FindObjectOfType<World>();
        _playerInput.OnMouseClick += _handleMouseClick;
        _mainCamera = Camera.main;
    }

    private void _handleMouseClick()
    {
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, _rayDistance, _groundMask))
        {
            if (hit.collider.GetComponent<ChunkRenderer>())
            {
                var blockWorldPosition = Chunk.GetBlockPositionFromSurfacePoint(hit.point, hit.normal);

                _world.ModifyBlock(blockWorldPosition, BlockType.Air);
            }
        }
    }   
}
