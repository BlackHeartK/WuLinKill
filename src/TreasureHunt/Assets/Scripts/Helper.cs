using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour {

    public GameObject nextBtn;
    public GameObject backBtn;
    public GameObject equipArea;
    public GameObject endTurnBtn;

    [HideInInspector]
    public List<Transform> helpPages = new List<Transform>();

    public static bool isInstructionMode;
    public static int unLockStep;
    private static int index;

    public static int Index
    {
        get { return index; }
        set
        {
            index = value;
            switch (value)
            {
                case 1:
                    CanvasUI.ShowInstructionTerrain(false);
                    break;
                case 5:
                    if (UIManager.Instance.playerHandCardUI.Count == 0)
                    {
                        GameManager.Instance.battleSatge = GameManager.BattleStage.PlayerGetCard;
                        EventManager.Instance.GotoNewStageEvent(GameManager.BattleStage.PlayerGetCard);
                    }
                    unLockStep = 1;
                    break;
                case 6:
                    if (unLockStep < 2)
                    {
                        GameManager.Instance.battleSatge = GameManager.BattleStage.PlayerGetCard;
                        EventManager.Instance.GotoNewStageEvent(GameManager.BattleStage.PlayerGetCard);
                    }
                    unLockStep = 2;
                    break;
                case 7:
                    CanvasUI.ShowInstructionTerrain(true);
                    GameManager.Instance.battleSatge = GameManager.BattleStage.PlayerGetCard;
                    EventManager.Instance.GotoNewStageEvent(GameManager.BattleStage.PlayerGetCard);
                    break;
                case 10:
                    if (unLockStep < 3)
                    {
                        GameManager.Instance.battleSatge = GameManager.BattleStage.PlayerGetCard;
                        EventManager.Instance.GotoNewStageEvent(GameManager.BattleStage.PlayerGetCard);
                    }
                    unLockStep = 3;
                    break;
                case 11:
                    if (unLockStep < 4)
                    {
                        GameManager.Instance.battleSatge = GameManager.BattleStage.PlayerGetCard;
                        EventManager.Instance.GotoNewStageEvent(GameManager.BattleStage.PlayerGetCard);
                    }
                    unLockStep = 4;
                    break;
            }
        }
    }

	void Awake () {
        isInstructionMode = true;
        nextBtn.SetActive(true);
        backBtn.SetActive(false);
        foreach (Transform trans in transform)
        {
            if (trans.name.Split(' ')[0] == "Page")
            {
                helpPages.Add(trans);
            }
        }
    }

    private void Start()
    {
        EventManager.Instance.InitFinishedEvent();
    }

    private void Update()
    {
        if (UIManager.Instance.playerHandCardUI.Count == 0 && unLockStep != 0)
        {
            GameManager.Instance.battleSatge = GameManager.BattleStage.PlayerGetCard;
            EventManager.Instance.GotoNewStageEvent(GameManager.BattleStage.PlayerGetCard);
        }
    }

    public void Next () {
        backBtn.SetActive(true);
        helpPages[Index].gameObject.SetActive(false);
        Index++;
        helpPages[Index].gameObject.SetActive(true);
        if (Index == helpPages.Count - 1)
        {
            nextBtn.SetActive(false);
        }

        if (Index == 10)
        {
            equipArea.SetActive(true);
        }
        else if (Index == 11)
        {
            endTurnBtn.SetActive(true);
        }
    }

    public void Back()
    {
        nextBtn.SetActive(true);
        helpPages[Index].gameObject.SetActive(false);
        Index--;
        helpPages[Index].gameObject.SetActive(true);
        if (Index == 0)
        {
            backBtn.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        helpPages.Clear();
        isInstructionMode = false;
        unLockStep = 0;
        index = 0;
    }
}
