using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WindowOrientationAdapter : MonoBehaviour {
    public GameObject m_ModalWindow;
    public Animator m_animator;
	public bool swap;
    public bool _wasPortrait = false;
    void Start()
    {
        if (((Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) ))
        {

            m_animator.SetBool("SwitchPortrait", true);
            m_animator.SetBool("SwitchLandscape", false);

        }
        else if (((Input.deviceOrientation == DeviceOrientation.LandscapeLeft) || (Input.deviceOrientation == DeviceOrientation.LandscapeRight)))
        {

            m_animator.SetBool("SwitchLandscape", true);
            m_animator.SetBool("SwitchPortrait", false);

        }
    }
   
 

       
        void Update () {
        AdaptToOrientation();
    }

   public void AdaptToOrientation()
    {

        if (((Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) || (Input.deviceOrientation == DeviceOrientation.Portrait)) )
        {

            m_animator.SetBool("SwitchPortrait",true);
            m_animator.SetBool("SwitchLandscape", false);

        }
        else if (((Input.deviceOrientation == DeviceOrientation.LandscapeLeft) || (Input.deviceOrientation == DeviceOrientation.LandscapeRight)) )
        {
       
            m_animator.SetBool("SwitchLandscape", true);
            m_animator.SetBool("SwitchPortrait", false);
       
        }

    }
	

}
