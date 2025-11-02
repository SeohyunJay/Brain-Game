using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    [Header("SFX")]
    public AudioSource sfx;
    public AudioClip shoot, crateSuccess, crateFail, crateMiss, levelUp, levelDown, noAmmo;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Music")]
    public AudioSource music;
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    public float musicFadeSeconds = 0.75f;
    public bool playMusicOnStart = true;
    public bool persistAcrossScenes = true;

    [Header("State (read-only)")]
    [SerializeField] bool muted = false;
    public bool IsMuted => muted;

    Coroutine musicFadeCo;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        if (persistAcrossScenes) DontDestroyOnLoad(gameObject);

        if (!sfx)
        {
            sfx = gameObject.AddComponent<AudioSource>();
            sfx.playOnAwake = false; sfx.loop = false; sfx.spatialBlend = 0f; sfx.dopplerLevel = 0f;
        }
        if (!music)
        {
            music = gameObject.AddComponent<AudioSource>();
            music.playOnAwake = false; music.loop = true; music.spatialBlend = 0f; music.dopplerLevel = 0f;
        }

        ApplyVolumesImmediate();
    }

    void Start()
    {
        if (playMusicOnStart && backgroundMusic)
        {
            PlayMusic(backgroundMusic, loop: true, fadeIn: musicFadeSeconds);
        }
    }

    void OnValidate()
    {
        ApplyVolumesImmediate();
    }

    void ApplyVolumesImmediate()
    {
        if (sfx) sfx.volume = muted ? 0f : sfxVolume;
        if (music) music.volume = muted ? 0f : musicVolume;
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float fadeIn = 0.5f)
    {
        if (!clip || !music) return;

        music.loop = loop;

        if (!music.isPlaying || music.clip != clip)
        {
            music.clip = clip;
            music.Play();
        }

        StartMusicFade(muted ? 0f : musicVolume, fadeIn);
    }

    public void StopMusic(float fadeOut = 0.5f)
    {
        if (!music) return;
        if (musicFadeCo != null) StopCoroutine(musicFadeCo);
        musicFadeCo = StartCoroutine(FadeMusicTo(0f, fadeOut, stopWhenDone: true));
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        if (!muted && music) music.volume = musicVolume;
    }

    void StartMusicFade(float target, float seconds)
    {
        if (!music) return;
        if (musicFadeCo != null) StopCoroutine(musicFadeCo);
        musicFadeCo = StartCoroutine(FadeMusicTo(target, Mathf.Max(0.0001f, seconds), stopWhenDone: false));
    }

    IEnumerator FadeMusicTo(float target, float seconds, bool stopWhenDone)
    {
        float t = 0f;
        float start = music.volume;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            music.volume = Mathf.Lerp(start, target, t / seconds);
            yield return null;
        }
        music.volume = target;
        if (stopWhenDone) music.Stop();
    }

    public void SetMuted(bool m)
    {
        muted = m;
        ApplyVolumesImmediate();
    }

    public void ToggleMuted() => SetMuted(!muted);

    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        if (!muted && sfx) sfx.volume = sfxVolume;
    }

    public void PlayShoot() { Play(shoot); }
    public void PlayCrateSuccess() { Play(crateSuccess); }
    public void PlayCrateFail() { Play(crateFail); }
    public void PlayCrateMiss() { Play(crateMiss); }
    public void PlayLevelUp() { Play(levelUp); }
    public void PlayLevelDown() { Play(levelDown); }
    public void PlayNoAmmo() { Play(noAmmo); }

    void Play(AudioClip clip)
    {
        if (!clip || !sfx || muted) return;
        sfx.pitch = Random.Range(0.98f, 1.02f);
        sfx.PlayOneShot(clip, sfxVolume);
    }
}
