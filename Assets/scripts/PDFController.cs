using System.Collections;
using Paroxe.PdfRenderer;
using UnityEngine;

public class PDFController : MonoBehaviour
{
	public PDFModel   m_DM;
	public GameObject m_PDFViewerGo;

	public PDFButtonView m_SelfButton;

	public ErrorHandler            m_ErrorHandler;
	public DownloadModalController m_DownloadModalController;

	public PDFViewer        m_PDFviewer;
	public GameObject       m_grid;
	public PDFButtonHandler m_buttonHandler;
	public GameObject       LoadingImage;

	private void Start()
	{
		m_SelfButton = GetComponent<PDFButtonView>();
		m_DM = GetComponent<PDFModel>();

		m_SelfButton.m_DownloadStatusIcon.enabled = m_DM.HasDownloaded();
	}

	public void DownloadAction()
	{
		m_DownloadModalController.ModalWindowDownload();
		m_SelfButton.SwapIconToDownloading();
		m_SelfButton.m_DownloadStatusIcon.enabled = true;

		m_DM.StartDownload();
	}

	public void UpdateDownloadStatus(float progress)
	{
		if (m_DownloadModalController.m_ClickedButton == this)
		{
			m_DownloadModalController.UpdateDownloadStatus(progress);
		}
	}

	public void UninstallAction()
	{
		m_DownloadModalController.ModalWindowDownload();
		m_DM.Uninstall();
	}

	public void FinishDownload()
	{
		if (m_DownloadModalController.m_ClickedButton == this)
		{
			m_DownloadModalController.m_DownloadBar.SetActive(false);
			m_DownloadModalController.m_OpenButton.SetActive(true);
		}

		m_ErrorHandler.NotifyEndOfDownload("Downloading " + m_DM.FileName + " is done !");
		m_SelfButton.SwapIconToDownloaded();
	}

	public void FinishUninstall()
	{
		if (m_DownloadModalController.m_ClickedButton == this)
		{
			m_DownloadModalController.ModalWindowNotDownloaded();
		}

		m_SelfButton.m_DownloadStatusIcon.enabled = false;
	}

	public void EnableModalDownloadWindow()
	{
		m_DownloadModalController.EnableModalWindow(this, m_DM.Info);
	}

	public void ModalWindowNotDownloaded()
	{
		m_DownloadModalController.ModalWindowNotDownloaded();
	}

	public void ModalWindowDownloading()
	{
		m_DownloadModalController.ModalWindowDownloading();
	}


	public void SummonErrorWindow(string error)
	{
		// we will adapt this to the right error icon
		m_ErrorHandler.DisplayErrorWithIcon(error, m_ErrorHandler.m_NetworkSprite);
	}

	public void ModalWindowUninstall()
	{
		m_DownloadModalController.ModalWindowUninstall();
	}

	public void OpenFile()
	{
		m_DownloadModalController.m_ModalPdfDownloadWindow.enabled = false;
		m_PDFViewerGo.SetActive(true);
		StartCoroutine(LoadFile());
	}

	public IEnumerator LoadFile()
	{
		LoadingImage.SetActive(true);

		// Two frames to allow loading screen chance to be rendered
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		m_grid.GetComponent<Canvas>().enabled = false;

		m_PDFviewer.LoadDocumentFromBuffer(m_DM.GetPDFBytes(), "");
		m_buttonHandler.m_MatchCasePortraitLandscape.AdaptToOrientation();

		LoadingImage.SetActive(false);
	}
}
