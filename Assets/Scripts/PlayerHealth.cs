using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;
    private bool _dead;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public float HealthNormalized => (float)_currentHealth / _maxHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_dead) return;
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        if (_currentHealth == 0)
            Die();
    }

    public void Die()
    {
        if (_dead) return;
        _dead = true;
        Debug.Log("Player died — reloading scene.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
