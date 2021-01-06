using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SignOut : MonoBehaviour
{
	public string m_documentsRequestURL;

    public void SignOutFunction()
	{
		StartCoroutine(SendSignOutRequest());
	}

	IEnumerator SendSignOutRequest()
	{
		UnityWebRequest www = UnityWebRequest.Get(m_documentsRequestURL);
		www.SetRequestHeader("Authorization", " Bearer " + PlayerPrefs.GetString("token"));
		yield return www.SendWebRequest();
		SceneManager.LoadScene("Login");
	}
}
