using Paroxe.PdfRenderer;
using UnityEngine;
using UnityEngine.UI;
namespace Paroxe.PdfRenderer.Internal.Viewer
{
	public class FadingElements : MonoBehaviour
	{
		public float m_FadeSpeed = 0.5f;
		public ScrollRect m_ScrollRect;
		public PDFViewer m_PDFViewer;
		public CanvasGroup m_ZoomCanvasGroup;
		public CanvasGroup m_PageNumber;
		public float m_Velocity;
		public float m_MobileVelocity;

		private void HandleScroll(float velocity, Vector2 direction, float minVelocity)
		{
			if (LeanTween.isTweening(m_PDFViewer.m_Internal.m_TopPanel)) return;

			if (velocity < minVelocity) return;


			m_ZoomCanvasGroup.alpha = 1;
			m_PageNumber.alpha = 1;
		}
		
		void Update()
		{
			float velocity = m_ScrollRect.velocity.magnitude;
			Vector2 direction = m_ScrollRect.velocity;

#if UNITY_IOS || UNITY_ANDROID
			HandleScroll(velocity, direction, m_MobileVelocity);
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
HandleDesktop(velocity, direction, m_Velocity);
#endif

			m_ZoomCanvasGroup.alpha -= Time.deltaTime * m_FadeSpeed;
			m_PageNumber.alpha -= Time.deltaTime * m_FadeSpeed;
		
		}

		//This function is called on vercital scrollbar ui elements under pdfviewer
		public void ShowPageNumberAndZoomBoxes()
		{
			m_ZoomCanvasGroup.alpha = 1;
			m_PageNumber.alpha = 1;
		}
	}
}