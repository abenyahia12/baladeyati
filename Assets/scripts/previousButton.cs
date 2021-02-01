using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class previousButton : MonoBehaviour
{
    public ContentManager contentManager;
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                contentManager.SwitchCanvastoThemes();
                contentManager.GenerateThemes();
            }

        }
    }
}