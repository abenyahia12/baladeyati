using UnityEngine;
using UnityEngine.UI;
public class UiAccountManager : MonoBehaviour
{
	public Canvas LoginCanvas;
	public Canvas ForgetCanvas;


	void Start()
	{
		LoginCanvas.enabled = true;
		ForgetCanvas.enabled = false;
	}


	public void ToggleCanvas(string open)
	{
		if (open == "login")
		{
			ForgetCanvas.enabled = false;
		}
		else if (open == "forget")
		{
			ForgetCanvas.enabled = true;
		}
	}


}
