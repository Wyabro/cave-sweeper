using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private bool _dead;

    public void Die()
    {
        if (_dead) return;
        _dead = true;
        Debug.Log("Player died — reloading scene.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
