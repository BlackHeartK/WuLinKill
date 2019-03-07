using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using System;

/// <summary>
/// 主菜单
/// Create:2018/10
/// Last Edit:2019/1/3
/// </summary>
public class DemoMenu : MonoBehaviour {

    public Text buildVersion;

    internal static LuaEnv luaEnv = new LuaEnv();
    private float GCInterval = 1;
    private LuaTable scriptTable;
    private Action setVersion;

    [LuaCallCSharp]
    private void Start()
    {
        scriptTable = luaEnv.NewTable();

        LuaTable mate = luaEnv.NewTable();
        mate.Set("_Index", luaEnv.Global);
        scriptTable.SetMetaTable(mate);
        mate.Dispose();

        scriptTable.Set("self",this);
        luaEnv.DoString("require 'DemoMenu'");
        luaEnv.Global.Set("self", this);
        luaEnv.Global.Get("setversion", out setVersion);

        if (setVersion != null)
        {
            setVersion();
        }
    }

    private void Update()
    {
        if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.time;
        }
    }

    private void OnDestroy()
    {
        scriptTable.Dispose();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
	public void StartGame()
	{
		SceneManager.Instance.NextScene ();
	}

    /// <summary>
    /// 退出游戏
    /// </summary>
	public void QuitGame()
	{
		Application.Quit ();
	}

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="windows"></param>
    public void OpenWindows(GameObject windows)
    {
        if (windows.name == "Setting")
        {
            Slider[] sliders = windows.GetComponentsInChildren<Slider>();
            sliders[0].value = AudioManager.Instance.currentBGMvolume;
            sliders[1].value = AudioManager.Instance.currentEffectvolume;
            sliders[2].value = GameManager.Instance.AI_Difficulty;
            Toggle toggle = windows.GetComponentInChildren<Toggle>();
            toggle.isOn = AudioManager.Instance.AudioOpen;
        }
        windows.SetActive(true);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="windows"></param>
    public void CloseWindows(GameObject windows)
    {
        if (windows.name == "Setting")
        {
            GameManager.Instance.WriteConfig("Sound", "BGM_Volume", AudioManager.Instance.currentBGMvolume);
            GameManager.Instance.WriteConfig("Sound", "SoundEffect_Volume", AudioManager.Instance.currentEffectvolume);
            GameManager.Instance.WriteConfig("Sound", "SoundIsOn", AudioManager.Instance.AudioOpen);
            GameManager.Instance.WriteConfig("Game", "AI_Difficulty", GameManager.Instance.AI_Difficulty);
        }
        windows.SetActive(false);
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="volume"></param>
    public void SetSoundVolume(float volume)
    {
        AudioManager.Instance.SetSoundVolume(volume);
    }

    /// <summary>
    /// 设置音效
    /// </summary>
    /// <param name="volume"></param>
    public void SetSoundEffectVolume(float volume)
    {
        AudioManager.Instance.SetSoundEffectVolume(volume);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clip"></param>
    public void SetEffectSound(AudioClip clip)
    {
        AudioManager.Instance.PlayVoice(clip);
    }

    /// <summary>
    /// 开启or关闭所有声音
    /// </summary>
    /// <param name="state"></param>
    public void SetAllMucic(bool state)
    {
        AudioManager.Instance.AudioOpen = state;
    }

    /// <summary>
    /// 弹出提示更新的窗口
    /// </summary>
    public void ShowUpdateWindow()
    {
        Debug.Log("Need Update!");
    }
}
