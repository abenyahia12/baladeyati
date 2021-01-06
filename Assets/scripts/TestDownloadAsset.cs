using System.Collections;
using UnityEngine;
using Paroxe.PdfRenderer;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
public class TestDownloadAsset : MonoBehaviour
{
	public PDFViewer m_PDFviewer = null;
	public Text text;
	float m_CurrentValue = 0;
	public PDFController m_DC;
	public string m_fileName = "perf(m).pdf";
	public void StartDownload()
	{
		StartCoroutine(DownloadBytes());
	}
	public static void CleanCache()
	{
		DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath + "/" + "DownloadedDocuments");
		if (!dirInf.Exists)
		{
			Debug.Log("Creating subdirectory");
			dirInf.Create();
		}
		else
		{
			foreach (string directory in Directory.GetDirectories(Application.persistentDataPath + "/" + "DownloadedDocuments"))
			{
				DirectoryInfo data_dir = new DirectoryInfo(directory);
				data_dir.Delete(true);
			}

			foreach (string file in Directory.GetFiles(Application.persistentDataPath + "/" + "DownloadedDocuments"))
			{
				FileInfo file_info = new FileInfo(file);
				file_info.Delete();
			}
		}
	}

	IEnumerator DownloadBytes()
	{
		string urlFileName = WWW.EscapeURL(m_fileName);
		string url = "http://52.209.217.87/asset_bundles/" + urlFileName;

		UnityWebRequest www = UnityWebRequest.Get(url);

		//to test caching system
		CleanCache();
		// check if the download document exists , if not create it
		DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath + "/" + "DownloadedDocuments");
		if (!dirInf.Exists)
		{
			Debug.Log("Creating subdirectory");
			dirInf.Create();
		}

		//check if file exists then don't download it
		if (File.Exists(Application.persistentDataPath + "/DownloadedDocuments/" + m_fileName))
		{
			print(Application.persistentDataPath);
			m_PDFviewer.LoadDocumentFromBuffer(GetPDFBytes(), "");
			Debug.Log("it exists!");
		}
		else
		{

			Debug.Log("it doesn't");

			www.SendWebRequest();
			while (!www.isDone)
			{
				m_CurrentValue = www.downloadProgress * 100;
				text.text = m_CurrentValue.ToString();
				//m_DC.UpdateDownloadStatus(m_CurrentValue);
				Debug.Log(m_CurrentValue.ToString());
				yield return null;
			}

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);

			}
			else
			{
				string cachedFileName = Application.persistentDataPath + "/DownloadedDocuments/" + m_fileName;

				// Encryption will happen here

				File.WriteAllBytes(cachedFileName, www.downloadHandler.data);
			}

			m_PDFviewer.LoadDocumentFromBuffer(GetPDFBytes(), "");
		}


		yield return null;

	}

	// This will be called by whatever opens the PDFViewer and will call m_PDFviewer.LoadDocumentWithBuffer(bytes, "") with the bytes returned by this
	byte[] GetPDFBytes()
	{
		byte[] pdfBytes = File.ReadAllBytes(Application.persistentDataPath + "/DownloadedDocuments/" + m_fileName);

		// Decryption will happen here

		return pdfBytes;
	}
}