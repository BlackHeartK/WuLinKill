using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 播放动画管理
/// Create:2018/9
/// Last Edit:2019/1/7
/// </summary>
public class AnimationManager : Singleton<AnimationManager> {

    private GameObject damagePrefab;
    private GameObject defenseEffect;

    /// <summary>
    /// 初始化
    /// </summary>
	public void Init()
	{
        damagePrefab = Resources.Load("Damage") as GameObject;
        defenseEffect = Resources.Load("DefenseEffect") as GameObject;
        if (damagePrefab == null || defenseEffect == null)
        {
            Debug.LogError("未找到特效！");
            return;
        }
        Debug.Log ("AnimationM Init Finished!");
	}

	/// <summary>
	/// 激活攻击特效
	/// </summary>
	/// <param name="cardElementType">使用的卡片元素种类</param>
	/// <param name="pos">动画释放位置</param>
	public void PlayAttackAnime(ElementType cardData,float dam,Vector3 pos)
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
    public void PlayDefenseAnime(Vector3 pos)
    {
        GameObject go = Instantiate(defenseEffect) as GameObject;
        go.transform.SetParent(CanvasUI.trans);//去除此处CanvasUI的耦合
        go.transform.position = pos;
    }
}
