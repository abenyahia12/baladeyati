using UnityEngine;
using UnityEngine.UI;

namespace Paroxe.PdfRenderer.Internal.Viewer
{
	public class FitToWidthHandler : MonoBehaviour
	{
		public PDFViewer m_PDFViewer;
		public Image m_FitToWidthBackgroundImage;   // Background image to indicate fit to width is active

		private void Update()
		{
			if (m_PDFViewer.PageFitting == PDFViewer.PageFittingType.ViewerWidth)
			{
				m_FitToWidthBackgroundImage.enabled = true;
			}
			else
			{
				m_FitToWidthBackgroundImage.enabled = false;
			}
		}

		//This method is triggered by the Fit To Width Button
		public void ToggleFitToWidthHeight()
		{
			if (m_PDFViewer.PageFitting == PDFViewer.PageFittingType.ViewerWidth)
			{
				m_PDFViewer.PageFitting = PDFViewer.PageFittingType.ViewerHeight;
			}
			else
			{
				m_PDFViewer.PageFitting = PDFViewer.PageFittingType.ViewerWidth;
			}

			m_PDFViewer.AdjustZoomToPageFitting(m_PDFViewer.PageFitting, m_PDFViewer.GetDevicePageSize(m_PDFViewer.CurrentPageIndex));
		}
	}
}