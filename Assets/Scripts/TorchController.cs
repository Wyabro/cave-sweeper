using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TorchController : MonoBehaviour
{
    [SerializeField] private Light torchLight;
    [SerializeField] private float _swingDuration = 0.3f;

    private bool _on = true;
    private bool _swinging;

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

        if (Mouse.current.leftButton.wasPressedThisFrame && !_swinging)
            StartCoroutine(SwingCoroutine());
    }

    public void Toggle()
    {
        _on = !_on;
        if (torchLight != null)
            torchLight.enabled = _on;
    }

    private IEnumerator SwingCoroutine()
    {
        _swinging = true;
        var tf = torchLight.transform;
        Vector3 startPos = tf.localPosition;
        Vector3 peakPos  = startPos + new Vector3(0f, -0.15f, 0.4f);
        float half = _swingDuration * 0.5f;

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            tf.localPosition = Vector3.Lerp(startPos, peakPos, t / half);
            yield return null;
        }
        tf.localPosition = peakPos;
        TorchMeleeHit();

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            tf.localPosition = Vector3.Lerp(peakPos, startPos, t / half);
            yield return null;
        }
        tf.localPosition = startPos;
        _swinging = false;
    }

    public void TorchMeleeHit() { }
}
