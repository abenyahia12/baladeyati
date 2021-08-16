using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/ScriptableTheme")]
[Serializable]
public class ScriptableTheme : ScriptableObject
{
    public Color themeColor;
    public ScriptableSubTheme[] scriptableSubThemes;
    public string themeTitle;
    public string videoName;
    public Sprite themeImage;
}
