using UnityEngine;

//场景单例，退出场景时会销毁，不设置DontDestroyOnLoad属性
public class SceneMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T m_instance;
	private static object locker = new object ();

	public static T Instance {
		get {
			lock (locker) {
				if (m_instance == null) {
					T[] objs = FindObjectsOfType <T> ();
					if (objs.Length >= 1) {
						m_instance = objs [0];
						//多余的删除
						for (int i = 1; i < objs.Length; i++) {
							Destroy (objs [i]);
						}
					}
					if (m_instance == null) {
						GameObject singleton = new GameObject ();
						m_instance = singleton.AddComponent<T> ();
						singleton.name = "[Singleton]" + typeof(T).ToString ();
					}
				}
				return m_instance;
			}
		}
	}

	protected void OnDestroy ()
	{
		CleanUp ();
		if (m_instance == (this as T)) {
			m_instance = null;
		}
	}

	protected void Awake ()
	{
		Initialize ();
		if (m_instance == null) {
			m_instance = this as T;
		} else if (m_instance != this) {
			Debug.LogError (string.Format ("在场景中存在多个单例[{0}]", typeof(T).ToString ()));
			Destroy (this);
		}
	}

	//初始化放这里
	protected virtual void Initialize()
	{
		
	}

	//清理放这里
	protected virtual void CleanUp()
	{

	}

    //实例是否存在
    public static bool IsExisted
    {
        get { return m_instance != null; }
    }
}