using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 用于对主面板UI进行操作
/// Create:2018/10
/// Last Edit:2019/1/7
/// </summary>
public class CanvasUI : MonoBehaviour {

	public GameObject playerOverTurnButton;
    public GameObject dorpButton;
    public Text battleInfoText;
    public GameObject vectoryUI;
    public GameObject failUI;
    public GameObject focusPlane;
    public Transform firstHandCardTrans;
    public Transform lastHandCardTrans;
    public Transform cardBornPos;
    public AudioClip vectoryAudio;
    public AudioClip failAudio;
    public AudioClip getCardAudio;
    public static BattleBox[,] battleBoxes = new BattleBox[3,3];//九宫格矩阵
    [HideInInspector]
    public Vector3[] preinstallPointForHandCard = new Vector3[6];//预设手牌位置点，手牌不大于6时使用
    [HideInInspector]
    public static Transform trans;

    private float getCardTime = 1;

    #region Unity
    void Start () {
        trans = GetComponent<Transform>();
        //初始化手牌预设点
        preinstallPointForHandCard[0] = firstHandCardTrans.localPosition;
        preinstallPointForHandCard[5] = lastHandCardTrans.localPosition;
        Vector3 delat = (lastHandCardTrans.localPosition - firstHandCardTrans.localPosition) / (6 - 1);
        for (int i = 1; i < 5; i++)
        {
            preinstallPointForHandCard[i] = firstHandCardTrans.localPosition + delat * i;
        }

		UIManager.Instance.uiDictionary.Add (UIManager.UiArea.CanvasRoot, gameObject);
		UIManager.Instance.uiDictionary.Add (UIManager.UiArea.OverPlayerTurnButton,playerOverTurnButton);
        UIManager.Instance.uiDictionary.Add (UIManager.UiArea.DorpButton, dorpButton);
        EventManager.Instance.PlayerGetCard += PlayerGetCardAnime;
        EventManager.Instance.UpdateBattleInfo += UpdateBattleInfo;
        EventManager.Instance.PlayerAttackEvent += AttackTrigger;
        EventManager.Instance.EnemyAttackEvent += AttackTrigger;
        EventManager.Instance.battleVectoryEvent += ShowVectoryUI;
        EventManager.Instance.battleFailEvent += ShowFailUI;
        GetBattleBox();
       
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.PlayerGetCard -= PlayerGetCardAnime;
        EventManager.Instance.UpdateBattleInfo -= UpdateBattleInfo;
        EventManager.Instance.PlayerAttackEvent -= AttackTrigger;
        EventManager.Instance.EnemyAttackEvent -= AttackTrigger;
        EventManager.Instance.battleVectoryEvent -= ShowVectoryUI;
        EventManager.Instance.battleFailEvent -= ShowFailUI;
    }
    #endregion

    /// <summary>
    /// 触发攻击
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="dam"></param>
    void AttackTrigger(int row,int col,float dam)
    {
        battleBoxes[row, col].HP -= dam;
    }
    
    /// <summary>
    /// 获取九宫格
    /// </summary>
    public void GetBattleBox()
    {
        BattleBox[] box = GetComponentsInChildren<BattleBox>();
        int cnt = 0;
        for (int i = 0; i <= battleBoxes.Rank; i++)
        {
            for (int j = 0; j < battleBoxes.GetLength(0); j++)
            {
                battleBoxes[i, j] = box[cnt];
                cnt++;
                //Debug.Log(string.Format("九宫格数：{0}-{1}  {2}",i,j,cnt));
            }
        }
    }

    /// <summary>
    /// 设置九宫格
    /// </summary>
    /// <param name="elementType"></param>
    public void SetBoxTerrain(ElementType elementType)
    {
        int row = Random.Range(0, 2);
        int col = Random.Range(0, 2);
        switch (elementType)
        {
            case ElementType.Fire:
                battleBoxes[row, col].SetTerrain(TerrainType.desert);
                break;
            case ElementType.Soil:
                battleBoxes[row, col].SetTerrain(TerrainType.mountain);
                break;
            case ElementType.Water:
                battleBoxes[row, col].SetTerrain(TerrainType.lake);
                break;
            case ElementType.Wind:
                battleBoxes[row, col].SetTerrain(TerrainType.forest);
                break;
        }
        
    }
    
    /// <summary>
    /// 玩家获取卡牌UI
    /// </summary>
    /// <param name="cardDatas"></param>
    public void PlayerGetCardAnime(CardData[] cardDatas)
    {
        
        for (int i = 0; i < cardDatas.Length; i++)
        {
#if UNITY_EDITOR
            string path = string.Format("Card/{0}", cardDatas[i].cTpye.ToString());
            Object card = Resources.Load(path);

#elif UNITY_ANDROID
            string path = string.Format("Card/{0}", cardDatas[i].cTpye.ToString());
            Object card = Resources.Load(path);
#endif

            GameObject cardClone = Instantiate(card) as GameObject;
            cardClone.transform.SetParent(trans);
            cardClone.transform.localPosition = cardBornPos.localPosition;
            //cardClone.transform.localPosition = cardBornPos.localPosition;//暂时注释 后面要做触发动画
            AudioManager.Instance.PlayVoice(getCardAudio);
            if (cardDatas[i].cTpye == CardType.AttackCard)
            {
                cardClone.GetComponent<Card>().SetCardProperty(cardDatas[i].cTpye, cardDatas[i].eType, cardDatas[i].dam + GameManager.Instance.playerExtraDamage);
            }
            else if (cardDatas[i].cTpye == CardType.TerrainCard)
            {
                cardClone.GetComponent<Card>().SetCardProperty(cardDatas[i].cTpye, cardDatas[i].eType);
            }
            UIManager.Instance.playerHandCardUI.Add(cardClone);
        }
        CardUISort();
        
    }

    /// <summary>
    /// 手牌排序
    /// </summary>
    /// <param name="n"></param>
    public void CardUISort()
    {
        StartCoroutine(IECardUISort());
    }

    IEnumerator IECardUISort()
    {
        int n = UIManager.Instance.playerHandCardUI.Count;
        float actionTime = getCardTime;
        if (GameManager.Instance.battleSatge == GameManager.BattleStage.PlayerUseCard)
        { actionTime = 0.2f; }
        if (n <= 6)//手牌数不大于6的情况
        {
            for (int i = 0; i < n; i++)
            {
                UIManager.Instance.playerHandCardUI[i].transform.DOLocalMove(preinstallPointForHandCard[i], actionTime);
            }
        }
        else
        {
            Vector3 delat = (lastHandCardTrans.localPosition - firstHandCardTrans.localPosition) / (n - 1);
            for (int i = 0; i < n; i++)
            {
                UIManager.Instance.playerHandCardUI[i].transform.DOLocalMove(firstHandCardTrans.localPosition + delat * i, actionTime);
            }
        }
        if (GameManager.Instance.battleSatge != GameManager.BattleStage.PlayerGetCard)
            yield break;
        yield return new WaitForSecondsRealtime(actionTime);
        GameManager.Instance.NextStage();
    }

    /// <summary>
    /// 更新战斗日志
    /// </summary>
    /// <param name="info"></param>
    public void UpdateBattleInfo(string info)
    {
        battleInfoText.GetComponent<BattleInfoUIController>().Info += ('\n' + info);
    }

    /// <summary>
    /// 显示胜利界面
    /// </summary>
    void ShowVectoryUI()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayVoice(vectoryAudio);
        focusPlane.SetActive(true);
        vectoryUI.SetActive(true);
    }

    /// <summary>
    /// 显示失败界面
    /// </summary>
    void ShowFailUI()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayVoice(failAudio);
        focusPlane.SetActive(true);
        failUI.SetActive(true);
    }

}
