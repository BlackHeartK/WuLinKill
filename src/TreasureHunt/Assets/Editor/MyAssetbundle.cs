using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyAssetbundle))]
public class MyAssetbundle : EditorWindow {
	
	[MenuItem("Tools/GetAssetbundle")]
	static void AddWindows()
	{
		Rect wr=new Rect(0,0,250,50);
		MyAssetbundle windows=(MyAssetbundle)EditorWindow.GetWindowWithRect(typeof(MyAssetbundle),wr,true,"打包Assetbundel");
		windows.Show();
	}

    [MenuItem("Tools/GetAssetbundleDefault")]
    static void GetAssetbundleDefault()
    {
        try
        {
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.Android);
        }
        catch (Exception ex)
        {
            Debug.LogError("打包失败！");
        }
        Debug.Log("资源均已打包完成");
        AssetDatabase.Refresh();
    }
    
    private string bundleName;

	void OnGUI()
	{
		bundleName = EditorGUILayout.TextField ("输入保存的名字", bundleName,GUILayout.Width(200));
		if (GUILayout.Button ("打包", GUILayout.Width (100))) {
			GetAssetbundle (bundleName);
		}
	}

	void GetAssetbundle(string bundleName)
	{
		AssetBundleBuild[] abb = new AssetBundleBuild[1];
		UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object),SelectionMode.DeepAssets);
		string[] textAsset = new string[objs.Length];
		for (int i = 0; i < objs.Length; i++) {
			textAsset [i] = AssetDatabase.GetAssetPath (objs [i]);
		}
		abb[0].assetNames = textAsset;
		abb[0].assetBundleName = bundleName;
		try
		{
			BuildPipeline.BuildAssetBundles (Application.streamingAssetsPath,abb,BuildAssetBundleOptions.None,BuildTarget.Android);
		}
		catch(Exception ex) {
			Debug.Log ("打包失败！");
		}
		Debug.Log ("打包完成！");
		AssetDatabase.Refresh();
	}
}
