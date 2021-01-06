using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RenewToken : MonoBehaviour
{
	public string m_newTokenRequestURL;
	[Header("Min Remaining Time (s)")]
	public long m_renewTokenTime;

	private void Start()
	{
		string tokenString = PlayerPrefs.GetString("token");

		// Request new token if about to expire
		if (TokenUtils.TimeToExpiry(tokenString) < m_renewTokenTime)
		{
			StartCoroutine(RequestNewToken());
		}
	}

	public IEnumerator RequestNewToken()
	{
		UnityWebRequest www = UnityWebRequest.Get(m_newTokenRequestURL);
		www.SetRequestHeader("Authorization", " Bearer " + PlayerPrefs.GetString("token"));
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			yield break;
		}

		string jsonData = Encoding.UTF8.GetString(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
		JSONObject json = new JSONObject(jsonData);

		if (json.IsNull)
		{
			yield break;
		}

		string token = "";
		json.GetField(ref token, "token");
		PlayerPrefs.SetString("token", token);
	}
}
