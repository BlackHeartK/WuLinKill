using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// UI界面管理
/// Create:2018/9
/// Last Edit:2019/1/7
/// </summary>
public class UIManager : Singleton<UIManager> {

	public enum UiArea
	{
		CanvasRoot,
		Player,
		Emeny,
		BattleArea,
		OverPlayerTurnButton,
	}

	public Dictionary<UiArea,GameObject> uiDictionary = new Dictionary<UiArea, GameObject>();
    public List<GameObject> playerHandCardUI = new List<GameObject>();
    public GameObject currentSelectCardUI;

    /// <summary>
    /// 初始化
    /// </summary>
	public void Init()
	{
        EventManager.Instance.SceneChangeEvent += ToReset;
        Debug.Log ("UiM Init Finished!");
	}

	/// <summary>
	/// 激活或关闭指定UI
	/// </summary>
	/// <param name="uia">指定的UI</param>
	/// <param name="state">If set to <c>true</c> 是否为激活</param>
	public void EnableOrDisableUI(UiArea uia,bool state)
	{
		GameObject ui;
		try
		{
			ui = uiDictionary [uia];
		}
		catch(Exception e)
		{
			Debug.Log ("查询指定的UI出错!");
			throw new Exception (e.Message);
		}
		//Debug.Log ("查询指定的UI完成!");
		ui.SetActive (state);
	}

    /// <summary>
    /// 清除玩家所有手牌
    /// </summary>
    public void ClearCardUI()
    {
        for (int i = 0; i < playerHandCardUI.Count; i++)
        {
            Destroy(playerHandCardUI[i]);
        }
    }

    /// <summary>
    /// 冻结玩家手牌
    /// </summary>
    /// <param name="state"></param>
    public void FrezzePlayerHandCard(bool state)
    {
        for (int i = 0; i < playerHandCardUI.Count; i++)
        {
            if (state)
            {
                if (playerHandCardUI[i].GetComponent<CanvasGroup>() == null)
                {
                    playerHandCardUI[i].AddComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }
            else
            {
                if (playerHandCardUI[i].GetComponent<CanvasGroup>() != null)
                {
                    Destroy(playerHandCardUI[i].GetComponent<CanvasGroup>());
                }
            }
        }
    }

    /// <summary>
    /// 重置
    /// </summary>
    void ToReset()
    {
        currentSelectCardUI = null;
        playerHandCardUI.Clear();
        playerHandCardUI = new List<GameObject>();
    }
}
