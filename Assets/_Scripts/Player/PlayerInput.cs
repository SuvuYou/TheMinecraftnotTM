using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action OnMouseClick, OnToggleFly;

    public bool IsRunning { get; private set; }

    public bool IsJumping { get; private set; }

    public Vector3 PlayerMovementInput { get; private set; }

    public Vector2 MouseMovementInput { get; private set; }

    private void Update()
    {
        _updateIsRunning();
        _updatePlayerMovement();
        _updateMouseMovement();
        _updateIsJunping();

        _tryTriggerMouseClickEvent();
        _tryTriggerFlyEvent();
    }

    private void _updateIsRunning () => IsRunning = Input.GetKey(KeyCode.LeftShift);
    private void _updateIsJunping () => IsJumping = Input.GetKey(KeyCode.Space);
    private void _updatePlayerMovement () => PlayerMovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    private void _updateMouseMovement () => MouseMovementInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

    private void _tryTriggerMouseClickEvent() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseClick?.Invoke();
        }
    }

    private void _tryTriggerFlyEvent() 
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            OnToggleFly?.Invoke();
        }
    }
}
