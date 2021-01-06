using System.IO;
using UnityEngine;

public class ButtonGeneratorTest : MonoBehaviour
{
	private const string           m_serverURL = "http://52.209.217.87/asset_bundles/";
	public        PDFButtonHandler m_buttonManager;
	public        IconLookup       m_IconLookup;
	private       string           m_dirPath;

	private void GenerateTestButton(string name, string courseName, string fileName)
	{
		PDFInfo newButton = new PDFInfo();
		newButton.DisplayName = name;
		newButton.CourseName = courseName;
		newButton.FileName = fileName;
		newButton.URL = m_serverURL + fileName;
		newButton.Icon = m_IconLookup.GetIconFromName(newButton.DisplayName);

		if (File.Exists(m_dirPath + newButton.FileName))
		{
			newButton.AlreadyLoaded = true;
		}

		m_buttonManager.AddButton(newButton);
	}

	private void Start()
	{
		m_dirPath = Application.persistentDataPath + "/DownloadedDocuments/";

		// Create test buttons
		for (int i = 0; i < 9; ++i)
		{
			GenerateTestButton("Mass and Balance Manual", "ATPL (A)", "mb(m).pdf");
		}

		GenerateTestButton("Performance Manual", "ATPL (B)", "perf(m).pdf");
		GenerateTestButton("Non Exsistant", "ATPL (B)", "NonExsi.pdf");
		GenerateTestButton("Unknown", "ATPL (k)", "Unknown.pdf");

		m_buttonManager.GenerateContent();
	}
}
