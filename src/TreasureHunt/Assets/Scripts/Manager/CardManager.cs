using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
//using LitJson;
using SimpleJSON;
using XLua;

/// <summary>
/// 卡片种类
/// </summary>
[LuaCallCSharp]
public enum CardType
{
    none,
    AttackCard,
    TerrainCard,
    WeaponCard,
    ArmorCard
}

/// <summary>
/// 元素种类
/// </summary>
[LuaCallCSharp]
public enum ElementType
{
    none,
    Fire,
    Water,
    Soil,
    Wind
}

/// <summary>
/// 
/// 卡片数据管理
/// Create:2018/9
/// Last Edit Data:2019/3/12
/// </summary>
public class CardManager : Singleton<CardManager> {
    
	public CardType currentSelectCardType;
	public ElementType currentSelectCardElement;
    public float currentSelectCardDamage;

	public List<CardData> cardData = new List<CardData>();
    public Dictionary<ElementType, TerrainType> elementToTerrainType = new Dictionary<ElementType, TerrainType>();
    public Dictionary<ElementType, TerrainType> restraintTable = new Dictionary<ElementType, TerrainType>();

    private List<CardData> attackCards = new List<CardData>();
    private List<CardData> terrainCards = new List<CardData>();
    private List<CardData> equipCards = new List<CardData>();

    private LuaEnv luaEnv = new LuaEnv();

    /// <summary>
    /// 卡片数据管理初始化
    /// </summary>
	public void Init()
	{
		GetAllCardFormJson ();
        elementToTerrainType.Add(ElementType.Fire,TerrainType.desert);
        elementToTerrainType.Add(ElementType.Soil, TerrainType.mountain);
        elementToTerrainType.Add(ElementType.Water, TerrainType.lake);
        elementToTerrainType.Add(ElementType.Wind, TerrainType.forest);

        restraintTable.Add(ElementType.Fire, TerrainType.forest);
        restraintTable.Add(ElementType.Soil, TerrainType.lake);
        restraintTable.Add(ElementType.Water, TerrainType.desert);
        restraintTable.Add(ElementType.Wind, TerrainType.mountain);

        luaEnv.Global.Set("self", Instance);
        luaEnv.DoString("require 'CardManager'");
        Debug.Log ("CardM Init Finished!");
	}

    /// <summary>
    /// 设置当前选中的卡牌
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="elementType"></param>
    public void SetCurrentCard(CardType cardType,ElementType elementType)
    {
        currentSelectCardType = cardType;
        currentSelectCardElement = elementType;
    }

    /// <summary>
    /// 设置当前选中的卡牌
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="elementType"></param>
    /// <param name="dam"></param>
    public void SetCurrentCard(CardType cardType, ElementType elementType,float dam)
    {
        currentSelectCardType = cardType;
        currentSelectCardElement = elementType;
        currentSelectCardDamage = dam;
    }

    /// <summary>
    /// 从Json文件获取卡牌数据
    /// </summary>
    public void GetAllCardFormJson()
	{
        string url = Application.streamingAssetsPath + "/" + "CardData.json";
        ReadJson(url);
	}

    /// <summary>
    /// 从Json中读取卡牌数据
    /// </summary>
    /// <param name="url"></param>
	void ReadJson(string url)
	{
        
#if UNITY_STANDALONE_WIN
        string jsonText = null;
        try
        {
            jsonText = File.ReadAllText(url);
        }
        catch (Exception e)
        {
            throw new Exception("未找到卡牌数据文件，卡牌数据文件路径错误！",e);
        }
        //JsonData jsonData = JsonMapper.ToObject (jsonText);
        var jsonData = JSON.Parse(jsonText);

#elif UNITY_ANDROID
		WWW www = new WWW(url);
        if(www==null)
        {
            Debug.LogError("未找到卡牌数据文件，卡牌数据文件路径错误！");
            return;
        }
		while (!www.isDone) { }
		//JsonData jsonData = JsonMapper.ToObject (www.text);
        var jsonData = JSON.Parse(www.text);
#endif
        for (int i = 0; i < jsonData["Cards"].Count; i++)
        {
            //string type = jsonData["Cards"][i]["Type"].ToString();
            string type = jsonData["Cards"][i]["Type"].ToString().Split('"')[1];
            
            //string element = jsonData["Cards"][i]["Element"].ToString();
            string element = jsonData["Cards"][i]["Element"].ToString().Split('"')[1];

            CardData cd = new CardData();
            switch (element)
            {
                case "Fire":
                    cd.eType = ElementType.Fire;
                    break;
                case "Soil":
                    cd.eType = ElementType.Soil;
                    break;
                case "Water":
                    cd.eType = ElementType.Water;
                    break;
                case "Wind":
                    cd.eType = ElementType.Wind;
                    break;
                default: continue;
            }
            switch (type)
            {
                case "Attack":
                    cd.cTpye = CardType.AttackCard;
                    cd.dam = int.Parse(jsonData["Cards"][i]["Damage"].ToString().Split('"')[1]);
                    attackCards.Add(cd);
                    break;
                case "Terrain":
                    cd.cTpye = CardType.TerrainCard;
                    terrainCards.Add(cd);
                    break;
                case "Weapon":
                    cd.cTpye = CardType.WeaponCard;
                    equipCards.Add(cd);
                    break;
                case "Armor":
                    cd.cTpye = CardType.ArmorCard;
                    equipCards.Add(cd);
                    break;
                default:continue;
            }
            cardData.Add(cd);
        }
    }

    /// <summary>
    /// 从卡组中获取新卡
    /// </summary>
    /// <returns></returns>
    [Hotfix]
    public CardData[] GetNewCard()
    {
        int cardCount = 0;
        if (GameManager.Instance.battleSatge == GameManager.BattleStage.PlayerGetCard)
        {
            cardCount = GameManager.Instance.playerGetNewCardCount;
        }
        if (GameManager.Instance.battleSatge == GameManager.BattleStage.EnemyGetCard)
        {
            cardCount = GameManager.Instance.enemyGetNewCardCount;
        }
        CardData[] newCard = new CardData[cardCount];
        for (int i = 0; i < newCard.Length; i++)
        {
            newCard[i] = cardData[UnityEngine.Random.Range(0, cardData.Count - 1)];
        }
        return newCard;
    }
}
