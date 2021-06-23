using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DisableDevice : MonoBehaviour
{
	public string m_disableURL = "";
    public ErrorHandler m_errorHandler;

    public void DeviceDisable()
    {
        StartCoroutine(DeviceDisableCoroutine());
    }

    private IEnumerator DeviceDisableCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(m_disableURL);
        www.SetRequestHeader("Authorization", " Bearer " + PlayerPrefs.GetString("token"));
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result== UnityWebRequest.Result.ProtocolError)
        {
            HandleErrors(www);
        }
        else
        {
            SceneManager.LoadScene("Login");
        }
    }

    private void HandleErrors(UnityWebRequest www)
    {

            if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Network Error");
        }
        else if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, www.responseCode + ": " + www.error);
        }
    }
}
