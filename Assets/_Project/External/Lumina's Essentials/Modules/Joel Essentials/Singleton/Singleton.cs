#region
using Lumina.Essentials.Modules;
using UnityEngine;
#endregion

namespace Lumina.Essentials.Modules.Singleton
{
[DefaultExecutionOrder(-2)]

public class Singleton<T> : MonoBehaviour
		where T : Component
{
	static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				// In Editor, the instance is found by searching for the object as it is normally set in playmode.
				if (!Application.isPlaying && Application.isEditor)
				{
					instance = (T) Helpers.Find(typeof(T));
					return instance;
				}

				Debug.LogError("No object of type " + typeof(T).FullName + " was found.");
				return null;
			}

			return instance;
		}
	}

	/// <summary>
	/// <remarks> <b>You must make sure to override this function if your derived class uses Awake!!</b> </remarks>
	/// </summary>
	protected virtual void Awake()
	{
		if (instance == null) instance = this as T;
		else Destroy(gameObject);
	}

	protected virtual void OnDestroy()
	{
		if (instance == this) instance = null;
	}
}

/// <summary>
///     Allows you to create a singleton that persists through scenes by inheriting from this class.
/// </summary>
/// <typeparam name="T"> The class to turn into a singleton. </typeparam>
public class SingletonPersistent<T> : MonoBehaviour
		where T : Component
{
	static T instance;

	public static T Instance
	{
		get
		{
			if (Application.isEditor)
			{
				instance = (T) Helpers.Find(typeof(T));
				return instance;
			}

			if (instance == null)
			{
				Debug.LogError("No object of type " + typeof(T).FullName + " was found.");
				return null;
			}

			return instance;
		}
	}

	/// <summary>
	/// <remarks> <b>You must make sure to override this function if your derived class uses Awake!!</b> </remarks>
	/// </summary>
	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;

			if (transform.parent == null) DontDestroyOnLoad(gameObject);
			else DontDestroyOnLoad(gameObject.transform.parent);
		}
		else { Destroy(gameObject); }
	}

	void OnDestroy()
	{
		if (instance == this) instance = null;
	}
}

}