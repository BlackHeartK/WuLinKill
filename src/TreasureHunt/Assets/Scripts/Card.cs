using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 单张卡牌
/// Craete:2018/9
/// Last Edit:2019/3/12
/// </summary>
public class Card : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerDownHandler {

    private CardType mCardType;
    private ElementType mCardElement;
    public Image ui_Image;
    public RawImage ui_TypeImage;
    public Sprite[] eleSprites;
    public Texture2D[] ui_MainType;

    private bool willDorp;
    public bool WillDorp
    {
        set
        {
            if (value)
            {
                if (GameManager.Instance.CurHandCardCount <= GameManager.Instance.maxHandCardCount)
                { return; }
                transform.localPosition += Vector3.up * 30;
                GameManager.Instance.CurHandCardCount--;
            }
            else
            {
                transform.localPosition -= Vector3.up * 30;
                GameManager.Instance.CurHandCardCount++;
            }
            willDorp = value;
        }
    }

    private Vector3 beginDragTrns;

    public CardType cardType
    {
        get
        {return mCardType;}
    }

    public ElementType cardElement
    {
        get
        { return mCardElement; }
    }

    [SerializeField]
	private Text ui_Dam;
	private float dam;

	public Color[] cols = {
		Color.red,
		new Color(255,0,100),//土色
		new Color(0,55,255),//水色
		Color.green
	};

	/// <summary>
	/// 伤害值
	/// </summary>
	/// <value>伤害值</value>
	public float Dam
	{
		get{ return dam;}
		set
		{
			if (value <= 0) {
				Debug.Log ("设置伤害值失败,伤害值不应为0以下！");
			} else {
				dam = value;
				ui_Dam.text = dam.ToString ();
			}
		}
	}

	/// <summary>
	/// 设置卡牌属性
	/// </summary>
	/// <param name="cType">卡牌类型</param>
	/// <param name="eType">卡牌元素种类</param>
	public void SetCardProperty(CardType cType,ElementType eType)
	{
		mCardType = cType;
		mCardElement = eType;
		int i = 0;

		switch (eType) {
		case ElementType.Fire:
			i = 0;
			break;
		case ElementType.Soil:
			i = 1;
			break;
		case ElementType.Water:
			i = 2;
			break;
		case ElementType.Wind:
			i = 3;
			break;
		}

		switch (cType) {
		    case CardType.AttackCard:
			    ui_TypeImage.color = cols [i];
			    ui_Dam.color = cols [i];
                ui_Image.sprite = eleSprites[i];
                break;
		    case CardType.TerrainCard:
                ui_Image.sprite = eleSprites[i];
                ui_TypeImage.texture = ui_MainType[i];
                break;
            case CardType.WeaponCard:
                ui_Image.sprite = eleSprites[i];
                ui_TypeImage.texture = ui_MainType[i];
                break;
            case CardType.ArmorCard:
                ui_Image.sprite = eleSprites[i];
                ui_TypeImage.texture = ui_MainType[i];
                break;
		}
	}

    /// <summary>
    /// 设置卡牌属性
    /// </summary>
    /// <param name="cType">卡牌类型</param>
    /// <param name="eType">卡牌元素种类</param>
    public void SetCardProperty(CardType cType, ElementType eType,float damage)
    {
        Dam = damage;
        SetCardProperty(cType, eType);
    }

    /// <summary>
    /// 销毁自身
    /// </summary>
    public void DestroySelf()
    {
        for (int i = 0; i < UIManager.Instance.playerHandCardUI.Count; i++)
        {
            if (UIManager.Instance.playerHandCardUI[i] == gameObject)
            {
                UIManager.Instance.playerHandCardUI.RemoveAt(i);
                transform.parent.GetComponent<CanvasUI>().CardUISort();
                Destroy(gameObject);
                break;
            }
        }
    }

    /// <summary>
    /// 被丢弃
    /// </summary>
    public void BeDorp()
    {
        if (willDorp && GameManager.Instance.canDorp)
        {
            DestroySelf();
        }
    }

    /// <summary>
    /// 当结束拖拽卡牌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        CardManager.Instance.SetCurrentCard(CardType.none, ElementType.none);
        if (UIManager.Instance.currentSelectCardUI == null) { return; }
        Destroy(UIManager.Instance.currentSelectCardUI.GetComponent<CanvasGroup>());
        UIManager.Instance.currentSelectCardUI.GetComponent<RectTransform>().position = beginDragTrns;
        UIManager.Instance.currentSelectCardUI = null;
    }

    /// <summary>
    /// 当开始拖拽卡牌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null) { return; }
        if (eventData.pointerEnter.tag != "Card") { return; }
        UIManager.Instance.currentSelectCardUI = eventData.pointerEnter.gameObject;
        var card = UIManager.Instance.currentSelectCardUI.GetComponent<Card>();
        var group = UIManager.Instance.currentSelectCardUI.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        switch (card.cardType)
        {
            case CardType.AttackCard:
                CardManager.Instance.SetCurrentCard(card.cardType,card.cardElement,card.Dam);
                break;
            case CardType.TerrainCard:
                CardManager.Instance.SetCurrentCard(card.cardType, card.cardElement);
                break;
            case CardType.WeaponCard:
                CardManager.Instance.SetCurrentCard(card.cardType, card.cardElement);
                break;
            case CardType.ArmorCard:
                CardManager.Instance.SetCurrentCard(card.cardType, card.cardElement);
                break;
        }
        beginDragTrns = UIManager.Instance.currentSelectCardUI.GetComponent<RectTransform>().position;
    }

    /// <summary>
    /// 当正在拖拽卡牌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (UIManager.Instance.currentSelectCardUI == null) { return; }
        var cardRT = UIManager.Instance.currentSelectCardUI.GetComponent<RectTransform>();
        Vector3 mousePosInWorld;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(cardRT, Input.mousePosition, null, out mousePosInWorld))
        {
            cardRT.position = mousePosInWorld;
        }
    }

    /// <summary>
    /// 当点击卡牌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (UIManager.Instance.playerHandCardUI.Count > GameManager.Instance.maxHandCardCount && GameManager.Instance.canDorp)
        {
            //Debug.Log("Select");
            if (!willDorp)
            {
                WillDorp = true;
            }
            else
            {
                WillDorp = false;
            }
        }
    }

    #region Unity
    void Awake()
    {
        EventManager.Instance.DorpCardEvent += BeDorp;
    }

    void OnDestroy()
    {
        EventManager.Instance.DorpCardEvent -= BeDorp;
    }
    #endregion
}
