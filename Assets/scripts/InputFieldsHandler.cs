using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputFieldsHandler : MonoBehaviour {

	public InputField m_UsernameInputField;
	public InputField m_PasswordInputField;
	public Animator m_InputFieldsAnimator;

	void Update()
	{
		ChangeAnimation();
	}
	public void ChangeAnimation()
	{

		if (((m_PasswordInputField.isFocused) || (m_UsernameInputField.isFocused)))
		{

			m_InputFieldsAnimator.SetBool("IsFocused", true);

		}

	}

	public void UnZoom()
	{
		m_InputFieldsAnimator.SetBool("IsFocused", false);
	}
}
