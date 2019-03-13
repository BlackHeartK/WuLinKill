using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Person : MonoBehaviour {

    public List<CardData> handCards = new List<CardData>();

    protected int MAX_HP = 100;
    [HideInInspector]
    public bool isHasWeapon;
    [HideInInspector]
    public bool isHasArmor;
    [HideInInspector]
    public ElementType weaponEle = ElementType.none;
    [HideInInspector]
    public ElementType armorEle = ElementType.none;

    public Text word_Text;

    public Image image_HP;
    public Text hp_Text;

    protected string[] words;
    protected float hp;

    public float Hp
    {
        get { return hp; }
        set
        {
            if (value > 0)
            {
                if (value < 25)
                {
                    word_Text.text = words[3];
                }
                else if (value < 50)
                {
                    word_Text.text = words[2];
                }
                else if (value < 75)
                {
                    word_Text.text = words[1];
                }
                else
                {
                    word_Text.text = words[0];
                }
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

    /// <summary>
    /// 被攻击时调用
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="damage"></param>
    /// <param name="elementType"></param>
    public virtual void BeAttack(int i, int j, float damage, ElementType elementType)
    {
        //对应属性装甲使伤害减半
        if (isHasArmor && elementType == armorEle)
        {
            damage = damage * 0.5f;
        }
        Hp -= damage;
    }


    /// <summary>
    /// 获得新卡片数据
    /// </summary>
    /// <param name="cardDatas"></param>
    public virtual void GetCard(CardData[] cardDatas)
    { }
}
