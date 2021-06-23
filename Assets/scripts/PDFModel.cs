using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paroxe.PdfRenderer;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text;

public class PDFInfo
{
	public string DisplayName;
	public string URL;
	public string FileName;
	public bool AlreadyLoaded;
	public string CourseName;
	public Sprite Icon;
	public string VersionNumber;
	public string Author;
	public string Description;
	public bool ShowDocIcon;
	public bool IsDuplex;
}

public class PDFModel : MonoBehaviour
{
	public bool m_downloading;
	public float m_downloadProgress; // between 0 and 1

	public string FileName { get { return Info.FileName; } }

	public PDFInfo Info { set; get; }
	public string CourseName { get { return Info.CourseName; } }

	public bool m_DebugAllowed;

	public PDFButtonView m_SelfButton;
	public PDFController m_DC;

	void Start()
	{
		m_SelfButton = GetComponent<PDFButtonView>();
		m_DC = GetComponent<PDFController>();
	}


	public void Uninstall()
	{
		File.Delete(Application.persistentDataPath + "/" + "DownloadedDocuments/" + FileName);
		Info.AlreadyLoaded = false;
		m_downloading = false;
		m_DC.FinishUninstall();
	}

	public bool HasDownloaded()
	{
		return Info.AlreadyLoaded;
	}

	public bool IsDownloading()
	{
		return m_downloading;
	}
	public float GetDownloadProgress()
	{
		return m_downloadProgress;
	}


	public void StartDownload()
	{
		m_downloading = true;
		StartCoroutine(DownloadBytes());
	}
	public void CleanCache()
	{
		DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath + "/" + "DownloadedDocuments");
		if (!dirInf.Exists)
		{
			if (m_DebugAllowed)
			{
				Debug.Log("Creating subdirectory");
			}
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
		UnityWebRequest www = UnityWebRequest.Get(Info.URL);
		www.SetRequestHeader("Authorization", " Bearer " + PlayerPrefs.GetString("token"));

		//to test caching system
		//CleanCache();
		// check if the download document exists , if not create it
		DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath + "/" + "DownloadedDocuments");
		if (!dirInf.Exists)
		{
			if (m_DebugAllowed)
			{
				Debug.Log("Creating subdirectory");
			}
			dirInf.Create();
		}

		//check if file exists then don't download it
		if (File.Exists(Application.persistentDataPath + "/DownloadedDocuments/" + FileName))
		{
			m_downloading = false;
			m_DC.FinishDownload();
			Info.AlreadyLoaded = true;
			if (m_DebugAllowed)
			{
				Debug.Log("it exists!");
			}
		}
		else
		{
			if (m_DebugAllowed)
			{
				Debug.Log("it doesn't");
			}
			www.SendWebRequest();
			while (!www.isDone)
			{
				m_downloadProgress = www.downloadProgress;
				m_DC.UpdateDownloadStatus(m_downloadProgress);
				yield return null;
			}
			
			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				m_DC.SummonErrorWindow("Network or HTTP Error: " + www.responseCode + " " + www.error);
				Info.AlreadyLoaded = false;
				m_downloading = false;
				m_SelfButton.SwapIconToFailed();

			}
			else
			{
				string cachedFileName = Application.persistentDataPath + "/DownloadedDocuments/" + FileName;

				// Encryption will happen here
				byte[] pdfBytes = www.downloadHandler.data;;
				byte[] EncryptedpdfBytes = Helper.Encrypt(pdfBytes);
				File.WriteAllBytes(cachedFileName, EncryptedpdfBytes);
				m_downloading = false;
				m_DC.FinishDownload();
				Info.AlreadyLoaded = true;
			}
		}
		yield return null;

	}

	// This will be called by whatever opens the PDFViewer and will call m_PDFviewer.LoadDocumentWithBuffer(bytes, "") with the bytes returned by this
	public byte[] GetPDFBytes()
	{
		// TODO: Error handle file not existing
		byte[] file = File.ReadAllBytes(Application.persistentDataPath + "/DownloadedDocuments/" + FileName);
		return Helper.Decrypt(file);
	}

}

