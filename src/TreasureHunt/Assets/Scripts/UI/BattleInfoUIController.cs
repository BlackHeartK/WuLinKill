using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 战斗信息UI
/// </summary>
public class BattleInfoUIController : MonoBehaviour {

    //private Scrollbar scrollbar;
    private Text mText;
    private string info;
    public string Info
    {
        get { return info; }
        set
        {
            info = value;
            mText.text = value;
            transform.localPosition += new Vector3(0, 100, 0);
        }
    }

    private void Start()
    {
        mText = GetComponent<Text>();
        info = mText.text;
        //scrollbar = GetComponent<Scrollbar>();
    }

    public void OnValueChange(Vector2 v)
    {
        //Debug.Log(v);
    }
}
