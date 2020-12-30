using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager : MonoBehaviour
{
    public CanvasGroup[] canvasGroups;
    public Transform ThemeParent;
    public Transform SubMawadhiParent;
    public Transform TicketsParent;
    public Transform MainTicketParent;
    public GameObject themePrefab;
    public GameObject subThemePrefab;
    public GameObject ticketPrefab;
    public GameObject ticketTextElementPrefab;
    public GameObject ticketPhotoElementPrefab;
    public ScriptableTheme[] scriptableThemes;
    private void Start()
    {
        GenerateThemes();
    }
    public void ActivateCanvasGroup(int index)
    {
        int i = 0;
        foreach (CanvasGroup item in canvasGroups)
        {
            if (i == index)
            {
                item.alpha = 1;
                item.blocksRaycasts = true;
                item.interactable = true;
            }
            else
            {
                item.alpha = 0;
                item.blocksRaycasts = false;
                item.interactable = false;
            }
            i++;
        }
    }
    void CleanParent(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateThemes()
    {
        CleanParent(ThemeParent);
        for (int i = 0; i < scriptableThemes.Length; i++)
        {
            CreateTheme(scriptableThemes[i]);
        }
    }
    void CreateTheme(ScriptableTheme scriptableTheme)
    {
        GameObject go = Instantiate(themePrefab, ThemeParent);
        ThemeView themeView = go.GetComponent<ThemeView>();
        themeView.ScriptableTheme = scriptableTheme;
        themeView.themeTitle.text = scriptableTheme.themeTitle;
        themeView.themeButton.onClick.AddListener(() => GenerateSubThemes(scriptableTheme.scriptableSubThemes));
    }
    void GenerateSubThemes(ScriptableSubTheme[] scriptableSubThemes)
    {
        CleanParent(SubMawadhiParent);
        for (int i = 0; i < scriptableSubThemes.Length; i++)
        {
            CreateSubTheme(scriptableSubThemes[i], i);
        }
        SwitchCanvastoSubThemes();
    }
    void CreateSubTheme(ScriptableSubTheme scriptableTheme,int index)
    {
        GameObject go = Instantiate(subThemePrefab, SubMawadhiParent);
        SubThemeView subthemeView = go.GetComponent<SubThemeView>();
        subthemeView.ScriptableSubTheme = scriptableTheme;
        subthemeView.subthemeTitle.text = scriptableTheme.subThemeTitle;
        subthemeView.subthemeNumberTitle.text = (index + 1).ToString(); ;
        subthemeView.ticketsNumberText.text = scriptableTheme.scriptableTickets.Length.ToString();
        subthemeView.SubThemeButton.onClick.AddListener(() => GenerateTickets(scriptableTheme.scriptableTickets));
    }
    void GenerateTickets(ScriptableTicket[] scriptableTickets)
    {
        CleanParent(TicketsParent);
        for (int i = 0; i < scriptableTickets.Length; i++)
        {
            CreateTicket(scriptableTickets[i], i);
        }
        SwitchCanvastoTickets();
    }
    void CreateTicket(ScriptableTicket scriptableTicket, int index)
    {
        GameObject go = Instantiate(ticketPrefab, TicketsParent);
        TicketView ticketView= go.GetComponent<TicketView>();
        ticketView.scriptableTicket = scriptableTicket;
        ticketView.TicketTitle.text = scriptableTicket.ticketTitle;
        ticketView.TicketNumberTitle.text = (index + 1).ToString();
        ticketView.TicketButton.onClick.AddListener(() => GenerateTicketElements(scriptableTicket.ticketElements));
    }
    void GenerateTicketElements(ticketElement[] ticketElements)
    {
        CleanParent(MainTicketParent);
        for (int i = 0; i < ticketElements.Length; i++)
        {
            CreateElement(ticketElements[i], i);
        }
        SwitchCanvastoTicketElements();
    }
    void CreateElement(ticketElement ticketElement, int index)
    {

        if (ticketElement.type == Element.Photo)
        {
            GameObject go = Instantiate(ticketPhotoElementPrefab, MainTicketParent);
            TicketElementView ticketElementView = go.GetComponent<TicketElementView>();
            ticketElementView.ticketElement = ticketElement;
            ticketElementView.image.sprite = ticketElement.photo;
        }
        else if (ticketElement.type == Element.Text)
        {
            GameObject go = Instantiate(ticketTextElementPrefab, MainTicketParent);
            TicketElementView ticketElementView = go.GetComponent<TicketElementView>();
            ticketElementView.ticketElement = ticketElement;
            ticketElementView.text.text = ticketElement.text;
        }
    }
    public void SwitchCanvastoSubThemes()
    {
        ActivateCanvasGroup(1);
    }
    public void SwitchCanvastoTickets()
    {
        ActivateCanvasGroup(2);
    }
    public void SwitchCanvastoTicketElements()
    {
        ActivateCanvasGroup(3);
    }
    public void SwitchCanvastoThemes()
    {
        ActivateCanvasGroup(0);
    }
}