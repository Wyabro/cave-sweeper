using System.Collections;
using UnityEngine;

public class GasCrossfadeLoop : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float overlapDuration = 4.5f;
    [SerializeField] private float volume = 1f;

    private AudioSource _sourceA;
    private AudioSource _sourceB;

    private void Awake()
    {
        var sources = GetComponents<AudioSource>();
        _sourceA = sources[0];
        _sourceB = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        CopySpatialization(_sourceA, _sourceB);

        _sourceA.loop = false;
        _sourceB.loop = false;
        _sourceA.playOnAwake = false;
        _sourceB.playOnAwake = false;
    }

    private void Start()
    {
        if (clip == null) { Debug.LogWarning("[GasCrossfadeLoop] No clip assigned.", this); return; }
        StartCoroutine(CrossfadeLoop());
    }

    private IEnumerator CrossfadeLoop()
    {
        _sourceA.clip = clip;
        _sourceB.clip = clip;
        _sourceA.volume = volume;
        _sourceB.volume = 0f;
        _sourceA.Play();

        float waitTime = clip.length - overlapDuration;

        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            _sourceB.volume = 0f;
            _sourceB.Play();

            float elapsed = 0f;
            while (elapsed < overlapDuration)
            {
                float t = elapsed / overlapDuration;
                _sourceA.volume = (1f - t) * volume;
                _sourceB.volume = t * volume;
                elapsed += Time.deltaTime;
                yield return null;
            }

            _sourceA.Stop();
            _sourceA.volume = volume;

            (_sourceA, _sourceB) = (_sourceB, _sourceA);

            waitTime = clip.length - overlapDuration;
        }
    }

    private static void CopySpatialization(AudioSource src, AudioSource dst)
    {
        dst.spatialBlend   = src.spatialBlend;
        dst.minDistance    = src.minDistance;
        dst.maxDistance    = src.maxDistance;
        dst.rolloffMode    = src.rolloffMode;
        dst.spread         = src.spread;
        dst.dopplerLevel   = src.dopplerLevel;
    }
}
