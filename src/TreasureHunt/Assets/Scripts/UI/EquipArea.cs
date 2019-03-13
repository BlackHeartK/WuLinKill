using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum EquipType
{
    none,
    Armor,
    Weapon
}

/// <summary>
/// 装备牌区
/// Creat：2019/3/12
/// Last Editor：2019/3/13
/// </summary>
public class EquipArea : MonoBehaviour, IDropHandler{
    
    public EquipType equipType = EquipType.Armor;
    public RawImage equipUI; 
    //public Sprite[] equipSprites;

    /// <summary>
    /// 当放置卡牌与此
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        var card = eventData.pointerDrag.gameObject.GetComponent<Card>();
        if ((equipType == EquipType.Armor && card.cardType == CardType.ArmorCard) || (equipType == EquipType.Weapon && card.cardType == CardType.WeaponCard))
        {
            //switch (card.cardType)
            //{
            //    case CardType.ArmorCard:
            //        break;
            //    case CardType.WeaponCard:
            //        break;
            //}
        }
        else
        {
            return;
        }

        Debug.Log(eventData.pointerDrag.gameObject.name + " equip");
        equipUI.transform.parent.GetComponent<Image>().enabled = false;
        equipUI.gameObject.SetActive(true);
        equipUI.texture = card.ui_TypeImage.texture;
        switch (card.cardElement)
        {
            case ElementType.Fire:
                if (equipType == EquipType.Weapon)
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：玄火羽扇");
                }
                else
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：玄火法袍");
                }
                break;
            case ElementType.Soil:
                if (equipType == EquipType.Weapon)
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：铁骨扇");
                }
                else
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：铁骨甲");
                }
                break;
            case ElementType.Water:
                if (equipType == EquipType.Weapon)
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：沧溟斧");
                }
                else
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：沧溟法袍");
                }
                break;
            case ElementType.Wind:
                if (equipType == EquipType.Weapon)
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：混元剑");
                }
                else
                {
                    EventManager.Instance.UpdateBattleInfo("玩家使用了装备卡：混元法袍");
                }
                break;
        }
        GameManager.Instance.CurHandCardCount--;
        EventManager.Instance.PlayerEquip(equipType, card.cardElement);
        card.DestroySelf();
    }
}