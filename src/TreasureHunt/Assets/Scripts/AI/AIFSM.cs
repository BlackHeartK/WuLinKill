using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AI状态机根级
/// Create:2018/10
/// Last Edit:2019/1/2
/// </summary>

public enum AIrule//AI规则，用于转换状态
{
    NullAIrule = 0,
    IntoAIturn,
    AICardCountIsZero,
}

public enum AIEvent//AI事件，用于执行
{
    NullEvent = 0,
    AIUseCard,
    AIwait
}

public abstract class AIFSM:MonoBehaviour {

    protected Dictionary<AIrule, AIEvent> AI_RE_FSM = new Dictionary<AIrule, AIEvent>();

    protected AIEvent mAIEvent;
    public AIEvent AiEvent
    {
        get { return mAIEvent; }
    }

    protected AIManager m_AIManager;
    public AIManager _AIManager
    {
        set { m_AIManager = value; }
    }

    /// <summary>
    /// 添加键值AI规则与事件对应关系
    /// </summary>
    /// <param name="aIrule"></param>
    /// <param name="aIEvent"></param>
    public void AddAIEventAndEvent(AIrule aIrule, AIEvent aIEvent)
    {
        if (aIrule == AIrule.NullAIrule)
        {
            Debug.LogError("AIFSM ERROR: NullAIrule 不允许对应 AIrule");
            return;
        }
        if (aIEvent == AIEvent.NullEvent)
        {
            Debug.LogError("AIFSM ERROR: NullEvent 不允许对应 AIEvent");
            return;
        }
        if (AI_RE_FSM.ContainsKey(aIrule))
        {
            Debug.LogError("AIFSM ERROR: 已经存在相同AIrule."+ aIrule.ToString() +"作为Key");
            return;
        }
        AI_RE_FSM.Add(aIrule,aIEvent);
    }

    /// <summary>
    /// 删除键值AI规则与事件对应关系
    /// </summary>
    /// <param name="aIrule"></param>
    public void DelAIEventAndEvent(AIrule aIrule)
    {
        if (AI_RE_FSM.ContainsKey(aIrule))
        {
            AI_RE_FSM.Remove(aIrule);
        }
        else
        {
            Debug.LogError("AIFSM ERROR: AI规则与事件关联中并不包含此规则");
        }
    }

    /// <summary>
    /// 获取AI规则对应事件
    /// </summary>
    /// <param name="aIrule"></param>
    /// <returns></returns>
    public AIEvent GetOutputState(AIrule aIrule)
    {
        if (AI_RE_FSM.ContainsKey(aIrule))
        {
            return AI_RE_FSM[aIrule];
        }
        return AIEvent.NullEvent;
        
    }

    public virtual void DoBeforeEntering()
    { }

    public virtual void DoBeforeLeaving()
    { }
}
