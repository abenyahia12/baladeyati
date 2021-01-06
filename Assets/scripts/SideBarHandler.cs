using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Paroxe.PdfRenderer.Internal.Viewer
{
	public class SideBarHandler : MonoBehaviour
	{
		public PDFViewerLeftPanel m_PdfViewerLeftPanel;
		
		void Start()
		{
			m_PdfViewerLeftPanel = this.GetComponent<PDFViewerLeftPanel>();
			if (m_PdfViewerLeftPanel.m_Opened)
			{
				// I need to use the toggle so that the arrow swaps sprite and updates the open bool
				m_PdfViewerLeftPanel.Toggle();
			}
		}

		void OnEnable()
		{	
			Close();
		}


		public void Close()
		{
			if (m_PdfViewerLeftPanel.m_Opened)
			{
				m_PdfViewerLeftPanel.Toggle();
			}
		}
	}
}