using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/ScriptableSubTheme")]
[Serializable]
public class ScriptableSubTheme : ScriptableObject
{
    public ScriptableTicket[] scriptableTickets;
    public string subThemeTitle;
}
