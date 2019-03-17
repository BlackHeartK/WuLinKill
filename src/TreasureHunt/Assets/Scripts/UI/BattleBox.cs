using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// 地形类别
/// </summary>
public enum TerrainType
{
    none,
    mountain,
    lake,
    forest,
    desert
}

/// <summary>
/// 九宫格的格子
/// Create:2018/9
/// Last Edit Data:2019/3/16 地形类别加入none
/// </summary>
public class BattleBox : MonoBehaviour,IDropHandler {
    
	/// <summary>
	/// 阵营
	/// </summary>
	public enum Camp
	{
		none,
		my,
		enemy
	}
    
    public GameObject frame;
    [SerializeField]
    private Sprite[] frameSprite = new Sprite[2];

    [SerializeField]
	private Camp camp = Camp.none;
    /// <summary>
    /// 阵营
    /// </summary>
    public Camp _Camp
    {
        get { return camp; }
        set {
            if (value == camp) { return; }
            frame.SetActive(true);
            if (value == Camp.my)
            {
                frame.GetComponent<Image>().sprite = frameSprite[0];
                if (camp == Camp.enemy)
                {
                    GameManager.Instance.enemyExtraDamage-=0.5f;
                }
                GameManager.Instance.playerExtraDamage+=0.5f;
            }
            if (value == Camp.enemy)
            {
                frame.GetComponent<Image>().sprite = frameSprite[1];
                if (camp == Camp.my)
                {
                    GameManager.Instance.playerExtraDamage-=0.5f;
                }
                GameManager.Instance.enemyExtraDamage+=0.5f;
            }

            camp = value;
        }
    }

    [SerializeField]
    private float hp = 4;
    /// <summary>
    /// 血量
    /// </summary>
    public float HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= 0)
            {
                hp = 4;
                if (GameManager.Instance.battleSatge == GameManager.BattleStage.PlayerUseCard)
                {
                    if (_Camp == Camp.enemy || _Camp == Camp.none)
                    {
                        _Camp = Camp.my;
                    }
                }
                if (GameManager.Instance.battleSatge == GameManager.BattleStage.EnemyUseCard)
                {
                    if (_Camp == Camp.my || _Camp == Camp.none)
                    {
                        _Camp = Camp.enemy;
                    }
                }
            }
        }
    }

	private TerrainType myTerrainType = TerrainType.none;
	private RawImage mRawImage;

    /// <summary>
    /// 初始化此九宫格
    /// </summary>
    public void WakeUp()
    {
        mRawImage = GetComponent<RawImage>();
        if (Helper.isInstructionMode) { return; }
        SetTerrain((TerrainType)Random.Range(1, 4));
        //		Debug.Log (string.Format("{0}:{1}",GetIndex,myTerrainType));
    }

    /// <summary>
    /// 获取此格序号
    /// </summary>
    /// <value>The index.</value>
    public string GetIndex
	{
		get{ return name;}
	}

	/// <summary>
	/// 获取此格地形
	/// </summary>
	/// <value>The type of the get terrain.</value>
	public TerrainType GetTerrainType
	{
		get{ return myTerrainType;}
	}

    #region Unity
    void Awake()
	{
		EventManager.Instance.InitFinished += this.WakeUp;
	}
    void OnDestroy()
    {
        EventManager.Instance.InitFinished -= this.WakeUp;
    }
    #endregion

    /// <summary>
    /// 设置此九宫格地形
    /// </summary>
    /// <param name="type">地形枚举类型</param>
    public void SetTerrain(TerrainType type)
	{
		myTerrainType = type;
		switch (type) {
		case TerrainType.desert:
			//mRawImage.texture = GameManager.Instance.terrainImage [0];
            StartCoroutine(PlayChangeTerrainAnime(GameManager.Instance.terrainImage[0]));
			break;
		case TerrainType.forest:
			//mRawImage.texture = GameManager.Instance.terrainImage [1];
            StartCoroutine(PlayChangeTerrainAnime(GameManager.Instance.terrainImage[1]));
            break;
		case TerrainType.lake:
			//mRawImage.texture = GameManager.Instance.terrainImage [2];
            StartCoroutine(PlayChangeTerrainAnime(GameManager.Instance.terrainImage[2]));
            break;
		case TerrainType.mountain:
			//mRawImage.texture = GameManager.Instance.terrainImage [3];
            StartCoroutine(PlayChangeTerrainAnime(GameManager.Instance.terrainImage[3]));
            break;
		}
	}

    IEnumerator PlayChangeTerrainAnime(Texture2D terrainImage)
    {
        Vector3 currentRot = transform.localEulerAngles;
        transform.DOLocalRotate(currentRot + new Vector3(180, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        currentRot = transform.localEulerAngles;
        transform.DOLocalRotate(currentRot + new Vector3(180, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.25f);
        mRawImage.texture = terrainImage;
    }



    /// <summary>
    /// 当释放卡牌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (myTerrainType == TerrainType.none) { return; }
        var card = eventData.pointerDrag.gameObject.GetComponent<Card>();
        if (card.cardType == CardType.ArmorCard || card.cardType == CardType.WeaponCard)
        {
            return;
        }
        
        //Debug.Log(eventData.pointerDrag.gameObject.name + "Attack on " + GetIndex);
        
        CardData cd = new CardData();
        cd.cTpye = card.cardType;
        cd.eType = card.cardElement;
        cd.dam = card.Dam;
        if (cd.eType == GameManager.Instance.player.weaponEle)
        {
            cd.dam += 1;
        }
        GameManager.Instance.PlayerAttack(GetIndex, cd);
        GameManager.Instance.CurHandCardCount--;
        card.DestroySelf();
        
    }
    
}
