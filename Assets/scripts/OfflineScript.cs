using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
public class OfflineScript : MonoBehaviour
{
	void Start()
	{
		string filePath = Path.Combine(Application.persistentDataPath, "JSONData");
		if (!File.Exists(filePath))
		{
			gameObject.SetActive(false);
		}

	}
	public void GoToContentScene()
	{
		SceneManager.LoadScene("Contents");
	}
}
