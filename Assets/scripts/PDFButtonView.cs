using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PDFButtonView : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
	public Image m_ButtonImage;
	public Text  m_UIfilename;

	public Sprite          DownloadedIcon;
	public Sprite          StillDownloadingIcon;
	public Sprite          FailedToDownload;
	public Image           m_DownloadStatusIcon;
	public PDFController   m_DC;
	public PDFModel        m_DM;
	public GridLayoutGroup m_OwnGridParent;

	//timer for long press
	public  float         m_longPressTime = 0.8f;
	private float         m_longPressTimer;
	public  RectTransform m_GridOfGrids;
	private float         m_yStart; // Start position of long press
	public  float         m_ScrollThreshHold = 5;

	private void AdaptIconSize()
	{
		m_ButtonImage.SetNativeSize();

		float height = m_ButtonImage.rectTransform.GetHeight();
		float width = m_ButtonImage.rectTransform.GetWidth();
		float cellSizeX = m_OwnGridParent.cellSize.x;
		float newSizeFactor = 1.0f;

		if ((height > width) && (height > cellSizeX))
		{
			newSizeFactor = cellSizeX / height;
		}
		else if ((height <= width) && (width > cellSizeX))
		{
			newSizeFactor = cellSizeX / width;
		}
		else if ((height <= width) && (width < cellSizeX))
		{
			newSizeFactor = cellSizeX / width;
		}
		else if ((width <= height) && (height < cellSizeX))
		{
			newSizeFactor = cellSizeX / height;
		}

		RectTransform buttonTransform = m_ButtonImage.rectTransform;
		buttonTransform.SetHeight(height * newSizeFactor);
		buttonTransform.SetWidth(width * newSizeFactor);
	}

	public void Start()
	{
		m_OwnGridParent = transform.parent.GetComponent<GridLayoutGroup>();
		m_DownloadStatusIcon.enabled = m_DM.HasDownloaded();
		AdaptIconSize();
	}

	public void LoadDocument()
	{
		m_DC.OpenFile();
	}

	//when mouse is down
	public void OnPointerDown(PointerEventData pointerEventData)
	{
		m_yStart = m_GridOfGrids.position.y;

		//We start a timer here to detect the long press on pointer UP aka mouse up
		StartCoroutine(StartTimer());
	}

	private IEnumerator StartTimer()
	{
		while (m_longPressTimer < m_longPressTime)
		{
			m_longPressTimer += 0.1f;
			yield return new WaitForSeconds(0.1f);

			float currentYPos = m_GridOfGrids.position.y;
			float diffYPos = Math.Abs(m_yStart - currentYPos);

			if (diffYPos > m_ScrollThreshHold)
			{
				m_longPressTimer = 0.0f;
				yield break;
			}
		}

		OpenModal();

		m_longPressTimer = 0.0f;
	}

	public void SwapIconToDownloaded()
	{
		m_DownloadStatusIcon.sprite = DownloadedIcon;
	}

	public void SwapIconToDownloading()
	{
		m_DownloadStatusIcon.sprite = StillDownloadingIcon;
	}

	public void SwapIconToFailed()
	{
		m_DownloadStatusIcon.sprite = FailedToDownload;
	}

	private void OpenModal()
	{
		if (m_DM.HasDownloaded())
		{
			m_DC.ModalWindowUninstall();
		}
		// activate download button only and fill the modal window while informing the button handler which button was clicked
		else if (m_DM.IsDownloading())
		{
			m_DC.ModalWindowDownloading();
		}
		else
		{
			m_DC.ModalWindowNotDownloaded();
		}

		m_DC.EnableModalDownloadWindow();
	}

	public void OnPointerClick(PointerEventData pointerEventData)
	{
		StopAllCoroutines(); // Stop long press timer

		float currentYPos = m_GridOfGrids.position.y;
		float diffYPos = Math.Abs(m_yStart - currentYPos);

		if (diffYPos > m_ScrollThreshHold)
		{
			m_longPressTimer = 0.0f;
			return;
		}

		if (m_longPressTimer >= m_longPressTime)
		{
			return;
		}

		switch (pointerEventData.button)
		{
			case PointerEventData.InputButton.Left:
				{
					if (m_DM.HasDownloaded())
					{
						LoadDocument();
					}
					else
					{
						OpenModal();
					}

					break;
				}
			case PointerEventData.InputButton.Right:
				OpenModal();
				break;
		}

		m_longPressTimer = 0.0f;
	}
}
