using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class GasPocket : MonoBehaviour
{
    private TorchController _torch;
    private PlayerHealth _playerHealth;
    private PlayerOxygen _playerOxygen;
    private bool _playerInside;

    private void Awake()
    {
        _torch = FindAnyObjectByType<TorchController>();
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Update()
    {
        if (_playerInside && _torch != null && _torch.IsOn)
            _playerHealth.Die();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;
        _playerHealth = other.GetComponent<PlayerHealth>();
        _playerOxygen = other.GetComponent<PlayerOxygen>();
        _playerOxygen?.EnterGasZone();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
        _playerOxygen?.ExitGasZone();
        _playerOxygen = null;
        _playerHealth = null;
    }
}
