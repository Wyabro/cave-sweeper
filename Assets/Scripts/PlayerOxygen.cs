using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField] private float _drainRate = 1f / 20f;
    private float _oxygen = 1f;
    private int _insideCount = 0;
    private bool _dead = false;
    private PlayerHealth _playerHealth;

    public float Oxygen => _oxygen;

    private void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (_dead) return;

        if (_insideCount > 0)
            _oxygen -= _drainRate * Time.deltaTime;
        else
            _oxygen += _drainRate * Time.deltaTime;

        _oxygen = Mathf.Clamp01(_oxygen);

        if (_oxygen <= 0f)
        {
            _dead = true;
            _playerHealth?.Die();
        }
    }

    public void EnterGasZone()
    {
        _insideCount++;
    }

    public void ExitGasZone()
    {
        _insideCount = Mathf.Max(0, _insideCount - 1);
    }
}
