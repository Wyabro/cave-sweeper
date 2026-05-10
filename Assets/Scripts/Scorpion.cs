using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Scorpion : MonoBehaviour
{
    private enum State { Idle, Fleeing, Waiting, Creeping }

    private NavMeshAgent _agent;
    private TorchController _torch;
    private Transform _player;
    private State _state = State.Idle;
    private bool _lastTorchOn;
    private readonly HashSet<Zone> _occupiedZones = new HashSet<Zone>();

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _torch = FindAnyObjectByType<TorchController>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        Invoke(nameof(DelayedInit), 0.1f);
    }

    private void DelayedInit()
    {
        DetectInitialZones();
        _lastTorchOn = _torch.IsOn;
        if (_torch.IsOn) Flee();
    }

    private void Update()
    {
        bool torchOn = _torch.IsOn;
        if (torchOn != _lastTorchOn)
        {
            _lastTorchOn = torchOn;
            StopAllCoroutines();
            if (torchOn)
                Flee();
            else
                StartCoroutine(CreepAfterDelay());
        }

        if ((_state == State.Fleeing || _state == State.Creeping) &&
            !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
            _state = State.Idle;
    }

    private void Flee()
    {
        _state = State.Fleeing;
        Zone current = GetCurrentNonGasZone();
        if (current == null) return;

        List<Zone> adjacent = ZoneManager.Instance.GetAdjacentZones(current);
        List<Zone> safe = new List<Zone>();
        foreach (Zone z in adjacent)
            if (!z.hasGas) safe.Add(z);

        if (safe.Count == 0) return;

        Zone target = safe[Random.Range(0, safe.Count)];
        SetNavDestination(target.transform.position);
    }

    private IEnumerator CreepAfterDelay()
    {
        _state = State.Waiting;
        Vector3 stalePos = _player.position;
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        Vector3 target = stalePos;
        bool found = false;
        for (int i = 0; i < 8; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(1f, 2f);
            Vector3 candidate = stalePos + new Vector3(offset.x, 0f, offset.y);
            if (!IsPositionInGasZone(candidate))
            {
                target = candidate;
                found = true;
                break;
            }
        }

        if (!found && IsPositionInGasZone(stalePos))
        {
            _state = State.Idle;
            yield break;
        }

        _state = State.Creeping;
        SetNavDestination(target);
    }

    private void SetNavDestination(Vector3 worldPos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(worldPos, out hit, 5f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }

    private void DetectInitialZones()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider c in hits)
        {
            Zone z = c.GetComponent<Zone>();
            if (z != null) _occupiedZones.Add(z);
        }
    }

    private Zone GetCurrentNonGasZone()
    {
        _occupiedZones.RemoveWhere(z => z == null);
        foreach (Zone z in _occupiedZones)
            if (!z.hasGas) return z;
        foreach (Zone z in _occupiedZones)
            return z;
        return null;
    }

    private bool IsPositionInGasZone(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, 0.3f);
        foreach (Collider c in hits)
        {
            Zone z = c.GetComponent<Zone>();
            if (z != null && z.hasGas) return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Zone z = other.GetComponent<Zone>();
        if (z == null) return;
        _occupiedZones.Add(z);
        if (z.hasGas && _agent.hasPath)
        {
            _agent.ResetPath();
            _state = State.Idle;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Zone z = other.GetComponent<Zone>();
        if (z != null) _occupiedZones.Remove(z);
    }
}
