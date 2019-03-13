using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于对状态机系统进行管理
/// Create:2018/12
/// Last Edit:2019/1/2
/// </summary>
[RequireComponent(typeof(Enemy))]
public class AIManager : MonoBehaviour {

    public static AIFSM_System aiFSM_System;
    [HideInInspector]
    public Enemy enemy;

    public void Start()
    {
        enemy = GetComponent<Enemy>();
        aiFSM_System = new AIFSM_System();
        AIFSM[] aiFSMs = GetComponentsInChildren<AIFSM>();
        for (int i = 0; i < aiFSMs.Length; i++)
        {
            aiFSM_System.AddAIFSM(aiFSMs[i],this);
        }

        EnemyWaitState waitState = GetComponentInChildren<EnemyWaitState>();
        aiFSM_System.SetCurrentAIFSM(waitState);
        EventManager.Instance.EnemyUseCard += UseCard;
    }

    private void Update()
    {
        aiFSM_System.Update();
    }

    private void OnDestroy()
    {
        EventManager.Instance.EnemyUseCard -= UseCard;
    }

    /// <summary>
    /// AI使用卡牌
    /// </summary>
    public void UseCard()
    {
        aiFSM_System.PerformRule(AIrule.IntoAIturn);
    }

    
}
