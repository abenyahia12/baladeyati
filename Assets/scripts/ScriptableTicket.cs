using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
[Serializable]
public enum Element
{
    Text,
    Photo
}
[Serializable]
public struct ticketElement
{
    public Element type;    
    public Sprite photo;
    public string text;
}
[CreateAssetMenu(menuName = "My Assets/ScriptableTicket")]
[Serializable]
public class ScriptableTicket : ScriptableObject
{
    public string ticketTitle;
    public ticketElement[] ticketElements;
}
