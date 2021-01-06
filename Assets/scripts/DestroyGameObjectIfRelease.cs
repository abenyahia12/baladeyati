using UnityEngine;

public class DestroyGameObjectIfRelease : MonoBehaviour
{
	void Start()
	{
		if (!Debug.isDebugBuild)
		{
			Destroy(gameObject);
		}
	}
}
