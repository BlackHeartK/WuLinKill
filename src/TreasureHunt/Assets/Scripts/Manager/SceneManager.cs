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
	public void BackMenu()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (0,UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

    /// <summary>
    /// 跳转到主场景
    /// </summary>
    public void OpenMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    /// <summary>
    /// 跳转到教学场景
    /// </summary>
    public void OpenInstructionScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2, UnityEngine.SceneManagement.LoadSceneMode.Single);
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
