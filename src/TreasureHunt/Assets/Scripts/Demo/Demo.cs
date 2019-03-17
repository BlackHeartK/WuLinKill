using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

/// <summary>
/// Demo
/// Create:2018/9
/// Last Edit Data:2019/1/5
/// </summary>
public class Demo : MonoBehaviour {

	public PlayableDirector playableDirector;

    #region Unity
    void Start () {
        if (Helper.isInstructionMode) { return; }
		StartCoroutine (GameManager.Instance.ReadyStartBattle (playableDirector));
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			GameManager.Instance.NextStage ();
			Debug.Log (GameManager.Instance.battleSatge);
		}
    }
    #endregion
    /// <summary>
    /// 结束对战并返回菜单
    /// </summary>
    public void EndBattle()
    {
        UIManager.Instance.uiDictionary.Clear();
        GameManager.Instance.battleSatge = GameManager.BattleStage.None;
        SceneManager.Instance.BackMenu();
    }

    /// <summary>
    /// 玩家结束该回合
    /// </summary>
    public void PlayerOverTurn()
    {
        if (UIManager.Instance.playerHandCardUI.Count <= GameManager.Instance.maxHandCardCount)
        {
            GameManager.Instance.canDorp = false;
            EventManager.Instance.UpdateBattleInfo("玩家回合结束");
            GameManager.Instance.NextStage();
        }
        else
        {
            GameManager.Instance.canDorp = true;
            UIManager.Instance.uiDictionary[UIManager.UiArea.DorpButton].SetActive(true);
            UIManager.Instance.uiDictionary[UIManager.UiArea.OverPlayerTurnButton].SetActive(false);
        }
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="clip"></param>
    public void SetVoice(AudioClip clip)
    {
        AudioManager.Instance.PlayVoice(clip);
    }

    /// <summary>
    /// 确认丢弃卡牌
    /// </summary>
    public void ConfrimDorp()
    {
        EventManager.Instance.DorpCard();
        if (UIManager.Instance.playerHandCardUI.Count <= GameManager.Instance.maxHandCardCount)
        {
            UIManager.Instance.uiDictionary[UIManager.UiArea.OverPlayerTurnButton].SetActive(true);
            UIManager.Instance.uiDictionary[UIManager.UiArea.DorpButton].SetActive(false);
        }
    }
}
