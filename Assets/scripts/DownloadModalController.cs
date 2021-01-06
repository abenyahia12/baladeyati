using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DownloadModalController : MonoBehaviour
{
	public PDFController m_ClickedButton;
	public Canvas m_ModalPdfDownloadWindow;

	public GameObject m_DownloadButton;
	public GameObject m_OpenButton;
	public GameObject m_UninstallButton;
	public GameObject m_DownloadBar;


	public Image m_ModalWindowIconSprite;
	public Text m_ModalWindowFileName;
	public Text m_ModalWindowCourseName;

	public Text m_DownloadText;
	public Image m_DownloadImage;

	public void EnableModalWindow(PDFController clickedController, PDFInfo info)
	{
		m_ClickedButton = clickedController;
		m_ModalPdfDownloadWindow.enabled = true;
		SetText(info);
	}

	private void SetText(PDFInfo info)
	{
		m_ModalWindowCourseName.text = info.CourseName;
		m_ModalWindowFileName.text = info.FileName;
		m_ModalWindowIconSprite.sprite = info.Icon;
	}

	public void ModalWindowDownloading()
	{
		m_UninstallButton.SetActive(false);
		m_DownloadButton.SetActive(false);
		m_OpenButton.SetActive(false);
		m_DownloadBar.SetActive(true);
	}
	public void ModalWindowNotDownloaded()
	{
		m_UninstallButton.SetActive(false);
		m_DownloadButton.SetActive(true);
		m_OpenButton.SetActive(false);
		m_DownloadBar.SetActive(false);
	}
	public void ModalWindowUninstall()
	{
		m_UninstallButton.SetActive(true);
		m_DownloadButton.SetActive(false);
		m_OpenButton.SetActive(false);
		m_DownloadBar.SetActive(false);
	}
	public void ModalWindowDownload()
	{
		m_DownloadBar.SetActive(true);
		m_UninstallButton.SetActive(false);
		m_DownloadButton.SetActive(false);
		m_OpenButton.SetActive(false);
	}

	public void UpdateDownloadStatus(float progress)
	{
		m_DownloadImage.fillAmount = progress;
		m_DownloadText.text = "Loading " + ((int)(progress * 100)) + "%";

	}
	public void UpdateUninstallStatus(float progress)
	{

		m_DownloadImage.fillAmount = progress;
		m_DownloadText.text = "Deleted " + ((int)(progress * 100)) + "%";

	}
	public void DownloadClickedButton()
	{
		m_ClickedButton.DownloadAction();
	}
	public void UninstallClickedButton()
	{
		m_ClickedButton.UninstallAction();
	}
	public void OpenFile()
	{
		m_ClickedButton.OpenFile();


	}
}
