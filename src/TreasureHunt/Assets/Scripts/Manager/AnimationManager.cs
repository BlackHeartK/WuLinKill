using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using UnityEngine.Networking;

/// <summary>
/// 播放动画管理
/// Create:2018/9
/// Last Edit:2019/1/7
/// </summary>
public class AnimationManager : Singleton<AnimationManager> {

    [LuaCallCSharp]
    private GameObject damagePrefab;
    [LuaCallCSharp]
    private GameObject defenseEffect;
    [LuaCallCSharp]
    private GameObject waterEffect;
    [LuaCallCSharp]
    private GameObject windEffect;
    [LuaCallCSharp]
    private GameObject fireEffect;
    [LuaCallCSharp]
    private GameObject soilEffect;
    
    private LuaEnv luaEnv = new LuaEnv();

    /// <summary>
    /// 初始化
    /// </summary>
	public void Init()
	{
        damagePrefab = Resources.Load("Damage") as GameObject;
        defenseEffect = Resources.Load("DefenseEffect") as GameObject;
        waterEffect = Resources.Load("WaterEffect") as GameObject;
        windEffect = Resources.Load("WindEffect") as GameObject;
        fireEffect = Resources.Load("FireEffect") as GameObject;
        soilEffect = Resources.Load("SoilEffect") as GameObject;
        if (damagePrefab == null || defenseEffect == null || waterEffect == null || windEffect == null)
        {
            Debug.LogError("未找到特效！");
        }
        Debug.Log ("AnimationM Init Finished!");

        luaEnv.Global.Set("self", this);
        luaEnv.DoString("require 'AnimationManager'");

    }

    private void OnApplicationQuit()
    {
        luaEnv.Dispose();
    }

    /// <summary>
    /// 激活攻击特效
    /// </summary>
    /// <param name="cardElementType">使用的卡片元素种类</param>
    /// <param name="pos">动画释放位置</param>
    [Hotfix]
    public void PlayAttackAnime(ElementType etype,float dam,Vector3 pos)
	{
        //Debug.Log("攻击伤害动画释放位置：" + pos);
        GameObject go = Instantiate(damagePrefab) as GameObject;
        go.transform.SetParent(CanvasUI.trans);//去除此处CanvasUI的耦合
        go.transform.position = pos;
        go.GetComponentInChildren<Text>().text = (-dam).ToString();
        //Debug.Log("Damge:" + go.GetComponent<Text>().text);
	}

    /// <summary>
    /// 激活防御特效
    /// </summary>
    /// <param name="pos"></param>
    [Hotfix]
    public void PlayDefenseAnime(Vector3 pos)
    {
        GameObject go = Instantiate(defenseEffect) as GameObject;
        go.transform.SetParent(CanvasUI.trans);//去除此处CanvasUI的耦合
        go.transform.position = pos;
    }
}
