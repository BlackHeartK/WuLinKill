using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理玩家状态
/// Create:2018/12/7
/// Last Edit Data:2019/1/4
/// </summary>
public class Player : MonoBehaviour {

    const int MAX_HP = 100;

    public List<CardData> handCards = new List<CardData>();
    public Image image_HP;
    public Text hp_Text;
    private float hp = MAX_HP;

    public float HP
    {
        get { return hp; }
        set
        {
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
    void Start () {
        EventManager.Instance.PlayerGetCard += GetCard;
        EventManager.Instance.EnemyAttackEvent += BeAttack;
        hp_Text.text = string.Format("{0}/{1}",hp,MAX_HP);
    }

    private void OnDestroy()
    {
        EventManager.Instance.PlayerGetCard -= GetCard;
        EventManager.Instance.EnemyAttackEvent -= BeAttack;
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
        HP -= damage;
    }

    /// <summary>
    /// 获得新卡片数据
    /// </summary>
    /// <param name="cardDatas"></param>
    void GetCard(CardData[] cardDatas)
    {
        for (int i = 0; i < cardDatas.Length; i++)
        {
            handCards.Add(cardDatas[i]);
        }
    }

    
}
