using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGM
{
    Day,
    Sunset,
}

public enum SFX
{
    UI_SELECT,
    ROUND_START,
    ROUND_END,
    DAMAGED,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // ===== BGM =====
    [SerializeField] private AudioClip[] _bgmClip;
    private float bgmVolume = 0.5f;
    private AudioSource bgmPlayer;

    // ===== SFX =====
    [SerializeField] private AudioClip[] _sfxClips;
    private float sfxVolume = 0.5f;
    private readonly int channels = 16;
    private AudioSource[] sfxPlayers;
    private int sfxChannelIndex;

    // ===== Audio Mixer =====
    [SerializeField] private AudioMixer _audioMixer;
    private AudioMixerGroup _bgmMixerGroup;
    private AudioMixerGroup _sfxMixerGroup;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Init();
        BGMPlay(BGM.Day);
    }

    private void Init()
    {
        // 오디오 믹서 초기화
        _bgmMixerGroup = _audioMixer.FindMatchingGroups("Master/BGM")[0];
        _sfxMixerGroup = _audioMixer.FindMatchingGroups("Master/SFX")[0];

        // 배경음 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.outputAudioMixerGroup = _bgmMixerGroup;

        // 효과음 초기화
        sfxPlayers = new AudioSource[channels];
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxChannelIndex = 0;

        for (int i = 0; i < sfxPlayers.Length; ++i)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].loop = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].outputAudioMixerGroup = _sfxMixerGroup;
        }

    }

    public void SFXPlay(SFX sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; ++i)
        {
            int loop = (i + sfxChannelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loop].isPlaying)
                continue;

            sfxChannelIndex = loop;
            sfxPlayers[loop].clip = _sfxClips[(int)sfx];
            sfxPlayers[loop].Play();
            break;
        }
    }


    public void BGMPlay(BGM bgm)
    {
        bgmPlayer.clip = _bgmClip[(int)bgm];
        bgmPlayer.Play();
    }

    public void BGMStop()
    {
        bgmPlayer.Stop();
    }

    public void BGMChange(BGM bgm)
    {
        bgmPlayer.Stop();
        bgmPlayer.clip = _bgmClip[(int)bgm];
        bgmPlayer.Play();
    }

    public void ApplyLowPassFilter() // 소리 먹먹해짐
    {
        if (_audioMixer != null)
            _audioMixer.FindSnapshot("BGM_LowpassFilter").TransitionTo(0);
    }

    public void ResetAudioEffect()
    {
        if (_audioMixer != null)
            _audioMixer.FindSnapshot("Default").TransitionTo(0f);
    }


}