using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    private readonly HashSet<Zone> _playerZones = new HashSet<Zone>();
    private readonly Dictionary<Zone, List<Zone>> _adjacency = new Dictionary<Zone, List<Zone>>();
    private bool _adjacencyReady;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Invoke(nameof(DelayedBuildAdjacency), 0f);
    }

    private void DelayedBuildAdjacency()
    {
        BuildAdjacency();
        _adjacencyReady = true;
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

    public List<Zone> GetAdjacentZones(Zone zone)
    {
        if (!_adjacencyReady) return new List<Zone>();
        List<Zone> result;
        if (_adjacency.TryGetValue(zone, out result))
            return result;
        return new List<Zone>();
    }

    private void BuildAdjacency()
    {
        Zone[] allZones = FindObjectsByType<Zone>();
        foreach (Zone z in allZones)
            _adjacency[z] = new List<Zone>();

        for (int i = 0; i < allZones.Length; i++)
        {
            for (int j = i + 1; j < allZones.Length; j++)
            {
                Collider colA = allZones[i].GetComponent<Collider>();
                Collider colB = allZones[j].GetComponent<Collider>();
                if (colA == null || colB == null) continue;

                Bounds a = colA.bounds;
                a.Expand(0.3f);
                if (a.Intersects(colB.bounds))
                {
                    _adjacency[allZones[i]].Add(allZones[j]);
                    _adjacency[allZones[j]].Add(allZones[i]);
                }
            }
        }
    }
}
