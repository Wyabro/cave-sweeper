using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 0.15f;

    private float _pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();

        _pitch = Mathf.Clamp(_pitch - delta.y * sensitivity, -80f, 80f);
        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        playerBody.Rotate(Vector3.up * (delta.x * sensitivity));
    }
}
