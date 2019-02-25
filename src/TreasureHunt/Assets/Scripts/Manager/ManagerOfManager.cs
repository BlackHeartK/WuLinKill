using UnityEngine;

/// <summary>
/// 总管理 
/// Create:2018/9
/// Last Edit Data:2019/1/6
/// </summary>
public class ManagerOfManager : MonoBehaviour {

    public bool testMode;

	void Awake()
	{
        if (!testMode)
        {
            GameObject mo = GameObject.Find("Manager");
            if (mo != gameObject)
            {
                Destroy(gameObject);
                return;
            }
        }
		DontDestroyOnLoad (this.gameObject);
		Init ();
	}

    /// <summary>
    /// 总管理初始化
    /// </summary>
    void Init()
	{
        EventManager.Instance.Init();//初始化事件管理
        GameManager.Instance.Init();//初始化游戏管理
        CardManager.Instance.Init ();//获取卡牌数据
		UIManager.Instance.Init();//初始化UI界面管理
        AnimationManager.Instance.Init();//初始化动画管理
        AudioManager.Instance.Init();//初始化声音管理
	}
}
