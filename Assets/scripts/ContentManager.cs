using System.Collections;
using System.Collections.Generic;
using Paroxe.PdfRenderer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;
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
    public TextMeshProUGUI[] MenuMawdhiTexts;
    public Button[] MenuMawdhiButtons;
    public PDFViewer PDFViewer;
    public VideoManager videoManager;
    public Dictionary<string,GameObject> SearchObjs=new Dictionary<string, GameObject>();
    public TMP_InputField searchText;
    public GameObject loop;
    public TextMeshProUGUI SubThemesThemeTitle;
    public string subthemename;
    public string fileName =  "save.dat";
    public List<ScriptableTicket> tickets;
    private void Start()
    {
        GenerateThemes();
        SetupSideMenu();
        //SaveList();
        LoadeList();
    }
    void SaveList()
    {
        List<string> temp = new List<string>();
        //foreach (var item in tickets)
        //{
        //    temp.Add(item.ticketTitle);
        //}
        DataHandler.SaveList(temp, fileName);

    }
    void LoadeList()
    {
        List<string> temp = new List<string>();
        temp = DataHandler.LoadList(fileName);
        foreach (var item in temp)
        {
            Debug.Log(item);
        }
    }

    void SetupSideMenu()
    {
        for (int i = 0; i < scriptableThemes.Length; i++)
        {
            MenuMawdhiTexts[i].text = scriptableThemes[i].themeTitle;
        }
    }
    public void ActivateSearch(bool x)
    {
        searchText.gameObject.SetActive(x);
        loop.SetActive(x);
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
    void CleanSearchObjs()
    {
        SearchObjs.Clear();
    }
    public void Search()
    {
        foreach (var item in SearchObjs)
        {

            if (ArabicFixer.Fix(item.Key).Contains(searchText.text))
            {
                item.Value.SetActive(true);
            }
            else
            {
                item.Value.SetActive(false);
            }
        }
    }
    public void GenerateThemes()
    {
        CleanSearchObjs();
        CleanParent(ThemeParent);
        for (int i = 0; i < scriptableThemes.Length; i++)
        {
            CreateTheme(scriptableThemes[i]);
        }
        ActivateSearch(true);

    }
    void CreateTheme(ScriptableTheme scriptableTheme)
    {
        GameObject go = Instantiate(themePrefab, ThemeParent);
        ThemeView themeView = go.GetComponent<ThemeView>();
        themeView.themeImage.sprite = scriptableTheme.themeImage;
        themeView.ScriptableTheme = scriptableTheme;
        themeView.themeTitle.text = scriptableTheme.themeTitle;
        SearchObjs.Add(scriptableTheme.themeTitle, go);
        FixArabic(themeView.themeTitle);
        themeView.themeButton.onClick.AddListener(() => GenerateSubThemes(scriptableTheme.scriptableSubThemes, scriptableTheme.videoName, scriptableTheme.themeTitle));
    }
    void GenerateSubThemes(ScriptableSubTheme[] scriptableSubThemes,string videoName,string title)
    {
        SubThemesThemeTitle.text = title;
        FixArabic(SubThemesThemeTitle);
        CleanSearchObjs();
        CleanParent(SubMawadhiParent);
        for (int i = 0; i < scriptableSubThemes.Length; i++)
        {
            CreateSubTheme(scriptableSubThemes[i], i);
        }
        videoManager.SetupLink(videoName);
        ActivateSearch(true);
        SwitchCanvastoSubThemes();
    }
    public void GenerateSubThemes(int index)
    {
        CleanParent(SubMawadhiParent);
        CleanSearchObjs();
        for (int i = 0; i < scriptableThemes[index].scriptableSubThemes.Length; i++)
        {
            CreateSubTheme(scriptableThemes[index].scriptableSubThemes[i], i);
        }
        videoManager.SetupLink(scriptableThemes[index].videoName);
        ActivateSearch(true);
        CleanSearchObjs();
        SwitchCanvastoSubThemes();
    }
    void CreateSubTheme(ScriptableSubTheme scriptableSubTheme,int index)
    {
        GameObject go = Instantiate(subThemePrefab, SubMawadhiParent);
        SubThemeView subthemeView = go.GetComponent<SubThemeView>();
        subthemeView.SubThemeImage.sprite = scriptableSubTheme.subthemeImage;
        subthemeView.ScriptableSubTheme = scriptableSubTheme;
        subthemeView.subthemeTitle.text = scriptableSubTheme.subThemeTitle;
        SearchObjs.Add(scriptableSubTheme.subThemeTitle, go);
        FixArabic(subthemeView.subthemeTitle);
        subthemeView.subthemeNumberTitle.text =(index + 1).ToString(); ;
        subthemeView.SubThemeButton.onClick.AddListener(() => GenerateTickets(scriptableSubTheme.scriptableTickets));
    }
    void GenerateTickets(ScriptableTicket[] scriptableTickets)
    {
        CleanSearchObjs();
        CleanParent(TicketsParent);
        for (int i = 0; i < scriptableTickets.Length; i++)
        {
            CreateTicket(scriptableTickets[i]);
        }
        ActivateSearch(true);
        SwitchCanvastoTickets();
    }
    public void GenerateFavoriteTickets()
    {
        CleanSearchObjs();
        CleanParent(TicketsParent);
        List<string> favorites=DataHandler.LoadList(fileName);
        if(favorites.Count>0)
        { 
            for (int i = 0; i < tickets.Count; i++)
            {
                if (favorites.Contains(tickets[i].ticketTitle))
                { 
                CreateTicket(tickets[i]);
                }
            }
            SwitchCanvastoTickets();
        }
        ActivateSearch(true);
    }
    void CreateTicket(ScriptableTicket scriptableTicket)
    {
        GameObject go = Instantiate(ticketPrefab, TicketsParent);
        TicketView ticketView= go.GetComponent<TicketView>();
        if(scriptableTicket.ticketImage)
        ticketView.ticketImage.sprite = scriptableTicket.ticketImage;
        ticketView.scriptableTicket = scriptableTicket;
        ticketView.TicketTitle.text = scriptableTicket.ticketTitle;
        SearchObjs.Add(scriptableTicket.ticketTitle, go);
        FixArabic(ticketView.TicketTitle);
        ticketView.TicketButton.onClick.AddListener(() => ShowTicketPDF(scriptableTicket.ticketElement, scriptableTicket.ticketElement.videoName));
        bool x = checkIfLiked(ticketView.scriptableTicket.ticketTitle);
        ticketView.animator.SetBool("Pressed",x);
        ticketView.likeButton.onClick.AddListener(() => likeTicket(ticketView));
    }
    bool checkIfLiked(string title)
    {
        List<string> temp = new List<string>();
        temp = DataHandler.LoadList(fileName);
        return temp.Contains(title);
    }
    void likeTicket( TicketView ticketView )
    {
        List<string> temp = new List<string>();
        temp = DataHandler.LoadList(fileName);
        bool x = temp.Contains(ticketView.scriptableTicket.ticketTitle);
        ticketView.animator.SetBool("Pressed", !x);
        if (x)
        {
            temp.Remove(ticketView.scriptableTicket.ticketTitle);
        }
        else
        {
            temp.Add(ticketView.scriptableTicket.ticketTitle);
        }
        DataHandler.SaveList(temp, fileName);
    }
    void ShowTicketPDF(ticketElement ticketElement,string videoName)
    {
        CreateElement(ticketElement);
        videoManager.SetupLink(videoName);
        ActivateSearch(false);
        CleanSearchObjs();
        SwitchCanvastoTicketElement();
    }
    void CreateElement(ticketElement ticketElement)
    {
        PDFViewer.FileName = ticketElement.pdfTitle+".pdf";
        //if (ticketElement.type == Element.Photo)
        //{
        //    GameObject go = Instantiate(ticketPhotoElementPrefab, MainTicketParent);
        //    TicketElementView ticketElementView = go.GetComponent<TicketElementView>();
        //    ticketElementView.ticketElement = ticketElement;
        //    ticketElementView.image.sprite = ticketElement.photo;
        //}
        //else if (ticketElement.type == Element.Text)
        //{
        //    GameObject go = Instantiate(ticketTextElementPrefab, MainTicketParent);
        //    TicketElementView ticketElementView = go.GetComponent<TicketElementView>();
        //    ticketElementView.ticketElement = ticketElement;
        //    ticketElementView.text.text = ticketElement.text;
        //}
    }
    public void ActivatePDFVIEWER(bool x)
    {
        MainTicketParent.gameObject.SetActive(x);
    }
    public void SwitchCanvastoSubThemes()
    {
        ActivateCanvasGroup(1);
        ActivatePDFVIEWER(false);
    }
    public void SwitchCanvastoTickets()
    {
        ActivateCanvasGroup(2);
        ActivatePDFVIEWER(false);
    }
    public void SwitchCanvastoTicketElement()
    {
        ActivateCanvasGroup(3);
        ActivatePDFVIEWER(true);
    }
    public void SwitchCanvastoThemes()
    {
        ActivateSearch(true);
        ActivateCanvasGroup(0);
        ActivatePDFVIEWER(false);
    }
    public void FixArabic(TextMeshProUGUI textMeshProUGUI)
    {
        textMeshProUGUI.text = ArabicFixer.Fix(textMeshProUGUI.text, false, false);
    }
    public string FixArabic(string text)
    {
        return ArabicFixer.Fix(text, false, false);
    }
    public void FixArabicc(TMP_InputField tMP_InputField)
    {
        
        tMP_InputField.text = ArabicFixer.Fix(tMP_InputField.text, false, false);
    }
}