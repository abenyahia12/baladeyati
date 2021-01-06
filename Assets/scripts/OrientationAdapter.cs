using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OrientationAdapter : MonoBehaviour
{
	public Image m_backgroundPortrait;
	public Image m_backgroundLandscape;
	public float m_delay = 0.7f;
	public bool m_CanSwitch = true;
	void Start()
	{
		if (((Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) || (Input.deviceOrientation == DeviceOrientation.Portrait)) && (m_CanSwitch == true))
		{
			m_CanSwitch = false;

			m_backgroundPortrait.enabled = true;
			m_backgroundLandscape.enabled = false;
			StartCoroutine(StartDelay());
		}
		else if ((Input.deviceOrientation == DeviceOrientation.LandscapeLeft) || (Input.deviceOrientation == DeviceOrientation.LandscapeRight) && (m_CanSwitch == true))
		{
			m_CanSwitch = false;
			m_backgroundPortrait.enabled = false;
			m_backgroundLandscape.enabled = true;
			StartCoroutine(StartDelay());
		}
	}


	void Update()
	{

		AdaptToOrientation();
	}

	void AdaptToOrientation()
	{

		if (((Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) || (Input.deviceOrientation == DeviceOrientation.Portrait)) && (m_CanSwitch == true))
		{
			m_CanSwitch = false;
			m_backgroundPortrait.enabled = true;
			m_backgroundLandscape.enabled = false;
			StartCoroutine(StartDelay());
		}
		else if (((Input.deviceOrientation == DeviceOrientation.LandscapeLeft) || (Input.deviceOrientation == DeviceOrientation.LandscapeRight)) && (m_CanSwitch == true))
		{
			m_CanSwitch = false;

			m_backgroundPortrait.enabled = false;
			m_backgroundLandscape.enabled = true;
			StartCoroutine(StartDelay());
		}

	}
	private IEnumerator StartDelay()
	{

		float timer = 0;
		while (timer < m_delay)
		{
			timer += 0.1f;
			yield return new WaitForSeconds(0.05f);
		}
		m_CanSwitch = true;
		yield return null;
	}
}
