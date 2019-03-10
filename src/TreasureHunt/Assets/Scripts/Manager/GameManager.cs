using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using XLua;

/// <summary>
/// 游戏主要流程逻辑管理
/// Create:2018/9
/// Last Edit Data:2018/1/7
/// </summary>
public class GameManager : Singleton<GameManager> {

	/// <summary>
	/// 回合阶段
	/// </summary>
	public enum BattleStage
	{
		None,
		PlayerGetCard,
		PlayerUseCard,
		EnemyGetCard,
		EnemyUseCard
	}

	public BattleStage battleSatge = BattleStage.PlayerGetCard;
	[Tooltip("顺序为：沙漠、森林、湖泊、山地")]
	public Texture2D[] terrainImage;
    [Tooltip("顺序为：火、风、水、地、场地变动")]
    public AudioClip[] attackSoundEffect;
    public AudioClip terrainSoundEffect;
    public AudioClip defenseSoundEffect;

    public int startCardCount = 3;//初始抽卡数
    public int playerGetNewCardCount = 2;//玩家新回合抽卡数
    public int enemyGetNewCardCount = 2;//敌方新回合抽卡数
    public float playerExtraDamage = 0;//玩家额外伤害
    public float enemyExtraDamage = 0;//敌方额外伤害
    public int AI_Difficulty = 0;//AI难度 0 or 1
    public readonly int maxHandCardCount = 5;//最大手牌持有数
    public bool canDorp;
    private int curHandCardCount;//当前手牌有效卡牌数
    public int CurHandCardCount
    {
        get { return curHandCardCount; }
        set
        {
            curHandCardCount = value;
        }
    }

    private bool gameover;
    public bool GameOver
    {
        get { return gameover; }
        set
        {
            gameover = value;
            if (gameover)
            {
                EventManager.Instance.GameOver();
            }
        }
    }

    private string fileName = "Config.xml";
    private string xmlFilePath;
    private int stageCount;

    internal static LuaEnv luaEnv = new LuaEnv();

    /// <summary>
    /// 初始化
    /// </summary>
    [LuaCallCSharp]
    public void Init()
	{
        ReadConfig();
		battleSatge = BattleStage.None;
		terrainImage = new Texture2D[4];
		stageCount = System.Enum.GetNames (battleSatge.GetType ()).Length;
		terrainImage[0] = Resources.Load ("TerrainImage/沙漠") as Texture2D;
		terrainImage[1] = Resources.Load ("TerrainImage/森林") as Texture2D;
		terrainImage[2] = Resources.Load ("TerrainImage/湖泊") as Texture2D;
		terrainImage[3] = Resources.Load ("TerrainImage/山地") as Texture2D;

        attackSoundEffect = new AudioClip[4];
        attackSoundEffect[0] = Resources.Load("Sound/Effect/火") as AudioClip;
        attackSoundEffect[1] = Resources.Load("Sound/Effect/风") as AudioClip;
        attackSoundEffect[2] = Resources.Load("Sound/Effect/水") as AudioClip;
        attackSoundEffect[3] = Resources.Load("Sound/Effect/地") as AudioClip;
        terrainSoundEffect = Resources.Load("Sound/Effect/地形变动") as AudioClip;

        defenseSoundEffect = Resources.Load("Sound/Effect/防御2") as AudioClip;

        luaEnv.DoString("require 'GameManager'");

        EventManager.Instance.SceneChangeEvent += ToReset;
        Debug.Log ("GameM Init Finished!");
	}

    private void OnApplicationQuit()
    {
        luaEnv.Dispose();
    }

    /// <summary>
    /// 从XML文件中读取数据
    /// </summary>
    /// <returns></returns>
    XElement LoadConfigFile()
    {
#if UNITY_EDITOR
        xmlFilePath = string.Format("{0}/{1}", Application.streamingAssetsPath, fileName);
        XElement root = XElement.Load(xmlFilePath);
        return root;
#elif UNITY_ANDROID
        xmlFilePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);
        FileInfo fileInfo = new FileInfo(xmlFilePath);
        if (!fileInfo.Exists)
        {
            TextAsset textAsset = Resources.Load("Config") as TextAsset;
            //XmlReader reader = XmlReader.Create(new StringReader(textAsset.text));
            StreamWriter sw = fileInfo.CreateText();
            sw.Write(textAsset.text);
            sw.Close();
            sw.Dispose();
        }
        XElement root = XElement.Load(xmlFilePath);
        return root;
#endif
    }

    /// <summary>
    /// 读取配置文件
    /// </summary>
    void ReadConfig()
    {
        XElement root = LoadConfigFile();
        if (root == null) { return; }
        AI_Difficulty = int.Parse(root.Element("Game").Element("AI_Difficulty").Value);
        playerGetNewCardCount = int.Parse(root.Element("Player").Element("PlayerGetNewCard").Value);
        enemyGetNewCardCount = int.Parse(root.Element("Enemy").Element("EnemyGetNewCard").Value);
        AudioManager.Instance.AudioOpen = bool.Parse(root.Element("Sound").Element("SoundIsOn").Value);
        AudioManager.Instance.currentBGMvolume = float.Parse(root.Element("Sound").Element("BGM_Volume").Value);
        AudioManager.Instance.currentEffectvolume = float.Parse(root.Element("Sound").Element("SoundEffect_Volume").Value);
    }

    /// <summary>
    /// 测试用创建配置文件
    /// </summary>
    //public void WriteConfig()
    //{
    //    XElement root = new XElement("Config");

    //    XElement gameElem = new XElement("Game",
    //        new XElement("PlayerHandCardCountLimit",6)
    //        );
    //    root.Add(gameElem);

    //    XElement soundElem = new XElement("Sound",
    //        new XElement("BGM_Volume", 100),
    //        new XElement("SoundEffect_Volume",100),
    //        new XElement("SoundIsOn",true)
    //        );
    //    root.Add(soundElem);

    //    XElement playerElem = new XElement("Player",
    //        new XElement("PlayerGetNewCard",2)
    //        );
    //    root.Add(playerElem);

    //    XElement enemyElem = new XElement("Enemy",
    //        new XElement("EnemyGetNewCard",2)
    //        );
    //    root.Add(enemyElem);

    //    string xmlFilePath = string.Format("{0}/{1}", Application.streamingAssetsPath, "Config.xml");
    //    root.Save(xmlFilePath);
    //}

   
    public void WriteConfig(string sectionName, string key, string value)
    {
        XElement root = LoadConfigFile();
        root.Element(sectionName).Element(key).Value = value;
        root.Save(xmlFilePath);
    }

    public void WriteConfig(string sectionName, string key, int value)
    {
        XElement root = LoadConfigFile();
        root.Element(sectionName).Element(key).Value = value.ToString();
        root.Save(xmlFilePath);
    }

    public void WriteConfig(string sectionName,string key, float value)
    {
        XElement root = LoadConfigFile();
        root.Element(sectionName).Element(key).Value = value.ToString();
        root.Save(xmlFilePath);
    }

    public void WriteConfig(string sectionName, string key, bool value)
    {
        XElement root = LoadConfigFile();
        root.Element(sectionName).Element(key).Value = value.ToString();
        root.Save(xmlFilePath);
    }

    /// <summary>
    /// 玩家攻击
    /// </summary>
    /// <param name="boxIndex">格子序号名</param>
    /// <param name="cardData">进行攻击的卡牌数据</param>
    public void PlayerAttack(string boxIndex,CardData cardData)
    {
        string[] row_col =  boxIndex.Split('-');
        int row = int.Parse(row_col[0]) - 1;//数组号与格子号差1
        int col = int.Parse(row_col[1]) - 1;//数组号与格子号差1
        string str;
        CardType type;
        CheckAttackType(cardData, row, col, out str, out type);
        if (type == CardType.AttackCard)
        {
            EventManager.Instance.UpdateBattleInfo("玩家使用了" + str + "攻击卡");
        }
        else if (type == CardType.TerrainCard)
        {
            EventManager.Instance.UpdateBattleInfo("玩家使用了地形卡:" + str);
        }
    }

    /// <summary>
    /// 敌方攻击
    /// </summary>
    /// <param name="cardData"></param>
    [Hotfix]
    public void EnemyAttack(CardData cardData)
    {
        int row = -1;
        int col = -1;

        if (AI_Difficulty == 0)
        {
            for (int i = 0; i < AI_Difficulty; i++)
            {
                row = Random.Range(0, 2);
                col = Random.Range(0, 2);
                if (CardManager.Instance.elementToTerrainType[cardData.eType] != CanvasUI.battleBoxes[row, col].GetTerrainType)
                {
                    break;
                }
            }
        }
        else
        {
            int bestRow = -1;
            int bestCol = -1;
            //查找克制最优解
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (CardManager.Instance.restraintTable[cardData.eType] == CanvasUI.battleBoxes[i, j].GetTerrainType)
                    {
                        if (CanvasUI.battleBoxes[i, j]._Camp == BattleBox.Camp.my)
                        {
                            bestRow = i;
                            bestCol = j;
                        }
                        else
                        {
                            row = i;
                            col = j;
                        }
                        break;
                    }
                }
            }
            //若无最优解，则找次优解
            if (row == -1)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (CardManager.Instance.elementToTerrainType[cardData.eType] != CanvasUI.battleBoxes[i, j].GetTerrainType)
                        {
                            if (CanvasUI.battleBoxes[i, j]._Camp == BattleBox.Camp.my)
                            {
                                bestRow = i;
                                bestCol = j;
                            }
                            else
                            {
                                row = i;
                                col = j;
                            }
                            break;
                        }
                    }
                }
            }
            if (bestRow != -1)
            {
                row = bestRow;
                col = bestCol;
            }
        }
        //若无解则随机选定
        if (row == -1)
        {
            row = Random.Range(0, 2);
            col = Random.Range(0, 2);
        }
        string str;
        CardType type;
        CheckAttackType(cardData,row,col,out str,out type);
        if (type == CardType.AttackCard)
        {
            EventManager.Instance.UpdateBattleInfo("敌方使用了" + str + "攻击卡");
        }
        else if (type == CardType.TerrainCard)
        {
            EventManager.Instance.UpdateBattleInfo("敌方使用了地形卡:" + str);
        }
    }

    /// <summary>
    /// 检测攻击类型，输出属性种类及卡片类型
    /// </summary>
    /// <param name="cardData"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="str"></param>
    /// <param name="type"></param>
    [Hotfix]
    void CheckAttackType(CardData cardData,int row,int col,out string str,out CardType type)
    {
        str = "";
        type = CardType.none;
        switch (cardData.cTpye)
        {
            case CardType.AttackCard:
                type = CardType.AttackCard;
                switch (cardData.eType)
                {
                    case ElementType.Fire:
                        str = "火属性";
                        AudioManager.Instance.PlayVoice(attackSoundEffect[0]);
                        break;
                    case ElementType.Wind:
                        str = "风属性";
                        AudioManager.Instance.PlayVoice(attackSoundEffect[1]);
                        break;
                    case ElementType.Water:
                        str = "水属性";
                        AudioManager.Instance.PlayVoice(attackSoundEffect[2]);
                        break;
                    case ElementType.Soil:
                        str = "地属性";
                        AudioManager.Instance.PlayVoice(attackSoundEffect[3]);
                        break;
                }
                
                AttackAlgo(row, col, cardData.eType, cardData.dam);
                break;
            case CardType.TerrainCard:
                type = CardType.TerrainCard;
                switch (CardManager.Instance.elementToTerrainType[cardData.eType])
                {
                    case TerrainType.desert:
                        str = "火"; break;
                    case TerrainType.forest:
                        str = "风"; break;
                    case TerrainType.lake:
                        str = "水"; break;
                    case TerrainType.mountain:
                        str = "土"; break;
                }
                
                CanvasUI.battleBoxes[row, col].SetTerrain(CardManager.Instance.elementToTerrainType[cardData.eType]);
                AudioManager.Instance.PlayVoice(terrainSoundEffect);
                break;
        }
    }

    /// <summary>
    /// 攻击卡伤害算法
    /// </summary>
    /// <param name="row">行号</param>
    /// <param name="col">列号</param>
    /// <param name="elementType">使用卡的元素类型</param>
    /// <param name="dam">伤害值</param>
    [Hotfix]
    void AttackAlgo(int row,int col,ElementType elementType,float dam)
    {
        TerrainType terrainType = CanvasUI.battleBoxes[row, col].GetTerrainType;
        if (CardManager.Instance.elementToTerrainType[elementType] == terrainType)//同类属性无伤害
        {
            AnimationManager.Instance.PlayDefenseAnime(CanvasUI.battleBoxes[row, col].transform.position);
            AudioManager.Instance.PlayVoice(defenseSoundEffect);
        }
        else if (CardManager.Instance.restraintTable[elementType] == terrainType)//克制属性正常伤害
        {

            AnimationManager.Instance.PlayAttackAnime(elementType, dam, CanvasUI.battleBoxes[row, col].transform.position);
            CheckBoxWithDamge(row,col,dam);
            //遍历行检测同属性
            for (int i = row-1; i >= 0; i--)
            {
                if (terrainType == CanvasUI.battleBoxes[i, col].GetTerrainType)
                {
                    AnimationManager.Instance.PlayAttackAnime(elementType, dam, CanvasUI.battleBoxes[i, col].transform.position);
                    CheckBoxWithDamge(i, col, dam);
                }
                else
                { break; }
            }
            for (int i = row+1; i < 3; i++)
            {
                if (terrainType == CanvasUI.battleBoxes[i, col].GetTerrainType)
                {
                    AnimationManager.Instance.PlayAttackAnime(elementType, dam, CanvasUI.battleBoxes[i, col].transform.position);
                    CheckBoxWithDamge(i, col, dam);
                }
                else
                { break; }
            }
            //遍历列检测同属性
            for (int j = col-1; j >= 0; j--)
            {
                if (terrainType == CanvasUI.battleBoxes[row, j].GetTerrainType)
                {
                    AnimationManager.Instance.PlayAttackAnime(elementType, dam, CanvasUI.battleBoxes[row, j].transform.position);
                    CheckBoxWithDamge(row, j, dam);
                }
                else
                { break; }
            }
            for (int j = col+1; j < 3; j++)
            {
                if (terrainType == CanvasUI.battleBoxes[row, j].GetTerrainType)
                {
                    AnimationManager.Instance.PlayAttackAnime(elementType, dam, CanvasUI.battleBoxes[row, j].transform.position);
                    CheckBoxWithDamge(row, j, dam);
                }
                else
                { break; }
            }
            
        }
        else//非克制属性伤害减半
        {
            AnimationManager.Instance.PlayAttackAnime(elementType, dam * 0.5f, CanvasUI.battleBoxes[row, col].transform.position);
            CheckBoxWithDamge(row, col, dam * 0.5f);
        }
    }

    void CheckBoxWithDamge(int row,int col,float dam)
    {
        if (battleSatge == BattleStage.PlayerUseCard)
        {
            EventManager.Instance.PlayerAttack(row, col, dam);
        }
        if (battleSatge == BattleStage.EnemyUseCard)
        {
            EventManager.Instance.EnemyAttack(row,col,dam);
        }
    }

    /// <summary>
    /// 等待开场动画
    /// </summary>
    /// <param name="playableDirector"></param>
    /// <returns></returns>
	public IEnumerator ReadyStartBattle(PlayableDirector playableDirector)
	{
		while (playableDirector.state == PlayState.Playing) {
			yield return new WaitForEndOfFrame ();
		}
		Debug.Log ("ReadyStartBattle Finish");
		EventManager.Instance.InitFinishedEvent();
        NextStage();
	}

	/// <summary>
	/// 前往下个回合阶段
	/// </summary>
	public void NextStage()
	{
		battleSatge++;
		if ((int)battleSatge >= stageCount) {
			battleSatge = (BattleStage)1;
		}
		EventManager.Instance.GotoNewStageEvent (battleSatge);
	}

    
    /// <summary>
    /// 重开对战
    /// </summary>
    public void RestartBattle()
	{
		EventManager.Instance.InitFinishedEvent();
	}

    /// <summary>
    /// 重置
    /// </summary>
    public void ToReset()
    {
        playerExtraDamage = 0;
        enemyExtraDamage = 0;
        GameOver = false;
        battleSatge = BattleStage.None;
    }
}
