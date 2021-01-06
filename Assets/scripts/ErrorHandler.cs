using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ErrorHandler : MonoBehaviour
{
	public Canvas m_ErrorCanvas;
	public enum Error { NetworkError, LackOfSpaceError, UnauthorizedError };
	public Sprite m_NetworkSprite;
	public string m_NetworkMessage;
	public Sprite m_SpaceSprite;
	public string m_SpaceMessage;
	public Sprite m_UnauthorizedSprite;
	public string m_UnauthorizedMessage;
	public Sprite m_MultipleErrorsSprite;
	public string m_MultipleErrorsMessage;
	public string ErrorUnspecified;
	public Sprite m_DownloadedFileIcon;
	public Image m_ErrorIcon;
	public Text m_ErrorMessage;
	public Text m_ErrorReason;
	public Animator m_ErrorWindowAnimator;
	public Image m_targetImage;
	public float m_fadeDuration = 0.5f;
	public float m_stayDuration = 0.1f;
	public AnimationCurve m_smoothCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
	private float m_timerCurrent;
	private readonly WaitForSeconds m_skipFrame = new WaitForSeconds(0.01f);

	private int m_counter = 0;
	private int m_Downloadcounter = 0;
	// Use this for initialization
	[Header("Simulation Tools")]
	public bool HighlightSimulator = false;

	public bool ErrorSimulator = false;

	void Update()
	{
		if (HighlightSimulator)
		{
			HighlightSimulator = false;
			HighlightErrorWindow();
		}

		if (ErrorSimulator)
		{
			ErrorSimulator = false;
			DisplayErrorWithIcon(ErrorUnspecified, m_NetworkSprite);
		}

	}

	private Sprite GetSpriteForError(Error errorType)
	{
		switch(errorType)
		{
			case Error.NetworkError:
				return m_NetworkSprite;
			case Error.LackOfSpaceError:
				return m_SpaceSprite;
			case Error.UnauthorizedError:
				return m_UnauthorizedSprite;
			default:
				return null;
		}
	}

	public void DisplayError(Error errorType, string message)
	{
		m_counter++;

		//check if user is already visualizing an error in order to update it or if the user wasn't having any error
		if (!m_ErrorCanvas.enabled)
		{
			m_ErrorMessage.text = message;
			m_ErrorIcon.sprite = GetSpriteForError(errorType);
			m_ErrorCanvas.enabled = true;
			m_ErrorWindowAnimator.SetBool("ErrorIn", true);
		}
		else
		{
			m_ErrorIcon.sprite = m_MultipleErrorsSprite;
			m_ErrorMessage.text = m_counter + " " + m_MultipleErrorsMessage;
			m_ErrorReason.text = "";
			HighlightErrorWindow();
		}
	}

	public void DisplayErrorUnspecified(string error, string CourseName)
	{
		//check if user is already visualizing an error in order to update it or if the user wasn't having any error
		if (!m_ErrorCanvas.enabled)
		{
			m_counter++;
			m_ErrorMessage.text = "Downloading " + CourseName + " has failed.";
			m_ErrorIcon.sprite = m_MultipleErrorsSprite;
			m_ErrorReason.text = "Reason : " + error;
			m_ErrorCanvas.enabled = true;
			m_ErrorWindowAnimator.SetBool("ErrorIn", true);
		}
		else
		{
			m_counter++;

			m_ErrorIcon.sprite = m_MultipleErrorsSprite;
			m_ErrorMessage.text = m_counter + " " + m_MultipleErrorsMessage;
			m_ErrorReason.text = "";
			HighlightErrorWindow();
		}
	}
	public void DisplayErrorWithIcon(string error, Sprite Icon)
	{
		//check if user is already visualizing an error in order to update it or if the user wasn't having any error
		if (!m_ErrorCanvas.enabled)
		{
			m_counter++;
			m_ErrorMessage.text = error;
			m_ErrorIcon.sprite = Icon;
			m_ErrorReason.text = "";
			m_ErrorCanvas.enabled = true;
			m_ErrorWindowAnimator.SetBool("ErrorIn", true);
		}
		else
		{
			m_counter++;

			m_ErrorIcon.sprite = Icon;
			m_ErrorMessage.text = error;
			m_ErrorReason.text = "";
			HighlightErrorWindow();
		}
	}
	public void NotifyEndOfDownload(string error)
	{
		//check if user is already visualizing an error in order to update it or if the user wasn't having any error
		if (!m_ErrorCanvas.enabled)
		{
			m_Downloadcounter++;
			m_ErrorMessage.text = error;
			m_ErrorIcon.sprite = m_DownloadedFileIcon;
			m_ErrorReason.text = "";
			m_ErrorCanvas.enabled = true;
			m_ErrorWindowAnimator.SetBool("ErrorIn", true);
		}
		else
		{
			m_Downloadcounter++;
			m_ErrorIcon.sprite = m_MultipleErrorsSprite;
			m_ErrorMessage.text = error;
			m_ErrorReason.text = "";
			HighlightErrorWindow();
		}
	}
	public void ResetErrorCounter()
	{
		m_Downloadcounter = 0;
		m_counter = 0;
		m_ErrorWindowAnimator.SetBool("ErrorIn", false);
	}
	void HighlightErrorWindow()
	{
		StartCoroutine(FadeInOut());
	}
	private IEnumerator FadeInOut()
	{
		float start = 0f;
		float end = 1f;
		yield return StartCoroutine(Fade(end, start));
		yield return new WaitForSeconds(m_stayDuration);
		yield return StartCoroutine(Fade(start, end));

		HighlightSimulator = false;
	}

	private IEnumerator Fade(float start, float end)
	{
		m_timerCurrent = 0f;

		while (m_timerCurrent <= m_fadeDuration)
		{
			m_timerCurrent += Time.deltaTime;
			Color c = m_targetImage.color;
			m_targetImage.color = new Color(c.r, c.g, c.b, Mathf.Lerp(start, end, m_smoothCurve.Evaluate(m_timerCurrent / m_fadeDuration)));
			yield return m_skipFrame;
		}
	}
}
