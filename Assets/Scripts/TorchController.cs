using UnityEngine;
using UnityEngine.InputSystem;

public class TorchController : MonoBehaviour
{
    [SerializeField] private Light torchLight;

    private bool _on = true;
    public bool IsOn => _on;

    private void Start()
    {
        if (torchLight != null)
            torchLight.enabled = _on;
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
            Toggle();
    }

    public void Toggle()
    {
        _on = !_on;
        if (torchLight != null)
            torchLight.enabled = _on;
    }
}
