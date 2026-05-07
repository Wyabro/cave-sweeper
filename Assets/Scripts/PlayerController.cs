using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private Transform cameraRig;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _isCrouching;
    private float _standCamY;
    private float _crouchCamY;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _standCamY = cameraRig.localPosition.y;
        _crouchCamY = _standCamY - (standHeight - crouchHeight) * 0.5f;
    }

    private void Update()
    {
        HandleCrouch();
        HandleMove();
        ApplyGravity();
    }

    private void HandleCrouch()
    {
        bool wantCrouch = Keyboard.current.leftCtrlKey.isPressed;
        if (wantCrouch == _isCrouching) return;

        _isCrouching = wantCrouch;
        _cc.height = _isCrouching ? crouchHeight : standHeight;
        _cc.center = Vector3.up * (_cc.height * 0.5f);

        var p = cameraRig.localPosition;
        p.y = _isCrouching ? _crouchCamY : _standCamY;
        cameraRig.localPosition = p;
    }

    private void HandleMove()
    {
        var kb = Keyboard.current;
        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        Vector3 move = transform.right * x + transform.forward * z;
        _cc.Move(move * (((_isCrouching ? crouchSpeed : walkSpeed)) * Time.deltaTime));
    }

    private void ApplyGravity()
    {
        if (_cc.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }
}
