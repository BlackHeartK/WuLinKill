using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class AssetCheckerManager : Singleton<AssetCheckerManager> {
    
    private void Start()
    {
        StartCoroutine(CheckNewVersion());
    }
    
    /// <summary>
    /// 检测是否有新版本
    /// </summary>
    IEnumerator CheckNewVersion()
    {
        string verTex;
        WWW local = new WWW(Application.streamingAssetsPath + "/Version.txt");
        while (!local.isDone)
        { }
        verTex = local.text;

#if UNITY_EDITOR
        string url = @"F:\WorkSpace\WulinKill\Version.txt";
#elif UNITY_ANDROID
        string url = "jar:file:///storage/emulated/0/Download/Version.txt";
#endif
        WWW server = new WWW(url);
        while (!server.isDone)
        { }
        if (server.text != verTex)
        {
            LoadAssetBundles();
            UpdateVersion(server.text);
        }
        yield break;
    }

    /// <summary>
    /// 加载AB包
    /// </summary>
    public void LoadAssetBundles()
    {
        StartCoroutine(DownloadAB());
    }

    /// <summary>
    /// 下载AB包
    /// </summary>
    /// <returns></returns>
    IEnumerator DownloadAB()
    {
        string[] abName = new string[]
        {
            "/Game/version.assetbundle",
            "Texture.assetbundle",
            "Audio.assetbundle",
            "Prefabs.assetbundle",
        };
        for (int index = 0; index < abName.Length; index++)
        {
#if UNITY_EDITOR
            string url = Application.streamingAssetsPath + abName[index];
#elif UNITY_ANDROID
        string url = "jar:file:///storage/emulated/0/Download/version.assetbundle";
#endif
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return request.SendWebRequest();

            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
            string[] name = ab.GetAllAssetNames();
            TextAsset[] textAssets = ab.LoadAllAssets<TextAsset>();
            for (int i = 0; i < textAssets.Length; i++)
            {
                //Debug.Log(name[i]);
                //Debug.Log(textAssets[i]);
                ReplaceTextAsset(name[i], textAssets[i].text);
            }
            ab.Unload(false);
        }
    }

    /// <summary>
    /// 替换文本类资源文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileContext"></param>
    void ReplaceTextAsset(string fileName,string fileContext)
    {
        string part;
        name = GetLastPartFileName(fileName,out part);
#if UNITY_EDITOR
        string fullPath = Application.dataPath + part + '/' + name;
#elif UNITY_ANDROID
        string fullPath = Application.persistentDataPath + part + '/' + name;
#endif
        Debug.Log(fullPath);
        FileInfo info = new FileInfo(fullPath); ;
        if (info.Exists)
        {
            info.Delete();
            File.WriteAllText(info.FullName, fileContext);
        }
    }

    /// <summary>
    /// 获取文件名并输出中间路径
    /// </summary>
    /// <param name="full"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    string GetLastPartFileName(string full,out string part)
    {
        string[] str = full.Split('/');
        part = "";
        for (int i = 1; i < str.Length-1; i++)
        {
            part += '/' + str[i];
        }
        return str[str.Length - 1];
    }

    /// <summary>
    /// 更新版本信息
    /// </summary>
    /// <param name="context"></param>
    void UpdateVersion(string context)
    {
        FileInfo info = new FileInfo(Application.streamingAssetsPath + "/Version.txt"); ;
        if (info.Exists)
        {
            info.Delete();
            File.WriteAllText(info.FullName, context);
        }
    }
}
