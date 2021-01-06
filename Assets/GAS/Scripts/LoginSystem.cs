using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
#if (UNITY_STANDALONE || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX) && (!UNITY_WEBPLAYER && !UNITY_WEBGL)
using System.Net.NetworkInformation;
#endif

public class LoginSystem : MonoBehaviour
{
	[SerializeField]
	private InputField userName;
	[SerializeField]
	private InputField passwordName;
	[SerializeField]
	public string LoginUrl;
	[SerializeField]
	private Text WarningMsg;

	public void Login()
	{
		Login(userName.text, passwordName.text);
	}

	public void Login(string userName, string password)
	{
		StartCoroutine(Query_Account(userName, password));
	}

	IEnumerator Query_Account(string userName, string password)
	{
		WarningMsg.text = "Logging in... ";

		WWWForm form = new WWWForm();
		form.AddField("login", userName);
		form.AddField("password", password);
		form.AddField("deviceId", SystemInfo.deviceUniqueIdentifier);
		form.AddField("deviceKey", ("jkgojraeipgja;m" + SystemInfo.deviceUniqueIdentifier + "u9i0p5[r43189u5rjkl;nfgva").GetHashCode());

		UnityWebRequest www = UnityWebRequest.Post(LoginUrl, form);
		yield return www.SendWebRequest();

		WarningMsg.text = "Please Wait ... ";
		yield return www;

		if (www.isNetworkError)
		{
			WarningMsg.text = www.error;
		}
		else if (www.isHttpError)
		{
			if (www.responseCode == 404)
			{
				WarningMsg.text = "404 Not found";
				yield break;
			}

			if (www.responseCode == 405)
			{
				WarningMsg.text = "405 Not Allowed";
				yield break;
			}

			string jsonData = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
			JSONObject json = new JSONObject(jsonData);

			if (json == null)
			{
				WarningMsg.text = www.error;
				yield break;
			}

			string message = "";
			json.GetField(ref message, "message");
			WarningMsg.text = message;
		}
		else
		{
			string jsonData = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
			JSONObject json = new JSONObject(jsonData);
			string token = "";

			if (json == null)
			{
				WarningMsg.text = "Invalid server response";
				yield break;
			}

			json.GetField(ref token, "token");
			PlayerPrefs.SetString("token", token);

			SceneManager.LoadScene("Contents");
		}
	}

    public void SkipLogin()
	{
		SceneManager.LoadScene("Contents");
	}

    
}

