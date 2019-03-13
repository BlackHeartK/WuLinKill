using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理敌方相关逻辑
/// Create:2018/12
/// Last Edit:2019/3/12
/// </summary>
public class Enemy : Person {

    public RawImage weaponImage;
    public RawImage armorImage;

    #region Unity
    void Start ()
    {
        hp = MAX_HP;
        EventManager.Instance.EnemyGetCard += GetCard;
        EventManager.Instance.PlayerAttackEvent += BeAttack;
        hp_Text.text = string.Format("{0}/{1}",hp,MAX_HP);

        TextAsset textAsset = Resources.Load("EnemySpeak") as TextAsset;
        words = textAsset.text.Split('\n');
    }

    private void OnDestroy()
    {
        EventManager.Instance.EnemyGetCard -= GetCard;
        EventManager.Instance.PlayerAttackEvent -= BeAttack;
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
        base.BeAttack(i, j, damage, elementType);
    }

    /// <summary>
    /// 敌方获取卡牌数据
    /// </summary>
    /// <param name="cardDatas"></param>
	public override void GetCard (CardData[] cardDatas)
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
    public void UseCard()
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
        switch(newCard.cTpye)
        {
            case CardType.AttackCard:
                float weaponAttach = 0;
                if (weaponEle == newCard.eType)
                {
                    weaponAttach = 1;
                }
                newCard.dam = handCards[0].dam + GameManager.Instance.enemyExtraDamage + weaponAttach;
                GameManager.Instance.EnemyAttack(newCard);
                break;
            case CardType.TerrainCard:
                GameManager.Instance.EnemyAttack(newCard);
                break;
            case CardType.WeaponCard:
                Debug.Log("use Wcard");
                isHasWeapon = true;
                weaponEle = newCard.eType;
                SetEquipImage(newCard.cTpye, newCard.eType);
                break;
            case CardType.ArmorCard:
                Debug.Log("use Acard");
                isHasArmor = true;
                armorEle = newCard.eType;
                SetEquipImage(newCard.cTpye, newCard.eType);
                break;
        }
        handCards.RemoveAt(0);
    }

    void SetEquipImage(CardType cardType,ElementType elementType)
    {
        if (cardType == CardType.WeaponCard)
        {
            weaponImage.transform.parent.GetComponent<Image>().enabled = false;
            weaponImage.gameObject.SetActive(true);
        }
        else if (cardType == CardType.ArmorCard)
        {
            armorImage.transform.parent.GetComponent<Image>().enabled = false;
            armorImage.gameObject.SetActive(true);
        }
        switch (elementType)
        {
            case ElementType.Fire:
                if (cardType == CardType.WeaponCard)
                {
                    weaponImage.texture = GameManager.Instance.WeaponImage[0];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：玄火羽扇");
                }
                else if (cardType == CardType.ArmorCard)
                {
                    armorImage.texture = GameManager.Instance.ArmorImage[0];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：玄火法袍");
                }
                break;
            case ElementType.Wind:
                if (cardType == CardType.WeaponCard)
                {
                    weaponImage.texture = GameManager.Instance.WeaponImage[1];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：混元剑");
                }
                else if (cardType == CardType.ArmorCard)
                {
                    armorImage.texture = GameManager.Instance.ArmorImage[1];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：混元法袍");
                }
                break;
            case ElementType.Water:
                if (cardType == CardType.WeaponCard)
                {
                    weaponImage.texture = GameManager.Instance.WeaponImage[2];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：沧溟斧");
                }
                else if (cardType == CardType.ArmorCard)
                {
                    armorImage.texture = GameManager.Instance.ArmorImage[2];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：沧溟法袍");
                }
                break;
            case ElementType.Soil:
                if (cardType == CardType.WeaponCard)
                {
                    weaponImage.texture = GameManager.Instance.WeaponImage[3];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：铁骨扇");
                }
                else if (cardType == CardType.ArmorCard)
                {
                    armorImage.texture = GameManager.Instance.ArmorImage[3];
                    EventManager.Instance.UpdateBattleInfo("敌方使用了装备牌：铁骨甲");
                }
                break;
        }
    }
}
