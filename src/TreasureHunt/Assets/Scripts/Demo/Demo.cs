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
        SceneManager.Instance.NextScene();
    }

    /// <summary>
    /// 玩家结束该回合
    /// </summary>
    public void PlayerOverTurn()
    {
        EventManager.Instance.UpdateBattleInfo("玩家回合结束");
        GameManager.Instance.NextStage();
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="clip"></param>
    public void SetVoice(AudioClip clip)
    {
        AudioManager.Instance.PlayVoice(clip);
    }
}
