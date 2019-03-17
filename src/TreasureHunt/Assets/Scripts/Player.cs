using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理玩家状态
/// Create:2018/12/7
/// Last Edit:2019/3/12
/// </summary>
public class Player : Person {
    

    #region Unity
    void Start () {
        hp = MAX_HP;
        EventManager.Instance.PlayerGetCard += GetCard;
        EventManager.Instance.EnemyAttackEvent += BeAttack;
        hp_Text.text = string.Format("{0}/{1}",hp,MAX_HP);
        TextAsset textAsset = Resources.Load("PlayerSpeak") as TextAsset;
        words = textAsset.text.Split('\n');
        GameManager.Instance.player = this;
    }

    private void OnDestroy()
    {
        EventManager.Instance.PlayerGetCard -= GetCard;
        EventManager.Instance.EnemyAttackEvent -= BeAttack;
    }
    #endregion

    /// <summary>
    /// 被攻击时调用
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="damage"></param>
    /// <param name="elementType"></param>
    public override void BeAttack(int i,int j,float damage,ElementType elementType)
    {
        base.BeAttack(i,j, damage, elementType);
    }

    /// <summary>
    /// 设置装备
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="elementType"></param>
    void SetEquip(EquipType equipType, ElementType elementType)
    {
        switch (equipType)
        {
            case EquipType.Armor:
                isHasArmor = true;
                armorEle = elementType;
                break;
            case EquipType.Weapon:
                isHasWeapon = true;
                weaponEle = elementType;
                break;
        }
    }

    /// <summary>
    /// 获得新卡片数据
    /// </summary>
    /// <param name="cardDatas"></param>
    public override void GetCard(CardData[] cardDatas)
    {
        for (int i = 0; i < cardDatas.Length; i++)
        {
            handCards.Add(cardDatas[i]);
            GameManager.Instance.CurHandCardCount++;
        }
    }
}
