using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI状态机系统
/// Create:2018/11
/// Last Edit:2019/1/2
/// </summary>
public class AIFSM_System {

    private List<AIFSM> aiFSMs;

    private AIEvent currentAIEvent;
    public AIEvent CurrentAIEvent
    {
        get { return currentAIEvent; }
    }

    private AIFSM currentAIFSM;
    public AIFSM CurrentAIFSM
    {
        get { return currentAIFSM; }
    }

    public AIFSM_System()
    {
        aiFSMs = new List<AIFSM>();
    }

    /// <summary>
    /// 设置当前状态
    /// </summary>
    /// <param name="aiFSM"></param>
    public void SetCurrentAIFSM(AIFSM aiFSM)
    {
        currentAIFSM = aiFSM;
        currentAIEvent = aiFSM.AiEvent;
        CurrentAIFSM.DoBeforeEntering();
    }

    /// <summary>
    /// 添加一个新状态
    /// </summary>
    /// <param name="aiFSM"></param>
    public void AddAIFSM(AIFSM aiFSM)
    {
        if (aiFSM == null)
        {
            Debug.LogError("AddAIFSM Error:为空的AIFSM不被允许添加！");
            return;
        }

        for (int i = 0; i < aiFSMs.Count; i++)
        {
            if (aiFSMs[i].AiEvent == aiFSM.AiEvent)
            {
                Debug.LogError("AddAIFSM Error:" + aiFSM.AiEvent.ToString()+"在列表中已存在！");
                return;
            }
        }

        aiFSMs.Add(aiFSM);
    }

    /// <summary>
    /// 删除一个状态
    /// </summary>
    /// <param name="aiEvent"></param>
    public void DeleteAIFSM(AIEvent aiEvent)
    {
        if (aiEvent == AIEvent.NullEvent)
        {
            Debug.LogError("DeleteAIFSM Error:NullEvent");
            return;
        }
        for (int i = 0; i < aiFSMs.Count; i++)
        {
            if (aiFSMs[i].AiEvent == aiEvent)
            {
                aiFSMs.RemoveAt(i);
                return;
            }
        }
        Debug.LogError("DeleteAIFSM Error:" + aiEvent.ToString() + "不存在于状态集列表中！");
    }

    /// <summary>
    /// 转换状态
    /// </summary>
    /// <param name="aiRule"></param>
    public void PerformRule(AIrule aiRule)
    {
        if (aiRule == AIrule.NullAIrule)
        {
            Debug.LogError("PerformRule Error:NullAIrule");
            return;
        }

        AIEvent aiEvent = currentAIFSM.GetOutputState(aiRule);
        if (aiEvent == AIEvent.NullEvent)
        {
            Debug.LogError("PerformRule Error:NullEvent");
            return;
        }

        currentAIEvent = aiEvent;
        for (int i = 0; i < aiFSMs.Count; i++)
        {
            if (aiFSMs[i].AiEvent == currentAIEvent)
            {
                currentAIFSM.DoBeforeLeaving();
                currentAIFSM = aiFSMs[i];
                currentAIFSM.DoBeforeEntering();
                return;
            }
        }
    }

    public void Update()
    {
        //Debug.Log(currentAIEvent);
    }
}
