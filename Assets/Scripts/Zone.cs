using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Zone : MonoBehaviour
{
    public enum ZoneType { Tunnel, ChamberCell }

    public ZoneType type;
    public bool hasGas;
    public Chamber parentChamber;

    private TorchController _torch;
    private PlayerHealth _playerHealth;
    private PlayerOxygen _playerOxygen;
    private bool _playerInside;

    private void Awake()
    {
        _torch = FindAnyObjectByType<TorchController>();
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        if (hasGas && _playerInside && _playerHealth != null && _torch != null && _torch.IsOn)
            _playerHealth.Die();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;
        _playerHealth = other.GetComponent<PlayerHealth>();
        _playerOxygen = other.GetComponent<PlayerOxygen>();
        ZoneManager.Instance?.OnPlayerEnterZone(this);
        if (hasGas) _playerOxygen?.EnterGasZone();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
        ZoneManager.Instance?.OnPlayerExitZone(this);
        if (hasGas) _playerOxygen?.ExitGasZone();
        _playerHealth = null;
        _playerOxygen = null;
    }

    private void OnDrawGizmos()
    {
        var col = GetComponent<Collider>();
        if (col == null) return;
        Gizmos.color = hasGas ? new Color(1f, 0.2f, 0.2f, 0.5f) : new Color(0.2f, 1f, 0.2f, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        if (col is BoxCollider box)
            Gizmos.DrawWireCube(box.center, box.size);
        else
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
