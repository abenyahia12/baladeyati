using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeTextSwapper : MonoBehaviour
{
	public Text m_WelcomeText;
	public Image m_LanguageButtonImage;
	public GameObject m_GermanText;
	public GameObject m_GermanTitleText;
	public GameObject m_EnglishText;
	public GameObject m_EnglishTitleText;
	public Sprite m_GermanFlag;
	public Sprite m_BritishFlag;

	private bool m_German = false;

	void Start()
	{
		m_LanguageButtonImage.sprite = m_GermanFlag;
		m_GermanText.SetActive(false);
		m_EnglishText.SetActive(true);
		m_GermanTitleText.SetActive(false);
		m_EnglishTitleText.SetActive(true);
		m_German = false;


	}
	public void SwapLanguage()
	{
		if (m_German)
		{
			m_LanguageButtonImage.sprite = m_GermanFlag;
			m_GermanText.SetActive(false);
			m_EnglishText.SetActive(true);
			m_GermanTitleText.SetActive(false);
			m_EnglishTitleText.SetActive(true);
			m_German = !m_German;
		}
		else
		{
			m_LanguageButtonImage.sprite = m_BritishFlag;
			m_GermanText.SetActive(true);
			m_EnglishText.SetActive(false);
			m_GermanTitleText.SetActive(true);
			m_EnglishTitleText.SetActive(false);
			m_German = !m_German;
		}
	}

}
