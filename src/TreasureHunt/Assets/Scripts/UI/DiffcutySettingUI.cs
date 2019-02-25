using UnityEngine;
using UnityEngine.UI;

public class DiffcutySettingUI : MonoBehaviour {

    public Text mText;

    /// <summary>
    /// 当困难度发生变化
    /// </summary>
    /// <param name="value"></param>
    public void OnDiffcultyChange(float value)
    {
        if (value < 0.5f)
        {
            mText.text = "Normal";
            GameManager.Instance.AI_Difficulty = 0;
        }
        else
        {
            mText.text = "Hard";
            GameManager.Instance.AI_Difficulty = 1;
        }
    }
}
