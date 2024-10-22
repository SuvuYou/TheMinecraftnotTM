using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _playerCameraHolder;
    [SerializeField] private Transform _playerCharacter;

    private int _sensitivity = 150;
    private float _verticalRotation = 0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        var mouseMovementX = _playerInput.MouseMovementInput.x * _sensitivity * Time.deltaTime;
        var mouseMovementY = _playerInput.MouseMovementInput.y * _sensitivity * Time.deltaTime;

        _setHorizontalRotation(mouseMovementX);
        _setVerticalRotation(mouseMovementY);
    }

    private void _setVerticalRotation(float mouseMovementY)
    {
        _verticalRotation -= mouseMovementY;
        _verticalRotation = Math.Clamp(_verticalRotation, -90, 90);   

        _playerCameraHolder.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }

    private void _setHorizontalRotation(float mouseMovementX)
    {
        _playerCharacter.Rotate(Vector3.up * mouseMovementX);
    }
}
