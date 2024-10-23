using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController _characterController;
    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _runningSpeed = 15;
    [SerializeField] private float _flightSpeed = 15;
    [SerializeField] private float _jumpHeight = 8;

    private bool _isFlightEnabled;

    private float _playerVerticalVelocity;

    private const float GRAVITY_FORCE = -9.81f;
    private const float MAX_VERTICAL_VELOCITY = 10;
    private const float MIN_VERTICAL_VELOCITY = GRAVITY_FORCE * 2;

    // Ground check
    [SerializeField] private LayerMask _groundMask;

    private float _rayDistance = 0.1f;

    public bool IsGrounded { get; private set; }

    private void Start()
    {
        _playerInput.OnToggleFly += () => _isFlightEnabled = !_isFlightEnabled;
    }

    private void Update()
    {
        _performGroundCheck();

        _move(_transformIntoLocalSpace(_playerInput.PlayerMovementInput.normalized), _playerInput.IsRunning);

        if (_isFlightEnabled) 
        {
            _fly(_playerInput.IsJumping, _playerInput.IsRunning);
        }
        else
        {
            _resetVerticalVelocity();
            _handleJump(_playerInput.IsJumping);
            _handleGravity();
        }
    }
    
    private Vector3 _transformIntoLocalSpace(Vector3 vector)
    {
        return transform.right * vector.x + transform.forward * vector.z;
    }

    private void _move(Vector3 directionVector, bool isRunning)
    {
        var speed = _isFlightEnabled ? _flightSpeed : isRunning ? _runningSpeed : _movementSpeed;

        _characterController.Move(directionVector * (speed * Time.deltaTime));
    }

    private void _fly(bool isGoingUp, bool isGoingDown)
    {
        Vector3 direction = Vector3.zero;

        if (isGoingDown) direction += Vector3.down;
        if (isGoingUp) direction += Vector3.up;

        _characterController.Move(direction * (_flightSpeed * Time.deltaTime));
    }

    private void _handleJump(bool isJumping)
    {
        if (isJumping && IsGrounded)
            _playerVerticalVelocity = _jumpHeight;
    }

    private void _handleGravity()
    {
        if (_playerVerticalVelocity > 0)
        {
            _playerVerticalVelocity += GRAVITY_FORCE * Time.deltaTime;
        }
        else
        {
            _playerVerticalVelocity += GRAVITY_FORCE * 2 * Time.deltaTime;
        }
          
        _playerVerticalVelocity = Mathf.Clamp(_playerVerticalVelocity, MIN_VERTICAL_VELOCITY, MAX_VERTICAL_VELOCITY);

        _characterController.Move(new Vector3(0, _playerVerticalVelocity, 0) * Time.deltaTime);
    }

    private void _resetVerticalVelocity()
    {
        if (IsGrounded && _playerVerticalVelocity < 0)
        {
            _playerVerticalVelocity = 0f;
        }
    }

    private void _performGroundCheck()
    {
        IsGrounded = Physics.Raycast(_characterController.transform.position, Vector3.down, _rayDistance, _groundMask);   
    }
}
