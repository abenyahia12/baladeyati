using UnityEngine;

public class DebugLogin : MonoBehaviour
{
	public LoginSystem	m_loginSystem	= null;
	public string		m_userName		= "";
	public string		m_password		= "";

	public void Login()
	{
		m_loginSystem.Login(m_userName, m_password);
	}
}
