using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件管理
/// Create:2018/9
/// Last Edit Data:2019/1/7
/// </summary>
public class EventManager : Singleton<EventManager> {

	public delegate void InitFinishedEventHandler();
	public event InitFinishedEventHandler InitFinished;
    public delegate void SceneChangeEventHandler();
    public event SceneChangeEventHandler SceneChangeEvent;//??????
    public delegate void BattleVectoryEventHandler();
    public event BattleVectoryEventHandler battleVectoryEvent;
    public delegate void BattleFailEventHandler();
    public event BattleFailEventHandler battleFailEvent;

    //回合阶段事件
    public delegate void PlayerGetCardEventHandler(CardData[] cardDatas);
    public event PlayerGetCardEventHandler PlayerGetCard;
    public delegate void PlayerUseCardEventHandler();
    public event PlayerUseCardEventHandler PlayerUseCard;
    public delegate void EnemyGetCardEventHandler(CardData[] cardDatas);
    public event EnemyGetCardEventHandler EnemyGetCard;
    public delegate void EnemyUseCardEventHandler();
    public event EnemyUseCardEventHandler EnemyUseCard;
    public delegate void DorpCardEventHandler();
    public event DorpCardEventHandler DorpCardEvent;

    //攻击事件
    public delegate void AttackEventHandler(int boxRow,int boxCol,float damage);
    public event AttackEventHandler PlayerAttackEvent;
    public event AttackEventHandler EnemyAttackEvent;

    public delegate void UpdateBattleInfoEventHandler(string info);
    public UpdateBattleInfoEventHandler UpdateBattleInfo;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
	{
		Debug.Log ("EventM Init Finished!");
	}

    /// <summary>
    /// 触发游戏初始化完成事件
    /// </summary>
	public void InitFinishedEvent()
	{
		if (InitFinished != null) {
			InitFinished ();
			return;
		}
		Debug.Log ("Event InitFinished fail!");
	}

    /// <summary>
    /// 触发战斗结束事件
    /// </summary>
    public void GameOver()
    {
        UIManager.Instance.ClearCardUI();
        if (GameManager.Instance.battleSatge == GameManager.BattleStage.PlayerUseCard)
        {
            battleVectoryEvent();
        }
        if (GameManager.Instance.battleSatge == GameManager.BattleStage.EnemyUseCard)
        {
            battleFailEvent();
        }
    }

    /// <summary>
    /// 触发玩家攻击事件
    /// </summary>
    /// <param name="boxRow"></param>
    /// <param name="boxCol"></param>
    /// <param name="dam"></param>
    public void PlayerAttack(int boxRow,int boxCol,float dam)
    {
        if (PlayerAttackEvent != null)
        {
            PlayerAttackEvent(boxRow, boxCol, dam);
        }
    }

    /// <summary>
    /// 触发敌方攻击事件
    /// </summary>
    /// <param name="boxRow"></param>
    /// <param name="boxCol"></param>
    /// <param name="dam"></param>
    public void EnemyAttack(int boxRow, int boxCol,float dam)
    {
        if (EnemyAttackEvent != null)
        {
            EnemyAttackEvent(boxRow, boxCol, dam);
        }
    }

    /// <summary>
    /// 触发丢弃卡牌事件
    /// </summary>
    public void DorpCard()
    {
        if (DorpCardEvent != null)
        {
            DorpCardEvent();
        }
    }

    /// <summary>
    /// 进行新回合阶段事件
    /// </summary>
    /// <param name="battleSatge"></param>
	public void GotoNewStageEvent(GameManager.BattleStage battleSatge)
	{
        if (GameManager.Instance.GameOver) { return; }
		switch (battleSatge) {
		case GameManager.BattleStage.PlayerGetCard://玩家抽卡阶段
                UpdateBattleInfo("玩家抽卡");
                if (PlayerGetCard != null)
                    PlayerGetCard(CardManager.Instance.GetNewCard());
                break;
		case GameManager.BattleStage.PlayerUseCard://玩家用卡阶段
                UpdateBattleInfo("玩家回合开始");
                if (PlayerUseCard != null)
                {
                    PlayerUseCard();
                }
                UIManager.Instance.FrezzePlayerHandCard(false);
                UIManager.Instance.EnableOrDisableUI (UIManager.UiArea.OverPlayerTurnButton, true);
			break;
		case GameManager.BattleStage.EnemyGetCard://敌方抽卡阶段
                UpdateBattleInfo("敌方抽卡");
                if (EnemyGetCard != null)
                {
                    EnemyGetCard(CardManager.Instance.GetNewCard());
                }
                UIManager.Instance.FrezzePlayerHandCard(true);
                UIManager.Instance.EnableOrDisableUI (UIManager.UiArea.OverPlayerTurnButton, false);
			break;
		case GameManager.BattleStage.EnemyUseCard://敌方用卡阶段
                UpdateBattleInfo("敌方回合开始");
                if (EnemyUseCard != null)
                    EnemyUseCard();
			break;
		}
	}

    /// <summary>
    /// 触发场景切换事件
    /// </summary>
    public void SceneChange()
    {
        if (SceneChangeEvent != null)
        {
            SceneChangeEvent();
        }
    }
}

