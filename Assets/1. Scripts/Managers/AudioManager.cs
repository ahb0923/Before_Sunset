using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private float _bgmVolume = 0.5f;
    private float _sfxVolume = 0.2f;

    [Header("BGM")]
    public AudioSource bgmSource;

    private int _sfxPoolSize = 30;
    private Queue<AudioSource> _sfxPool;

    private Dictionary<string, AudioClip> _bgmClips = new();
    private Dictionary<string, AudioClip> _sfxClips = new();

    private Dictionary<string, float> _volumeOverrides = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitBGM();
            InitSFXPool();
            LoadAllClips();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitBGM()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = _bgmVolume;
    }

    private void InitSFXPool()
    {
        _sfxPool = new Queue<AudioSource>();
        for (int i = 0; i < _sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            _sfxPool.Enqueue(source);
        }
    }

    private void LoadAllClips()
    {
        var bgms = Resources.LoadAll<AudioClip>("Sounds/BGM");
        foreach (var clip in bgms)
            _bgmClips[clip.name] = clip;

        var sfxs = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (var clip in sfxs)
            _sfxClips[clip.name] = clip;

        _volumeOverrides["BasicMine1"] = 0.1f;
        _volumeOverrides["DefenseBase"] = 0.1f;
        _volumeOverrides["RareMine1"] = 0.1f;
    }

    public void PlayBGM(string name)
    {
        if (_bgmClips.TryGetValue(name, out var clip))
        {
            bgmSource.clip = clip;

            float overrideVolume = _bgmVolume;
            if (_volumeOverrides.TryGetValue(name, out var multiplier))
            {
                overrideVolume *= multiplier;
            }

            bgmSource.volume = overrideVolume;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{name}' not found");
        }
    }

    public void PlaySFX(string name)
    {
        if (_sfxClips.TryGetValue(name, out var clip))
        {
            AudioSource source = GetAvailableSFXSource();
            source.clip = clip;

            float overrideVolume = _sfxVolume;

            if (_volumeOverrides.TryGetValue(name, out var multiplier))
            {
                overrideVolume *= multiplier;
            }

            source.volume = overrideVolume;
            source.Play();
            StartCoroutine(ReturnToPoolWhenDone(source, clip.length));
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' not found");
        }
    }

    // 효과음 여러개 재생시키고 싶을 때
    public void PlayRandomSFX(string namePrefix, int variantCount)
    {
        if (variantCount <= 0)
        {
            Debug.LogWarning($"Variant count must be greater than 0 for '{namePrefix}'");
            return;
        }

        int rand = Random.Range(1, variantCount + 1);
        string sfxName = $"{namePrefix}{rand}";

        PlaySFX(sfxName);
    }

    public void PlayMonsterSFX(string monsterKey, string action)
    {
        string clipName = $"{monsterKey}_{action}";
        PlaySFX(clipName);
    }

    private AudioSource GetAvailableSFXSource()
    {
        AudioSource source = _sfxPool.Dequeue();
        _sfxPool.Enqueue(source);
        return source;
    }

    private System.Collections.IEnumerator ReturnToPoolWhenDone(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        source.clip = null;
    }

    public void SetWholeVolume(float volume)
    {
        SetBGMVolume(volume);
        SetSFXVolume(volume);
    }

    public float GetWholeVolume()
    {
        return (_bgmVolume + _sfxVolume) / 2f;
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = volume;
        bgmSource.volume = volume;
    }

    public float GetBGMVolume()
    {
        return _bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
    }

    public float GetSFXVolume()
    {
        return _sfxVolume;
    }
}
