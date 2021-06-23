using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadPDFs : MonoBehaviour
{
	public string           m_documentsRequestURL;
	public string           m_userRequestURL;
	public PDFButtonHandler m_buttonManager;
	public Sprite           m_missingIcon;
	public ErrorHandler     m_errorHandler;
	public bool             m_debugLogs;
	public Text             m_loadingText;
	public bool             m_dbgLoadingText;

	private void Start()
	{
		Assert.IsNotNull(m_missingIcon);
		Assert.IsNotNull(m_errorHandler);
		StartCoroutine(Download());

		if (m_loadingText != null)
		{
			m_loadingText.enabled = true;
		}
	}

	public IEnumerator Download()
	{
		if (m_dbgLoadingText && m_loadingText != null)
		{
			m_loadingText.text = "Getting Documents...";
		}

		JSONObject documents = null;
		yield return StartCoroutine(GetDocuments(getDocuments => { documents = getDocuments; }));

		if (m_dbgLoadingText && m_loadingText != null)
		{
			m_loadingText.text = "Saving Documents...";
		}

		string filePath = Path.Combine(Application.persistentDataPath, "JSONData");

		if (documents != null)
		{
			// Save file, always save as it may have changed. Could perform check to see if it has changed?
			File.WriteAllText(filePath, documents.ToString());
		}
		else
		{
			if (!File.Exists(filePath))
			{
				m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Offline and no cached JSON");
				yield break; // Offline and no cached JSON data....nothing we can do
			}

			// Read file
			string dataAsJson = File.ReadAllText(filePath);
			documents = JSONObject.Create(dataAsJson);
		}

		if (m_debugLogs) Debug.Log("User Documents " + documents);

		yield return StartCoroutine(CreateDocuments(documents));

		if (m_dbgLoadingText && m_loadingText != null)
		{
			m_loadingText.text = "Getting School Logo...";
		}

		string iconURL = "";
		documents.GetField(ref iconURL, "schoolLogo");

		if (!string.IsNullOrEmpty(iconURL))
		{
			// Cache icon now
			yield return StartCoroutine(GetIcon(getIcon => { }, iconURL));

			PlayerPrefs.SetString("schoolLogo", iconURL);
		}

		m_buttonManager.GenerateContent();

		if (m_loadingText != null)
		{
			m_loadingText.enabled = false;
		}
	}

	private IEnumerator GetDocuments(Action<JSONObject> documents)
	{
		// Get JSON of Documents
		UnityWebRequest www = UnityWebRequest.Get(m_documentsRequestURL);
		www.SetRequestHeader("Authorization", " Bearer " + PlayerPrefs.GetString("token"));
		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
		{
			HandleErrors(www);
			yield break;
		}

		string jsonData = Encoding.UTF8.GetString(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
		JSONObject json = new JSONObject(jsonData);

		if (json.type == JSONObject.Type.NULL)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Invalid Server Response");
			yield break;
		}

		documents(json);
	}

	private IEnumerator CreateDocuments(JSONObject json)
	{
		if (m_dbgLoadingText && m_loadingText != null)
		{
			m_loadingText.text = "Creating Course Documents...";
		}

		JSONObject courses = json.GetField("courses");

		if (courses.type == JSONObject.Type.NULL)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Courses is Null");
			yield break;
		}

		if (!courses.IsArray)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Courses is not an array");
			yield break;
		}

		List<JSONObject> courseList = courses.list;

		foreach (JSONObject course in courseList)
		{
			string courseName = "";
			course.GetField(ref courseName, "name");

			JSONObject documents = course.GetField("documents");

			if (!documents.IsArray)
			{
				continue;
			}

			List<JSONObject> documentList = documents.list;

			foreach (JSONObject document in documentList)
			{
				// TODO: Don't wait for each one to finished one at a time, run multiple then wait for all to finish
				yield return StartCoroutine(CreateDocument(document, courseName));
			}
		}

		if (m_dbgLoadingText && m_loadingText != null)
		{
			m_loadingText.text = "Creating Misc Documents...";
		}

		JSONObject miscDocuments = json.GetField("documents");

		if (miscDocuments.type == JSONObject.Type.NULL)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Misc documents is Null");
			yield break;
		}

		if (!miscDocuments.IsArray)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Misc documents is not an array");
			yield break;
		}

		List<JSONObject> miscDocumentsList = miscDocuments.list;

		foreach (JSONObject miscDocument in miscDocumentsList)
		{
			// TODO: Don't wait for each one to finished one at a time, run multiple then wait for all to finish
			yield return StartCoroutine(CreateDocument(miscDocument, "Misc"));
		}

		if (m_dbgLoadingText && m_loadingText != null)
		{
			m_loadingText.text = "Creating School Documents...";
		}

		JSONObject schoolClass = json.GetField("schoolClass");

		if (schoolClass.type == JSONObject.Type.NULL)
		{
			// Apparently this is fine
			//m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "SchoolClass is Null");
			yield break;
		}

		JSONObject schoolDocuments = schoolClass.GetField("documents");

		if (schoolDocuments.type == JSONObject.Type.NULL)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "School documents is Null");
			yield break;
		}

		if (!schoolDocuments.IsArray)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "School documents is not an array");
			yield break;
		}

		string className = schoolClass["name"].ToString();
		List<JSONObject> schoolDocumentsList = schoolDocuments.list;

		foreach (JSONObject schoolDocument in schoolDocumentsList)
		{
			// TODO: Don't wait for each one to finished one at a time, run multiple then wait for all to finish
			yield return StartCoroutine(CreateDocument(schoolDocument, className));
		}
	}

	private IEnumerator CreateDocument(JSONObject documentJSONObject, string courseName)
	{
		Dictionary<string, string> document = documentJSONObject.ToDictionary();

		// Also available: id, title, description, displayName, state, createdAt, updatedAt
		PDFInfo newInfo = new PDFInfo
		{
			FileName = document["title"],
			DisplayName = document["displayName"],
			CourseName = courseName,
			Description = document["description"],
			ShowDocIcon = Convert.ToBoolean(document["hasLogo"]),
			IsDuplex = Convert.ToBoolean(document["isDuplex"])
		};

		string dirPath = Application.persistentDataPath + "/DownloadedDocuments/";
		newInfo.AlreadyLoaded = File.Exists(dirPath + newInfo.FileName);

		JSONObject documentVersion = documentJSONObject.GetField("activeVersion");

		if (documentVersion.IsNull)
		{
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "activeVersion is null");
			yield break;
		}

		yield return StartCoroutine(CreateButtonForVersion(documentVersion, newInfo));
	}

	private IEnumerator CreateButtonForVersion(JSONObject versionJSONObject, PDFInfo info)
	{
		Dictionary<string, string> version = versionJSONObject.ToDictionary();
		// Also available: id, filename, isReleaseVersion, createdAt, updatedAt
		string iconURL = version["cover"];
		info.VersionNumber = version["name"];
		info.Author = version["author"];
		string serverURL = "https://ltms.cat-europe.com";
		info.URL = serverURL + version["url"];

		if (m_debugLogs) Debug.Log(versionJSONObject);

		Sprite icon = null;
		yield return StartCoroutine(GetIcon(getIcon => { icon = getIcon; }, iconURL));

		info.Icon = icon;

		m_buttonManager.AddButton(info);
	}

	private void HandleErrors(UnityWebRequest www)
	{
		if (www.result == UnityWebRequest.Result.ConnectionError) 
		{ 
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, "Network Error");
		}
		else if (www.result == UnityWebRequest.Result.ProtocolError)
		
		{
			string jsonErrorData = Encoding.UTF8.GetString(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
			JSONObject jsonErrorObject = new JSONObject(jsonErrorData);

			// Unable to get error message from JSON, so display error code and message
			if (jsonErrorObject.type == JSONObject.Type.NULL)
			{
				m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, www.responseCode + www.error);
				return;
			}

			string message = "";
			jsonErrorObject.GetField(ref message, "message");
			m_errorHandler.DisplayError(ErrorHandler.Error.NetworkError, www.responseCode + ": " + message);
		}
	}

	private IEnumerator GetIcon(Action<Sprite> icon, string iconURL)
	{
		yield return StartCoroutine(IconHandler.GetIconFromURL(iconURL, sprite => { icon(sprite); }));

		if (icon.Target != null) yield break;

		icon(m_missingIcon);
	}
}
