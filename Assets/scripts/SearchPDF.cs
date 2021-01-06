using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Paroxe.PdfRenderer.Internal.Viewer
{
	public class SearchPDF : MonoBehaviour
	{
		public PDFSearchPanel m_SearchPanel;
		public Image m_LoopImage;
		private Color m_TransparentColor;
		void Start()
		{
			m_TransparentColor = new Color(m_SearchPanel.m_ValidatorImage.color.r, m_SearchPanel.m_ValidatorImage.color.g, m_SearchPanel.m_ValidatorImage.color.b, 0.0f);

		}
		void Update()
		{
			if (string.IsNullOrEmpty(m_SearchPanel.m_InputField.text.Trim()))
			{
				m_SearchPanel.m_CloseButton.gameObject.SetActive(false);
				m_LoopImage.enabled = true;
			}
			else
			{
				m_SearchPanel.m_CloseButton.gameObject.SetActive(true);
				m_LoopImage.enabled = false;
			}
		}


		public void Close()
		{
			m_SearchPanel.m_InputField.text = "";
			m_SearchPanel.m_ValidatorImage.color = m_TransparentColor;
			m_SearchPanel.m_TotalResultText.text = "";
			m_SearchPanel.m_MatchCaseCheckBox.enabled = false;
			m_SearchPanel.m_MatchWholeWordCheckBox.enabled = false;
			m_SearchPanel.Close();

		}



	}
}