using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理敌方相关逻辑
/// Create:2018/12
/// Last Edit:2019/1/4
/// </summary>
public class Enemy : MonoBehaviour {

    const int MAX_HP = 100;

    public static List<CardData> handCards = new List<CardData>();
    public Image image_HP;
    public Text hp_Text;

    private float hp = MAX_HP;
    public float Hp
    {
        get { return hp; }
        set {
            if (value > 0)
            {
                hp = value;
                image_HP.fillAmount = hp / MAX_HP;
            }
            else
            {
                hp = 0;
                image_HP.fillAmount = 0;
                GameManager.Instance.GameOver = true;
            }
            hp_Text.text = string.Format("{0}/{1}", hp, MAX_HP);
        }
    }
    #region Unity
    void Start ()
    {
        EventManager.Instance.EnemyGetCard += GetCard;
        EventManager.Instance.PlayerAttackEvent += BeAttack;
        hp_Text.text = string.Format("{0}/{1}",hp,MAX_HP);
    }

    private void OnDestroy()
    {
        EventManager.Instance.EnemyGetCard -= GetCard;
        EventManager.Instance.PlayerAttackEvent -= BeAttack;
    }
    #endregion

    /// <summary>
    /// 被攻击
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="damage"></param>
    void BeAttack(int i,int j,float damage)
    {
        Hp -= damage;
    }

    /// <summary>
    /// 敌方获取卡牌数据
    /// </summary>
    /// <param name="cardDatas"></param>
	void GetCard (CardData[] cardDatas)
    {
        for (int i = 0; i < cardDatas.Length; i++)
        {
            handCards.Add(cardDatas[i]);
        }
        StartCoroutine(PassTime());
	}

    IEnumerator PassTime()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.NextStage();
    }

    /// <summary>
    /// 敌方使用卡牌
    /// </summary>
    public static void UseCard()
    {
        Debug.Log("敌方手牌数："+handCards.Count);
        if (handCards.Count == 0)
        {
            AIManager.aiFSM_System.PerformRule(AIrule.AICardCountIsZero);
            GameManager.Instance.NextStage();
            return;
        }
        CardData newCard = new CardData();
        newCard.cTpye = handCards[0].cTpye;
        newCard.eType = handCards[0].eType;
        newCard.dam = handCards[0].dam + GameManager.Instance.enemyExtraDamage;
        GameManager.Instance.EnemyAttack(newCard);
        handCards.RemoveAt(0);
    }

    
}
