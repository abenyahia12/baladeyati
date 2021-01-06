using System;
using Paroxe.PdfRenderer;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
namespace Paroxe.PdfRenderer.Internal.Viewer
{
	public class ZoomDisplay : MonoBehaviour
	{
		public Text m_ZoomText;
		public Text m_ZoomTextOpen;
		public PDFViewer m_PDFViewer;
		public float m_FadeSpeed = 0.5f;
		private int m_tapCount = 0;
		private float m_doubleTapTimer;
		public CanvasGroup m_ZoomDisplayClosed;
		public CanvasGroup m_ZoomDisplayOpened;
		public InputField m_InputField;
		public float m_ThreshholdDistance;
		public PDFViewerLeftPanel m_PDFViewerLeftPanel;
		private bool m_PrevSideBarOpen = false;
		private float m_PreviousZoomFactor = 0;
		private float m_DoubleTapCouldown = 0f;
		public float m_MaxCooldown = 0.5f;
		public float m_MinimumTimeToDetectSecondTap = 0.2f;
		public float m_MaxTimeToDetectSecondTap = 0.5f;
		public float m_PreviousZoomToGo = 0;

		void Start()
		{
			m_ThreshholdDistance = (Mathf.Sqrt((Mathf.Pow(Screen.width, 2)) + (Mathf.Pow(Screen.height, 2)))) * 0.02f;
		}
		//This function is called with the zoom in /out buttons
		public void ZoomDisplayUpdate()
		{
			if (m_PDFViewerLeftPanel.m_Opened)
			{
				m_ZoomDisplayOpened.alpha = 1;
				m_ZoomDisplayClosed.alpha = 0;
			}
			else
			{
				m_ZoomDisplayOpened.alpha = 0;
				m_ZoomDisplayClosed.alpha = 1;
			}
		}
		
		void UpdateZoomText()
		{

			if (Math.Abs(m_PDFViewer.ZoomFactor - 1) < Mathf.Epsilon)
			{
				SetZoomText("FIT");
			}
			else if (Math.Abs(m_PDFViewer.ZoomFactor - m_PDFViewer.MaxZoomFactor) < Mathf.Epsilon)
			{
				SetZoomText("MAX");
			}
			else if (Math.Abs(m_PDFViewer.ZoomFactor - m_PDFViewer.MinZoomFactor) < Mathf.Epsilon)
			{
				SetZoomText("MIN");
			}
			else
			{
				SetZoomText((int)(m_PDFViewer.ZoomFactor * 100) + " %");
			}

		}
		public void SetZoomText(string text)
		{
			m_ZoomText.text = text;
			m_ZoomTextOpen.text = text;
		}
		void FixedUpdate()
		{

			if (m_PDFViewer.ZoomFactor != m_PreviousZoomFactor)
			{
				ZoomDisplayUpdate();
			}
			if ((m_PDFViewerLeftPanel.m_Opened) && (m_PDFViewerLeftPanel.m_Opened != m_PrevSideBarOpen))
			{

				OnSideBarClosed();

			}
			else if (!(m_PDFViewerLeftPanel.m_Opened) && (m_PDFViewerLeftPanel.m_Opened != m_PrevSideBarOpen))
			{
				OnSideBarOpened();
			}
			UpdateZoomText();
			UpdateZoomAlpha();
			
			m_PrevSideBarOpen = m_PDFViewerLeftPanel.m_Opened;
			m_PreviousZoomFactor = m_PDFViewer.ZoomFactor;
			//if (LeanTween.isTweening(m_PDFViewer.m_Internal.m_TopPanel)) return;
			//HandleDoubleTap();
		}
		void OnSideBarOpened()
		{
			m_ZoomDisplayOpened.alpha = 0;
			m_ZoomDisplayClosed.alpha = 1;
		}
		void OnSideBarClosed()
		{
			m_ZoomDisplayOpened.alpha = 1;
			m_ZoomDisplayClosed.alpha = 0;
		}
		void UpdateZoomAlpha()
		{
			if (m_ZoomDisplayOpened.alpha > 0)
			{
				m_ZoomDisplayOpened.alpha -= Time.fixedDeltaTime * m_FadeSpeed;
			}

			if (m_ZoomDisplayClosed.alpha > 0)
			{
				m_ZoomDisplayClosed.alpha -= Time.fixedDeltaTime * m_FadeSpeed;
			}
		}
		void HandleDoubleTap()
		{
			//check if input field is focused or not
			if (m_InputField.isFocused)
				return;
			if (m_doubleTapTimer > m_MaxTimeToDetectSecondTap)
			{
				StopTimer();
			}
		
			if ((Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)&&!(m_PDFViewer.m_Internal.m_LeftPanelRect.rect.Contains(Input.GetTouch(0).position)))
			{
				m_tapCount++;
			}
			m_DoubleTapCouldown += Time.fixedDeltaTime;
			//The PdfViewerPlugin made the zoom fix the resolution over a few milliseconds, so this checks that we don't double tap until it finishes its ZoomLevel
			if (!Mathf.Approximately(m_PDFViewer.m_ZoomFactor, m_PDFViewer.m_ZoomToGo))
			{
				StopTimerAndCooldown();
				return;
			}
			if (m_DoubleTapCouldown > m_MaxCooldown)
			{

				if (m_tapCount > 0)
				{
					m_doubleTapTimer += Time.fixedDeltaTime;
				}
				if (m_tapCount >= 2)
				{

					if ((m_doubleTapTimer >= m_MinimumTimeToDetectSecondTap) && ((m_doubleTapTimer <= m_MaxTimeToDetectSecondTap)))
					{
						if (m_PDFViewer.PageFitting != PDFViewer.PageFittingType.ViewerWidth)
						{

							m_PDFViewer.PageFitting = PDFViewer.PageFittingType.ViewerWidth;
							m_PDFViewer.AdjustZoomToPageFitting(m_PDFViewer.PageFitting, m_PDFViewer.GetDevicePageSize(m_PDFViewer.CurrentPageIndex));
							m_PreviousZoomToGo = m_PDFViewer.m_ZoomToGo;
						}
						else
						{
							m_PDFViewer.PageFitting = PDFViewer.PageFittingType.ViewerHeight;
							m_PDFViewer.AdjustZoomToPageFitting(m_PDFViewer.PageFitting, m_PDFViewer.GetDevicePageSize(m_PDFViewer.CurrentPageIndex));
							m_PreviousZoomToGo = 0;
						}
						m_doubleTapTimer = 0f;
						m_tapCount = 0;
						m_DoubleTapCouldown = 0f;
					}
				}


			}
			else
			{
				StopTimer();
			}
		}
		//StopTimerAndCooldown() is made to stop Timer and cooldown in order to have other buttons on the top panel stop the double tap from happening in wrong situations
		public void StopTimerAndCooldown()
		{
			m_DoubleTapCouldown = 0f;
			m_doubleTapTimer = 0f;
			m_tapCount = 0;
		}
		//StopTimer() is made to stop Timer and cooldown in order to have other buttons on the top panel stop the double tap from happening in wrong situations
		public void StopTimer()
		{
			m_doubleTapTimer = 0f;
			m_tapCount = 0;
		}
		//FitToPrevious() is called by the side bar arrow to make the page container fit to width if it was on fitting page type "Width"
		public void FitToPrevious()
		{
			if ((m_PDFViewer.PageFitting == PDFViewer.PageFittingType.ViewerWidth)|| (m_PreviousZoomToGo==m_PDFViewer.m_ZoomToGo))
			{
				m_PDFViewer.ZoomOut();
				StartCoroutine(DelayedFitToWidth());
			}
		}
		public void OnZoomInButtonClicked()
		{
			m_tapCount = 0;
			if (m_PDFViewer != null)
			{
				m_PDFViewer.ZoomIn();
			}

		}
		public IEnumerator DelayedFitToWidth()
		{
			yield return new WaitForSeconds(0.5f);

			m_PDFViewer.PageFitting = PDFViewer.PageFittingType.ViewerWidth;
			m_PDFViewer.AdjustZoomToPageFitting(m_PDFViewer.PageFitting, m_PDFViewer.GetDevicePageSize(m_PDFViewer.CurrentPageIndex));
			m_PreviousZoomToGo = m_PDFViewer.m_ZoomToGo;
			StopTimerAndCooldown();
		}
		public void OnZoomOutButtonClicked()
		{
			m_tapCount = 0;
			if (m_PDFViewer != null)
			{
				m_PDFViewer.ZoomOut();
			}
		}
	}
}
