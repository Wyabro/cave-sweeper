using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    private readonly HashSet<Zone> _playerZones = new HashSet<Zone>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void OnPlayerEnterZone(Zone zone) => _playerZones.Add(zone);
    public void OnPlayerExitZone(Zone zone) => _playerZones.Remove(zone);

    public IReadOnlyCollection<Zone> GetCurrentZones() => _playerZones;

    public bool IsInGasZone()
    {
        foreach (var z in _playerZones)
            if (z != null && z.hasGas) return true;
        return false;
    }
}
