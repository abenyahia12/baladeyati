using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Paroxe.PdfRenderer.Internal.Viewer
{
	public class LeftPanelHandler : MonoBehaviour
	{
		public PDFViewerLeftPanel m_PdfViewerLeftPanel;
		Color m_Grey= new Color(0.50f, 0.50f, 0.50f);
		public void OnBookmarksTabClicked()
		{
			m_PdfViewerLeftPanel.m_BookmarksTab.sprite = m_PdfViewerLeftPanel.m_OpenedTabSprite;
			m_PdfViewerLeftPanel.m_BookmarksTabImage.color = Color.white;
			m_PdfViewerLeftPanel.m_ThumbnailsTabImage.color = m_Grey;

		}
	
		public void OnThumbnailsTabClicked()
		{
			m_PdfViewerLeftPanel.m_BookmarksTab.sprite = m_PdfViewerLeftPanel.m_ClosedTabSprite;
			m_PdfViewerLeftPanel.m_BookmarksTabImage.color = m_Grey;
			m_PdfViewerLeftPanel.m_ThumbnailsTabImage.color = Color.white;

		}

	}

}