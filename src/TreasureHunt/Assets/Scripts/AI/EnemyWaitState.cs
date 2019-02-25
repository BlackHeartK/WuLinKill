using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaitState : AIFSM
{
    void Awake()
    {
        mAIEvent = AIEvent.AIwait;
        AddAIEventAndEvent(AIrule.IntoAIturn, AIEvent.AIUseCard);
    }
}
