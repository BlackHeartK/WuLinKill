using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:new()
{

	private static T _Instance;

	public static T Instance
	{
		get{
			if (_Instance == null) {
				_Instance = new T();
			}
			return _Instance;
		}
	}
}
