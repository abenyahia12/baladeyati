using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Paroxe.PdfRenderer.Internal.Viewer
{
	public class TopBarHandler : MonoBehaviour
	{

		public PDFViewerInternal m_Internal;
		public float m_TopPanelHeight= 121;
		public float m_AnimationSpeed=0.5f;
		public float m_FadeSpeed = 0.5f;
		public ScrollRect m_ScrollRect;
		public PDFViewer m_PDFViewer;
		public float m_Velocity;
		public float m_MobileVelocity;
		public bool m_TopBarIsShowing = true;


		public void ShowTopBarFunction(bool show)
		{
			if (LeanTween.isTweening(m_Internal.m_TopPanel)) return;


			if (!show)
			{
				LeanTween.moveY(m_Internal.m_TopPanel, m_TopPanelHeight, m_AnimationSpeed);
				LeanTween.size(m_Internal.m_Viewport, new Vector2(m_Internal.m_Viewport.sizeDelta.x, 0f), m_AnimationSpeed);
				LeanTween.size(m_Internal.m_VerticalScrollBar, new Vector2(m_Internal.m_VerticalScrollBar.sizeDelta.x, 0f), m_AnimationSpeed);

				if (m_Internal.m_LeftPanel == null) return;

				RectTransform leftPanel = m_Internal.m_LeftPanel.transform as RectTransform;
				LeanTween.size(leftPanel, new Vector2(leftPanel.sizeDelta.x, 0.0f), m_AnimationSpeed);
			}
			else
			{
				LeanTween.moveY(m_Internal.m_TopPanel, 0f, m_AnimationSpeed);
				LeanTween.size(m_Internal.m_Viewport, new Vector2(m_Internal.m_Viewport.sizeDelta.x, -m_TopPanelHeight), m_AnimationSpeed);
				LeanTween.size(m_Internal.m_VerticalScrollBar, new Vector2(m_Internal.m_VerticalScrollBar.sizeDelta.x, -m_TopPanelHeight), m_AnimationSpeed);

				if (m_Internal.m_LeftPanel == null) return;

				RectTransform leftPanel = m_Internal.m_LeftPanel.transform as RectTransform;
				LeanTween.size(leftPanel, new Vector2(leftPanel.sizeDelta.x, -m_TopPanelHeight+1), m_AnimationSpeed);
			}
		}
		private void HandleScroll(float velocity, Vector2 direction, float minVelocity)
		{
			if (LeanTween.isTweening(m_PDFViewer.m_Internal.m_TopPanel)) return;

			if (velocity < minVelocity) return;

			if (direction.y > 0)
			{

				if (m_PDFViewer.GetMostVisiblePageIndex() != 0)
				{
					if (m_TopBarIsShowing)
					{
						HideTopBar();
					}

				}
			}
			else
			{
				if (!m_TopBarIsShowing)
				{
					ShowTopBar();
				}


			}

		}
		void ShowTopBar()
		{
			ShowTopBarFunction(true);
			m_TopBarIsShowing = true;
		}
		void HideTopBar()
		{
			ShowTopBarFunction(false);
			m_TopBarIsShowing = false;
		}
		void Update()
		{
			float velocity = m_ScrollRect.velocity.magnitude;
			Vector2 direction = m_ScrollRect.velocity;

#if UNITY_IOS || UNITY_ANDROID
			HandleScroll(velocity, direction, m_MobileVelocity);

#endif
			if (LeanTween.isTweening(m_PDFViewer.m_Internal.m_TopPanel) == false)
			{
				if (m_TopBarIsShowing == false)
				{
					if (m_PDFViewer.m_PageCount != 0)
					{
						if (m_PDFViewer.GetMostVisiblePageIndex() == 0)
						{

							ShowTopBar();
						}

					}
				}
			}
		}
	}
}
