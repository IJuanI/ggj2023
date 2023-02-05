using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct BackgroundAudio
{

    [Range(0f, 100f)]
    public float entryPercentage;
  
    public AudioClip clip;

}

public class MusicController : MonoBehaviour
{
    // Singleton
    public static MusicController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float targetScore = 20f;

    float currentScore = 0f;

    public AudioSource musicSource;

    public BackgroundAudio[] musicClips;

    void Start()
    {
        StartCoroutine(PlayMusic());
    }

    AudioClip GetCurrentClip() {
        foreach (BackgroundAudio clip in musicClips) {
            if (currentScore >= clip.entryPercentage / 100f * targetScore) {
                return clip.clip;
            }
        }
        return null;
    }

    IEnumerator PlayMusic() {
        while (true) {
            AudioClip clip = GetCurrentClip();
            if (clip != null) {
                musicSource.clip = clip;
                musicSource.Play();
                yield return new WaitForSeconds(clip.length);
            } else {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void AddScore() {
        currentScore += 1f;
    }

}
