using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WelcomeScript : MonoBehaviour
{

    public GameObject m_AccountManager;
    public GameObject m_TapToContinueButton;
    public GameObject m_AnimationGO;
    public float m_DelayPeriod;
    [Header("First Use")]
    [Tooltip("Tick this to simulate a first time use of a user")]
    private bool m_ResetFirstTime = false;

    void Start ()
    {

        if (Debug.isDebugBuild)
        {
            m_ResetFirstTime = true;
        }
        StartCoroutine(DelayTapToContinue());
        if (!PlayerPrefs.HasKey("HasPlayed"))
        { 
            PlayerPrefs.SetInt("HasPlayed", 0);
        }
        int hasPlayed = PlayerPrefs.GetInt("HasPlayed");

        if (hasPlayed == 0 || m_ResetFirstTime == true)
        {
            
            //StartCoroutine("Welcome");
          
        }
        else
        {
            m_AccountManager.SetActive(true);
            gameObject.SetActive(false);
          
        }
    }

    private IEnumerator DelayTapToContinue()
    {
        yield return new WaitForSeconds(m_DelayPeriod);
        m_TapToContinueButton.SetActive(true);

    }
  
    public void SkipWelcome()
    {
        m_AccountManager.SetActive(true);
        PlayerPrefs.SetInt("HasPlayed", 1);
        gameObject.SetActive(false);

    }
    
   
}
