using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBranch : MonoBehaviour
{
	public string m_loginScene;
	public string m_contentsScene;

	public void Start()
	{
		// Check if token exists
		if (!PlayerPrefs.HasKey("token"))
		{
			SceneManager.LoadScene(m_loginScene);
			return;
		}

		// Check if token is still valid
		string tokenString = PlayerPrefs.GetString("token");

		if (TokenUtils.HasTokenExpired(tokenString))
		{
			SceneManager.LoadScene(m_loginScene);
			return;
		}

		SceneManager.LoadScene(m_contentsScene);
	}
}
