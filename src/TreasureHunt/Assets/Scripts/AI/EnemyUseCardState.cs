using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUseCardState : AIFSM
{
    void Awake()
    {
        mAIEvent = AIEvent.AIUseCard;
        AddAIEventAndEvent(AIrule.AICardCountIsZero, AIEvent.AIwait);
    }

    public override void DoBeforeEntering()
    {
        StartCoroutine(UseCard());
    }

    public override void DoBeforeLeaving()
    {
        EventManager.Instance.UpdateBattleInfo("敌方回合结束");
    }

    IEnumerator UseCard()
    {
        while (m_AIManager.enemy.handCards.Count > 0)
        {
            m_AIManager.enemy.UseCard();
            yield return new WaitForSeconds(1.0f);
            if (GameManager.Instance.GameOver)
            {
                yield break;
            }
        }
        m_AIManager.enemy.UseCard();
    }
}

