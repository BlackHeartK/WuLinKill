using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景管理
/// Create:2018/9
/// Last Edit:2019/1/5
/// </summary>
public class SceneManager : Singleton<SceneManager> {

	private int sceneNum = 0;
    private int currentSceneIndex = 0;

    /// <summary>
    /// 初始化
    /// </summary>
	public void Init()
	{
        Debug.Log ("SceneM Init Finished!");
	}

    /// <summary>
    /// 跳转到下一场景
    /// </summary>
	public void NextScene()
	{
		sceneNum++;
//		Debug.Log (UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings);
		if (sceneNum >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings) {
			sceneNum = 0;
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneNum,UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

    private void Update()
    {
        if (currentSceneIndex != UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex)
        {
            EventManager.Instance.SceneChange();
            currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }
        
    }

    /// <summary>
    /// 获取当前场景编号
    /// </summary>
    /// <returns></returns>
    public int GetSceneIndex()
    {
        return sceneNum;
    }
}
