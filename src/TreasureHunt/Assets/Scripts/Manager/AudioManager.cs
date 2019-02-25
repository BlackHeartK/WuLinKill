using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏声音管理
/// Create:2018/12
/// Last Edit:2019/1/6
/// </summary>
public class AudioManager : Singleton<AudioManager> {

    public AudioSource bgm;
    public AudioSource[] voices;
    
    public float currentBGMvolume = 1;
    public float currentEffectvolume = 1;

    private bool audioOpen = true;
    public bool AudioOpen
    {
        get { return audioOpen; }
        set
        {
            audioOpen = value;
            if (bgm == null || voices == null)
            {
                return;
            }
            bgm.gameObject.SetActive(value);
            foreach (var item in voices)
            {
                item.gameObject.SetActive(value);
            }
            if (value)
            {
                bgm.Play();
            }
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
	public void Init () {
        EventManager.Instance.SceneChangeEvent -= Init;
        EventManager.Instance.SceneChangeEvent += Init;
        GameObject bgo = GameObject.Find("BG_Sound");
        if (bgo == null)
        {
            Debug.LogError("AudioManager Error: BG_Sound not found!");
            return;
        }
        bgm = bgo.GetComponent<AudioSource>();
        GameObject vgo = GameObject.Find("SoundEffect");
        if (vgo == null)
        {
            Debug.LogError("AudioManager Error: SoundEffect not found!");
            return;
        }
        voices = vgo.GetComponentsInChildren<AudioSource>();
        AudioOpen = audioOpen;
        SetSoundVolume(currentBGMvolume);
        SetSoundEffectVolume(currentEffectvolume);
        Debug.Log("AudioM Init Finished!");
	}

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clip"></param>
    public void PlayVoice(AudioClip clip)
    {
        voices[0].clip = clip;
        voices[0].Play();
    }

    /// <summary>
    /// 设置背景音音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetSoundVolume(float volume)
    {
        bgm.volume = volume;
        currentBGMvolume = volume;
    }

    /// <summary>
    /// 设置效果音音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetSoundEffectVolume(float volume)
    {
        currentEffectvolume = volume;
        foreach (var item in voices)
        {
            item.volume = volume;
        }
    }

    /// <summary>
    /// 停止背景音
    /// </summary>
    public void StopBGM()
    {
        bgm.Stop();
    }

    /// <summary>
    /// 播放背景音
    /// </summary>
    public void PlayBGM()
    {
        bgm.Play();
    }
}
