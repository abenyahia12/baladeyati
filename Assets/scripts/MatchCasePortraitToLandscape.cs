using UnityEngine;

public class MatchCasePortraitToLandscape : MonoBehaviour
{
	public Animator m_animator;

	private void Start()
	{
		AdaptToOrientation();
	}

	private void Update()
	{
		AdaptToOrientation();
	}

	public void AdaptToOrientation()
	{
		switch (Input.deviceOrientation) {
			case DeviceOrientation.PortraitUpsideDown:
			case DeviceOrientation.Portrait:
				m_animator.SetBool("SwitchPortrait", true);
				m_animator.SetBool("SwitchLandscape", false);
				break;
			case DeviceOrientation.LandscapeLeft:
			case DeviceOrientation.LandscapeRight:
				m_animator.SetBool("SwitchLandscape", true);
				m_animator.SetBool("SwitchPortrait", false);
				break;
		}
	}
}
