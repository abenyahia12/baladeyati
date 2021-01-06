using Paroxe.PdfRenderer;
using Paroxe.PdfRenderer.Internal.Viewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PDFViewerHelper : UIBehaviour//, IPDFColoredRectListProviderForHightlights
{

    public struct pagesReps
    {
        public int n;
        public freeHandControl fhc;
    };

    public struct RectHightlights
    {
        public Rect rect;
        public int page;
        public Color color;
        public bool active;
        public List<Rect> innerRects;
        public RectHightlights(Rect _r, int _p, Color _c)
        {
            rect = _r;
            page = _p;
            color = _c;
            active = true;
            innerRects = new List<Rect>
            {
                rect
            };
        }
    }


    public struct RectPostIt
    {
        public Vector2 pos;
        public Vector2 size;
        public bool collapsed;
        public string text;
        public Color color;
        public int page;
        public int indexColor;
        public int id;
        public PDFPostItAdjust instanceOfPostIt;
        public RectPostIt(Vector2 _pos, Vector2 _size, int _page, bool _collapsed, string _text, Color _color, int _indexColor, int _id, float _z, PDFPostItAdjust _instanceOfPostIt)
        {
            pos = _pos;
            size = _size;
            page = _page;
            collapsed = _collapsed;
            text = _text;
            color = _color;
            id = _id;
            indexColor = _indexColor;
            instanceOfPostIt = _instanceOfPostIt;
        }
    }

    public List<RectPostIt> listOfPostIts;
    Vector2 oldDocumentSize, newDocumentSize;

    public List<GameObject> listOfIcons;
    public static List<RectHightlights> listOfHightlights;
    public List<freeHandControl.freeHandItems> listOfFreehandItems;

    public PDFViewer m_PdfViewer;
    public int m_PageWhereRadialMenuStarted;

    public bool m_IsDocumentDuplex;
    public Sprite m_IconToShow;
    public GameObject m_IconPrefab;
    float prevZoom = 0;
    bool setupAllComponents;
    public float initialZoom;
    int currentScreenWidth = -1;
    public int m_TypeOfOnScreenMenu, m_SubMenuOnScreen;
    public int m_XOnScreenMenu, m_YOnScreenMenu;
    public int m_XOnScreenSubMenu, m_YOnScreenSubMenu;
    GUIStyle styleSideBar;
    PDFViewerPageHelper[] m_PDFViewerPageHelper;
    RectTransform rtVerticalScrollBar;
    public int nOfSubMenus, indexSubMenus;
    public PDFPostItAdjust CurrentPostIt;
    public static bool m_MenuOnScreenActive = false;


    public PDFRadialInterface m_PDFRadialInterface;
    public PDFHightlight m_PDFHighlight;
    public PDFPostIt m_PDFPostit;

    public Texture m_OnScreenMenuTextureH;
    public Texture m_OnScreenMenuTextureP;

    public Texture m_OnScreenSubMenuTexture;

    public Texture[] m_TexturesOnScreenMenuActiveH;
    public Texture[] m_TexturesOnScreenMenuActiveP;
    public Texture[] m_TexturesOnScreenMenuActiveF;

    public Texture[] m_TexturesOnScreenMenuNormalH;
    public Texture[] m_TexturesOnScreenMenuNormalP;
    public Texture[] m_TexturesOnScreenMenuNormalF;

    public Texture[] m_TexturesOnScreenSubMenuActiveH;
    public Texture[] m_TexturesOnScreenSubMenuActiveP;
    public Texture[] m_TexturesOnScreenSubMenuActiveF;

    public Texture[] m_TexturesOnScreenSubMenuNormalH;
    public Texture[] m_TexturesOnScreenSubMenuNormalP;
    public Texture[] m_TexturesOnScreenSubMenuNormalF;

    public freeHandControl m_freeHandControl, m_freeHandControlPrev, m_freeHandControlNext;


    public enum OnScreenMenuAnimationState { HIDDEN, APPEARING, SHOWN, DISSAPEARING };
    public enum OnScreenSubMenuAnimationState { HIDDEN, APPEARING, SHOWN, DISSAPEARING };

    public OnScreenMenuAnimationState stateOnScreenMenu;
    public OnScreenSubMenuAnimationState stateOnScreenSubMenu;
    public OnScreenMenuAnimationState nextStateOnScreenMenu;

    public static int m_PageWhereSideBarWasInvoked;


    public void changeZoom(Vector2 vOld, Vector2 vNew, float currentZoom)
    {
        
        oldDocumentSize = vOld;
        newDocumentSize = vNew;

        newDocumentSize = m_PdfViewer.GetDocumentSize();

        //PDFViewerPageHelper[] m_PDFViewerPageHelper = GetComponentsInChildren<PDFViewerPageHelper>();

        //if (m_PDFViewerPageHelper[m_PageWhereRadialMenuStarted] != null)// && (m_PDFViewerPage.editMode == true))
        //{
            if ((m_PDFHighlight.m_MarkerLeft != null) && (m_PDFHighlight.m_MarkerRight != null))
            {
                m_PDFHighlight.UpdateMarkers2(currentZoom);
            }

            if (m_PDFRadialInterface.myMenu != null)
            {
                m_PDFRadialInterface.updateRadialMenu();
            }
        //}

        updateIconsPage(currentZoom);

        float x, y, w, h, pw, ph;

        for (int i = 0; i < listOfPostIts.Count; ++i)
        {
            RectPostIt item = listOfPostIts[i];

            RectTransform rt = item.instanceOfPostIt.GetComponent<RectTransform>();

            x = (rt.anchoredPosition.x / oldDocumentSize.x) * newDocumentSize.x;
            y = (rt.anchoredPosition.y / oldDocumentSize.y) * newDocumentSize.y;


            if (item.collapsed == false)
            {
                pw = rt.sizeDelta.x;
                ph = rt.sizeDelta.y;

                w = (pw / oldDocumentSize.x) * newDocumentSize.x;
                h = (ph / oldDocumentSize.y) * newDocumentSize.y;

                item.instanceOfPostIt.ResizePostIt(w - pw, h - ph);
            }
            else
            {
                pw = item.instanceOfPostIt.SizeCollapsedPostIt * m_PdfViewer.ZoomFactor;
                ph = item.instanceOfPostIt.SizeCollapsedPostIt * m_PdfViewer.ZoomFactor;

                item.instanceOfPostIt.rtMain.sizeDelta = new Vector2(pw, ph);
                item.instanceOfPostIt.background.rectTransform.sizeDelta = new Vector2(pw, ph);
                item.instanceOfPostIt.fake.rectTransform.sizeDelta = Vector2.zero;
            }
            rt.anchoredPosition = new Vector3(x, y, 0);

        }

        updateFreeHandElements();

        oldDocumentSize = newDocumentSize;
    }


    protected override void OnDisable()
    {
        removeAllPreviousComponents();
        m_PdfViewer.CloseDocument();
    }

    protected override void OnEnable()
    {
        if (prevZoom != 0)
            m_PdfViewer.ZoomFactor = prevZoom;

        setupAllComponents = false;

        m_TypeOfOnScreenMenu = -1;

        //m_PdfViewer.IsLoaded = false;

    }

    //void Start()
    protected override void Start()
    {

        listOfHightlights = new List<RectHightlights>();
        listOfHightlights.Clear();

        listOfIcons = new List<GameObject>();
        listOfIcons.Clear();

        listOfPostIts = new List<RectPostIt>();
        listOfPostIts.Clear();

        listOfFreehandItems = new List<freeHandControl.freeHandItems>();
        listOfFreehandItems.Clear();

        oldDocumentSize = new Vector2(0,0);

    }



    void removeAllPreviousComponents()
    {
        //delete post-its
        if (listOfPostIts != null)
        {
            RectPostIt p = new RectPostIt();
            for (int i = 0; i < listOfPostIts.Count; i++)
            {
                p = listOfPostIts[i];
                if (p.instanceOfPostIt.gameObject != null)
                    GameObject.Destroy(p.instanceOfPostIt.gameObject);
            }
            listOfPostIts.Clear();
        }

        RectTransform[] oldPostIts = GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < oldPostIts.Length; i++)
        {
            if (oldPostIts[i].name.Contains("PostItPrefab"))
                GameObject.Destroy(oldPostIts[i].gameObject);
        }

        //int numberOfPages = m_PdfViewer.Document.GetPageCount();
        int numberOfPages = m_PdfViewer.m_Internal.m_PageContainer.transform.childCount;
        for (int i = 0; i < numberOfPages; i++)
        {
            Transform cp = m_PdfViewer.m_Internal.m_PageContainer.GetChild(i) as Transform;
            RectTransform[] rt = cp.GetComponentsInChildren<RectTransform>();

            if (rt.Length > 1)
            {
                for (int j = 0; j < rt.Length; j++)
                {
                    if (rt[j].name.Contains("Page")==false)
                        GameObject.Destroy(rt[j].gameObject);
                }

            }

        }

    }

    void placeAllComponents()
    {

        listOfHightlights = SaveLoadHighlights.Load(m_PdfViewer.FileName + "-" + m_PdfViewer.DataBuffer.Length.ToString());

        if (listOfHightlights != null)
            m_PdfViewer.UpdateRenderer(0.1f);

        oldDocumentSize = m_PdfViewer.GetDocumentSize();

        PDFPostItAdjust.oldDocumentSize = oldDocumentSize;

        SetIcons();

        SetFreeHandPicturesFromLoad();

        SetPostItsFromLoad();

    }


    void OnGUI()
    {

        int wMenu = 128;

        int hMenu;
        if ((m_TypeOfOnScreenMenu == 1) || (m_TypeOfOnScreenMenu == 2))
            hMenu = 128 * 3;
        else
            hMenu = 128 * 4;

        int hIMenu = 128;
        int offset = 8;
        int speed = 3;
        int scrollBarWidth = 0;

        if (m_PdfViewer == null)
            m_PdfViewer = GetComponentInParent<PDFViewer>();

        if (rtVerticalScrollBar == null)
        {
            rtVerticalScrollBar = m_PdfViewer.m_Internal.m_VerticalScrollBar.GetComponent<RectTransform>();
        }
        scrollBarWidth = (int)((rtVerticalScrollBar.sizeDelta.x) / 1.75f);

        int newScrollBarWidth = (int)(((float)scrollBarWidth * (float)Screen.width) / 1024.0f);

        scrollBarWidth = newScrollBarWidth;


        if (Screen.width != currentScreenWidth)
        {
            m_XOnScreenMenu -= (currentScreenWidth - Screen.width);
            m_XOnScreenSubMenu -= (currentScreenWidth - Screen.width);
            currentScreenWidth = Screen.width;
        }

        if (styleSideBar == null)
        {
            styleSideBar = new GUIStyle(GUIStyle.none);
            styleSideBar.alignment = TextAnchor.MiddleCenter;
        }

        m_YOnScreenMenu = (Screen.height / 2) - (hMenu / 2);

        m_YOnScreenSubMenu = (Screen.height / 2) - (hMenu / 2) + (m_SubMenuOnScreen * hIMenu);

        switch (stateOnScreenMenu)
        {
            case OnScreenMenuAnimationState.HIDDEN:
                m_XOnScreenMenu = Screen.width;
                m_XOnScreenSubMenu = Screen.width;
                break;

            case OnScreenMenuAnimationState.APPEARING:
                m_XOnScreenMenu -= speed;

                if (m_XOnScreenMenu <= (Screen.width - wMenu - scrollBarWidth))
                {
                    m_XOnScreenMenu = (Screen.width - wMenu - scrollBarWidth);
                    stateOnScreenMenu = OnScreenMenuAnimationState.SHOWN;
                    stateOnScreenSubMenu = OnScreenSubMenuAnimationState.HIDDEN;
                }

                break;

            case OnScreenMenuAnimationState.SHOWN:

                switch (stateOnScreenSubMenu)
                {
                    case OnScreenSubMenuAnimationState.HIDDEN:
                        m_XOnScreenSubMenu = Screen.width;
                        break;

                    case OnScreenSubMenuAnimationState.APPEARING:
                        m_XOnScreenSubMenu -= speed * nOfSubMenus;

                        if (m_XOnScreenSubMenu <= m_XOnScreenMenu - (wMenu * nOfSubMenus))//(Screen.width - (wMenu * (nOfSubMenus + 1)) - (int)(rtVerticalScrollBar.sizeDelta.x / 2)) )
                        {
                            m_XOnScreenSubMenu = m_XOnScreenMenu - (wMenu * nOfSubMenus);// (Screen.width - (wMenu * (nOfSubMenus + 1) - (int)(rtVerticalScrollBar.sizeDelta.x / 2)));
                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.SHOWN;
                        }
                        break;

                    case OnScreenSubMenuAnimationState.SHOWN:

                        break;

                    case OnScreenSubMenuAnimationState.DISSAPEARING:
                        m_XOnScreenSubMenu += speed * nOfSubMenus;

                        if (m_XOnScreenSubMenu >= Screen.width)
                        {
                            m_XOnScreenSubMenu = Screen.width;
                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.HIDDEN;

                            if (nextStateOnScreenMenu != (OnScreenMenuAnimationState)(-1))
                            {
                                stateOnScreenMenu = nextStateOnScreenMenu;
                            }

                        }
                        break;
                }

                break;

            case OnScreenMenuAnimationState.DISSAPEARING:
                m_XOnScreenMenu += speed;

                if (m_XOnScreenMenu >= Screen.width)
                {
                    if (CurrentPostIt != null)
                    {
                        CurrentPostIt.setEdit(false);
                    }

                    m_XOnScreenMenu = Screen.width;
                    stateOnScreenMenu = OnScreenMenuAnimationState.HIDDEN;
                    m_MenuOnScreenActive = false;
                }
                break;
        }

        if ((stateOnScreenMenu != OnScreenMenuAnimationState.HIDDEN) && (stateOnScreenSubMenu != OnScreenSubMenuAnimationState.HIDDEN))
        {
            for (int j = 0; j < nOfSubMenus; j++)
            {
                if (m_TypeOfOnScreenMenu == 2)
                {
                    styleSideBar.active.background = (Texture2D)m_TexturesOnScreenSubMenuActiveH[indexSubMenus + j];
                    styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenSubMenuNormalH[indexSubMenus + j];
                }
                else if (m_TypeOfOnScreenMenu == 0)
                {
                    styleSideBar.active.background = (Texture2D)m_TexturesOnScreenSubMenuActiveP[indexSubMenus + j];
                    styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenSubMenuNormalP[indexSubMenus + j];
                }
                else if (m_TypeOfOnScreenMenu == 1)
                {
                    styleSideBar.active.background = (Texture2D)m_TexturesOnScreenSubMenuActiveF[indexSubMenus + j];
                    styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenSubMenuNormalF[indexSubMenus + j];
                }


                float alpha = m_XOnScreenSubMenu - (m_XOnScreenMenu - (wMenu * nOfSubMenus));// + (j * wMenu);
                alpha /= (m_XOnScreenMenu - (m_XOnScreenMenu - (wMenu * nOfSubMenus)));
                alpha = 1.0f - alpha;
                GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, alpha);

                if ((GUI.Button(new Rect(m_XOnScreenMenu - (wMenu * nOfSubMenus) + (j * wMenu), m_YOnScreenSubMenu + offset, wMenu - (offset * 2), hIMenu - (offset * 2)), "", styleSideBar)) &&
                (stateOnScreenSubMenu == OnScreenSubMenuAnimationState.SHOWN))
                {
                    switch (m_TypeOfOnScreenMenu)
                    {
                        case 2://highlights
                            switch (m_SubMenuOnScreen)
                            {

                                case 1:
                                    switch (j)
                                    {
                                        case 0:
                                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                            nextStateOnScreenMenu = OnScreenMenuAnimationState.DISSAPEARING;
                                            m_PDFHighlight.CancelHightlight();
                                            break;

                                        case 1:
                                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                            nextStateOnScreenMenu = OnScreenMenuAnimationState.SHOWN;
                                            break;
                                    }

                                    break;

                            }

                            break;
                        case 0://postits

                            switch (m_SubMenuOnScreen)
                            {

                                case 1://trashcan options

                                    switch (j)
                                    {
                                        case 0://save
                                            //m_PDFPostit.m_PostItAdjust.RealCloseFunction(0);
                                            CurrentPostIt.RealCloseFunction(0);
                                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                            nextStateOnScreenMenu = OnScreenMenuAnimationState.DISSAPEARING;
                                            break;

                                        case 1://cancel
                                            //m_PDFPostit.m_PostItAdjust.RealCloseFunction(1);
                                            CurrentPostIt.RealCloseFunction(1);
                                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                            nextStateOnScreenMenu = OnScreenMenuAnimationState.SHOWN;
                                            break;
                                    }


                                    break;

                            }

                            break;
                        case 1://freehand
                            switch (m_SubMenuOnScreen)
                            {

                                case 1://freehand options

                                    switch (j)
                                    {
                                        case 0://save

                                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                            nextStateOnScreenMenu = OnScreenMenuAnimationState.DISSAPEARING;

                                            break;

                                        case 1://cancel

                                            stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                            nextStateOnScreenMenu = OnScreenMenuAnimationState.SHOWN;
                                            break;
                                    }


                                    break;

                            }
                            break;

                    }

                }

            }

        }

        if (stateOnScreenMenu != OnScreenMenuAnimationState.HIDDEN)
        {
            int k;
            if ((m_TypeOfOnScreenMenu == 2) || (m_TypeOfOnScreenMenu == 1))
                k = 3;
            else
                k = 4;

            for (int i = 0; i < k; i++)
            {
                if (m_TypeOfOnScreenMenu == 2)
                {
                    styleSideBar.active.background = (Texture2D)m_TexturesOnScreenMenuActiveH[i];
                    styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenMenuNormalH[i];
                }
                else if (m_TypeOfOnScreenMenu == 0)
                {
                    styleSideBar.active.background = (Texture2D)m_TexturesOnScreenMenuActiveP[i];
                    styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenMenuNormalP[i];
                }
                else if (m_TypeOfOnScreenMenu == 1)
                {
                    styleSideBar.active.background = (Texture2D)m_TexturesOnScreenMenuActiveF[i];
                    styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenMenuNormalF[i];

                    if (i == 1)//second option // toggle
                    {
                        //if (PageToDrawCloned.GetComponent<freeHandControl>().getMode() == 0)//paint
                        if (m_freeHandControl.getMode() == 0)//paint
                        {
                            styleSideBar.active.background = (Texture2D)m_TexturesOnScreenMenuNormalF[i];
                            styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenMenuNormalF[i];
                        }
                        else//erase
                        {
                            styleSideBar.active.background = (Texture2D)m_TexturesOnScreenMenuActiveF[i];
                            styleSideBar.normal.background = (Texture2D)m_TexturesOnScreenMenuActiveF[i];
                        }
                    }
                }

                float alpha = m_XOnScreenMenu - (Screen.width - wMenu - scrollBarWidth);
                alpha /= (Screen.width - (Screen.width - wMenu - scrollBarWidth));
                alpha = 1.0f - alpha;
                GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, alpha);
                //if (GUI.Button(new Rect(m_XOnScreenMenu + offset, m_YOnScreenMenu + (i * hIMenu) + offset, wMenu - (offset * 2), hIMenu - (offset * 2)), "", styleSideBar))
                if ((GUI.Button(new Rect((Screen.width - wMenu - scrollBarWidth), m_YOnScreenMenu + (i * hIMenu) + offset, wMenu - (offset * 2), hIMenu - (offset * 2)), "", styleSideBar)) &&
                (stateOnScreenMenu == OnScreenMenuAnimationState.SHOWN))
                {
                    switch (m_TypeOfOnScreenMenu)
                    {
                        case 2://hightlights
                            switch (i)
                            {
                                case 0://ok

                                    stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                    nextStateOnScreenMenu = OnScreenMenuAnimationState.DISSAPEARING;

                                    m_PDFHighlight.SaveHightlight();

                                    break;

                                case 1://cancel

                                    if (stateOnScreenSubMenu == OnScreenSubMenuAnimationState.HIDDEN)
                                    {
                                        stateOnScreenSubMenu = OnScreenSubMenuAnimationState.APPEARING;
                                        nOfSubMenus = 2;
                                        indexSubMenus = 0;
                                        m_SubMenuOnScreen = 1;
                                        m_XOnScreenSubMenu = Screen.width - hIMenu;
                                        m_YOnScreenSubMenu = (Screen.height / 2) - (hMenu / 2) + (m_SubMenuOnScreen * hIMenu);
                                    }
                                    else
                                    {
                                        stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                        nextStateOnScreenMenu = (OnScreenMenuAnimationState)(-1);
                                    }


                                    break;

                                case 2://color selection

                                    m_PDFHighlight.ChooseColor((m_PDFHighlight.GetIndexColor() + 1) % 4);

                                    stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                    nextStateOnScreenMenu = (OnScreenMenuAnimationState)(-1);

                                    break;
                            }
                            break;

                        case 0://postits
                            switch (i)
                            {
                                case 0://confirm

                                    CurrentPostIt.RealCloseFunction(1);
                                    stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                    nextStateOnScreenMenu = OnScreenMenuAnimationState.DISSAPEARING;

                                    break;

                                case 1://trashcan

                                    if (stateOnScreenSubMenu == OnScreenSubMenuAnimationState.HIDDEN)
                                    {
                                        stateOnScreenSubMenu = OnScreenSubMenuAnimationState.APPEARING;
                                        nOfSubMenus = 2;
                                        indexSubMenus = 0;
                                        m_SubMenuOnScreen = 1;
                                        m_XOnScreenSubMenu = Screen.width - hIMenu;
                                        m_YOnScreenSubMenu = (Screen.height / 2) - (hMenu / 2) + (m_SubMenuOnScreen * hIMenu);
                                    }
                                    else
                                    {
                                        stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                        nextStateOnScreenMenu = (OnScreenMenuAnimationState)(-1);
                                    }

                                    break;

                                case 2://minimize

                                    stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                    nextStateOnScreenMenu = OnScreenMenuAnimationState.DISSAPEARING;

                                    //m_PDFPostit.m_PostItAdjust.CollapseFunction();
                                    CurrentPostIt.CollapseFunction();

                                    break;

                                case 3://color selection

                                    CurrentPostIt.ColorRadialMenuButton((CurrentPostIt.indexColor + 1) % 4);

                                    stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                    nextStateOnScreenMenu = (OnScreenMenuAnimationState)(-1);

                                    break;
                            }

                            break;

                        case 1://freehand
                            switch (i)
                            {
                                case 0://ok

                                    m_freeHandControl.setEditing(false);

                                    RawImage ii = m_freeHandControl.GetComponent<RawImage>();
                                    ii.raycastTarget = false;

                                    if (m_freeHandControlPrev != null)
                                    {
                                        RawImage iip = m_freeHandControlPrev.GetComponent<RawImage>();
                                        iip.raycastTarget = false;
                                    }
                                    if (m_freeHandControlNext != null)
                                    {
                                        RawImage iin = m_freeHandControlNext.GetComponent<RawImage>();
                                        iin.raycastTarget = false;
                                    }

                                    stateOnScreenSubMenu = OnScreenSubMenuAnimationState.DISSAPEARING;
                                    nextStateOnScreenMenu = OnScreenMenuAnimationState.DISSAPEARING;

                                    m_freeHandControl.Save();

                                    m_PdfViewer.m_AllowedToPinchZoom = true;

                                    //m_freeHandControl.Save();

                                    break;

                                case 1://toogle between painting and erasing

                                    //PageToDrawCloned.GetComponent<freeHandControl>().toggleMode();
                                    m_freeHandControl.toggleMode();

                                    break;

                                case 2://color selection

                                    //PageToDrawCloned.GetComponent<freeHandControl>().cycleColor();
                                    m_freeHandControl.cycleColor();

                                    break;
                            }

                            break;
                    }

                }

            }

        }

    }


    void Update()
    {

        if ((m_PdfViewer.IsLoaded == true) && (setupAllComponents == false))
        {
            prevZoom = m_PdfViewer.ZoomFactor;
            setupAllComponents = true;
            placeAllComponents();
        }

    }

    bool pageRepsContains(List<pagesReps> donePages, int p)
    {
        bool found = false;

        for (int i=0;i<donePages.Count;i++)
        {
            pagesReps pr = donePages[i];
            if (pr.n == p)
            {
                found = true;
                break;
            }
        }

        return found;
    }

    freeHandControl getFhcFromList(List<pagesReps> donePages, int p)
    {
        freeHandControl ret = null;

        for (int i = 0; i < donePages.Count; i++)
        {
            pagesReps pr = donePages[i];
            if (pr.n == p)
            {
                ret = pr.fhc;
                break;
            }
        }

        return ret;
    }


    private void SetIcons()
    {

        bool duplex = m_IsDocumentDuplex;// | m_ForceDuplex;

        int W, H, w, h, x, y;
        W = (int)((float)m_PdfViewer.Document.GetPageWidth(0) * m_PdfViewer.ZoomFactor);
        H = (int)((float)m_PdfViewer.Document.GetPageHeight(0) * m_PdfViewer.ZoomFactor);

        w = (int)((float)W * (472.0f / 2481.0f));
        h = (int)((float)H * (185.0f / 3508.0f));

        x = (int)((float)W * (184.0f / 2481.0f));
        y = (int)((float)H * (133.0f / 3508.0f));

        Vector2 pos1, pos2, size;
        pos1 = new Vector2(-W / 2 + x + w, H / 2 - y - h);
        pos2 = new Vector2(W / 2 - x - w, H / 2 - y - h);
        size = new Vector2(w, h);

        listOfIcons.Clear();

        int numberOfPages = m_PdfViewer.m_Internal.m_PageContainer.transform.childCount;

        for (int i = 1; i < numberOfPages; i++)
        {

            Transform currentPage = m_PdfViewer.m_Internal.m_PageContainer.GetChild(i) as Transform;

            GameObject newIcon = (GameObject)Instantiate(m_IconPrefab) as GameObject;

            newIcon.transform.SetParent(currentPage.transform);

            if (!duplex)
                newIcon.transform.localPosition = pos1;
            else
            {
                if ((i & 1) == 1)
                    newIcon.transform.localPosition = pos1;
                else
                    newIcon.transform.localPosition = pos2;
            }

            Image img = newIcon.GetComponent<Image>();
            img.sprite = m_IconToShow;
            img.preserveAspect = true;
            newIcon.GetComponent<RectTransform>().sizeDelta = size;

            listOfIcons.Add(newIcon);

        }

    }

    void SetFreeHandPicturesFromLoad()
    {
        initialZoom = m_PdfViewer.ZoomFactor;

        listOfFreehandItems = SaveLoadFreehand.Load(m_PdfViewer.FileName + "-" + m_PdfViewer.DataBuffer.Length.ToString());// FileName + "-" + DataBuffer.Length.ToString());

        int W, H;
        W = (int)((float)m_PdfViewer.Document.GetPageWidth(0) * m_PdfViewer.ZoomFactor);
        H = (int)((float)m_PdfViewer.Document.GetPageHeight(0) * m_PdfViewer.ZoomFactor);

        int numberOfPages = m_PdfViewer.m_Internal.m_PageContainer.transform.childCount;

        List<freeHandControl.CLines> list = new List<freeHandControl.CLines>();

        PDFViewerPageHelper[] m_PDFViewerPageHelper = GetComponentsInChildren<PDFViewerPageHelper>();

        list.Clear();

        List<pagesReps> donePages = new List<pagesReps>();
        donePages.Clear();

        GameObject newPageToPaint;

        for (int j = 0;j < listOfFreehandItems.Count;j++)
        {

            freeHandControl.freeHandItems item = listOfFreehandItems[j];

            freeHandControl fhc;

            if (pageRepsContains(donePages, item.page) ==false)
            {
                
                Transform currentPage = m_PdfViewer.m_Internal.m_PageContainer.GetChild(item.page) as Transform;

                newPageToPaint = (GameObject)Instantiate(m_PDFViewerPageHelper[item.page].m_PageToDraw) as GameObject;

                newPageToPaint.transform.SetParent(currentPage.transform);

                newPageToPaint.transform.localPosition = new Vector2(0, 0);
                newPageToPaint.transform.localScale = new Vector3(1, 1, 1);

                RawImage img = newPageToPaint.GetComponent<RawImage>();
                img.raycastTarget = false;
                img.color = new Color(1, 1, 1, 1);

                Texture2D t2d = new Texture2D((int)((float)m_PdfViewer.Document.GetPageWidth(0) * m_PdfViewer.ZoomFactor), (int)((float)m_PdfViewer.Document.GetPageHeight(0) * m_PdfViewer.ZoomFactor), TextureFormat.RGBA32, false);

                img.texture = t2d;

                fhc = newPageToPaint.GetComponent<freeHandControl>();

                newPageToPaint.GetComponent<RectTransform>().sizeDelta = new Vector2(W, H);

                fhc.theTexture = t2d;

                fhc.init(currentPage);

                pagesReps dp = new pagesReps();
                dp.n = item.page;
                dp.fhc = fhc;

                donePages.Add(dp);
            }
            else
            {
                newPageToPaint = m_PDFViewerPageHelper[item.page].m_PageToDraw;
                fhc = getFhcFromList(donePages, item.page);
            }

            freeHandControl.CLines lines = new freeHandControl.CLines();
            lines.color = freeHandControl.cycleColors[item.color];
            lines.xMin = item.xMin;
            lines.xMax = item.xMax;
            lines.yMin = item.yMin;
            lines.yMax = item.yMax;
            lines.listOfPoints = item.listOfPoints;

            fhc.setLinesFromLoad(lines);
            fhc.setHelper(this);

        }

    }


    void SetPostItsFromLoad()
    {

        listOfPostIts = SaveLoadPostIt.Load(m_PdfViewer.FileName + "-" + m_PdfViewer.DataBuffer.Length.ToString());

        PDFViewerPageHelper[] m_PDFViewerPageHelper = GetComponentsInChildren<PDFViewerPageHelper>();

        PDFPostIt pdfPostIt = m_PdfViewer.m_Internal.m_PageContainer.GetComponentInChildren<PDFPostIt>();
        pdfPostIt.m_PDFViewer = m_PdfViewer;

        PDFPage m_Page;

        int n = m_PdfViewer.m_Internal.m_PageContainer.transform.childCount;

        m_Page = m_PdfViewer.Document.GetPage(n-1);// transform.GetSiblingIndex());

        Vector2 sizePage = m_Page.GetPageSize();

        for (int i = 0; i < listOfPostIts.Count; ++i)
        {
            RectPostIt item = listOfPostIts[i];
            PDFPostItAdjust k = pdfPostIt.DoThePostit(m_PDFViewerPageHelper[item.page], m_PDFViewerPageHelper[item.page].GetType(), item, false);

            Vector2 sizePostIt = new Vector2((sizePage.x * 15) / 100, (sizePage.y * 10) / 100);

            k.MinWidth = sizePostIt.x;
            k.MinHeight = sizePostIt.y;

            k.setEdit(false);
        }

    }


    public void AddPostIt(Vector2 pos, Vector2 size, int page, bool collapsed, string text, Color color, int indexColor, int id, float zoom, PDFPostItAdjust instanceOfPostIt)
    {
        float w, h, x, y;
        Vector2 newDocumentSize = m_PdfViewer.GetDocumentSize();


        RectPostIt newP = new RectPostIt(pos, size, page, collapsed, text, color, indexColor, id, zoom, instanceOfPostIt);

        newP.size = size;// rtMain.sizeDelta;

        w = (newP.size.x / newDocumentSize.x) * oldDocumentSize.x;
        h = (newP.size.y / newDocumentSize.y) * oldDocumentSize.y;

        newP.size = new Vector2(w, h);

        newP.pos = pos;// rtMain.localPosition;

        x = (newP.pos.x / newDocumentSize.x) * oldDocumentSize.x;
        y = (newP.pos.y / newDocumentSize.y) * oldDocumentSize.y;

        newP.pos = new Vector2(x, y);

        listOfPostIts.Add(newP);

        //SaveLoadPostIt.Save(listOfPostIts, FileName);
        SaveLoadPostIt.Save(listOfPostIts, m_PdfViewer.FileName + "-" + m_PdfViewer.DataBuffer.Length.ToString());
    }


    private void updateIconsPage(float z)
    {
        int W, H, w, h, x, y;
        W = (int)((float)m_PdfViewer.Document.GetPageWidth(0) * z);
        H = (int)((float)m_PdfViewer.Document.GetPageHeight(0) * z);

        w = (int)((float)W * (472.0f / 2481.0f));
        h = (int)((float)H * (185.0f / 3508.0f));

        x = (int)((float)W * (184.0f / 2481.0f));
        y = (int)((float)H * (133.0f / 3508.0f));

        Vector2 pos1, pos2, size;
        pos2 = new Vector2(-W / 2 + x + w, H / 2 - y - h);
        pos1 = new Vector2(W / 2 - x - w, H / 2 - y - h);
        size = new Vector2(w, h);

        bool duplex = m_IsDocumentDuplex;// | m_ForceDuplex;

        for (int i = 0; i < listOfIcons.Count; ++i)
        {
            if (!duplex)
                listOfIcons[i].transform.localPosition = pos1;
            else
            {
                if ((i & 1) == 1)
                {
                    listOfIcons[i].transform.localPosition = pos1;
                }
                else
                {
                    listOfIcons[i].transform.localPosition = pos2;
                }
            }
            listOfIcons[i].GetComponent<RectTransform>().sizeDelta = size;

        }
    }

    private void updateFreeHandElements()
    {
        
        PDFViewerPage[] m_PDFViewerPage = m_PdfViewer.GetComponentsInChildren<PDFViewerPage>();

        for (int i = 0;i < m_PDFViewerPage.Length;i++)
        {

            Transform currentPage = m_PdfViewer.m_Internal.m_PageContainer.GetChild(i) as Transform;

            RawImage [] images = currentPage.GetComponentsInChildren<RawImage>();

            for (int j=0;j<images.Length;j++)
            {
                if (images[j].name.Contains("prefabFreeHand"))
                {
                    images[j].rectTransform.sizeDelta = m_PDFViewerPage[i].GetComponent<RectTransform>().sizeDelta;
                }
            }

            //RectTransform rt = m_PDFViewerPage[i].GetComponentInChildren<RectTransform>();
            //i += 0;
            //if ((go!=null) && (go.name.Contains("prefabFreeHand")))
            //{v
            //    go.GetComponent<RectTransform>().sizeDelta = m_PDFViewerPage[i].GetComponent<RectTransform>().sizeDelta;
            //}
        }

    }



    public void AddHightlight(Rect newRect, int page, Color c)
    {
        RectHightlights newH = new RectHightlights(newRect, page, c);
        listOfHightlights.Add(newH);
        m_PdfViewer.UpdateRenderer(0.25f);
    }


    public static IList<PDFColoredRect> GetBackgroundColoredRectListForHighlight(PDFPage page)
    {
#if !UNITY_WEBGL

        //PDFViewerPage m_PDFViewerPage = GetComponentInChildren<PDFViewerPage>();

        if ((listOfHightlights != null) && (listOfHightlights.Count > 0))
        {

            using (PDFTextPage textPage = page.GetTextPage())
            {
                List<PDFColoredRect> coloredRectList = new List<PDFColoredRect>();

                for (int i = 0; i < listOfHightlights.Count; i++)
                {
                    RectHightlights newH = listOfHightlights[i];

                    if (newH.page == page.PageIndex)
                    {
                        coloredRectList.Add(new PDFColoredRect(listOfHightlights[i].rect, listOfHightlights[i].color, true, listOfHightlights[i].active));//whole rectangle

                        for (int j = 0; j < listOfHightlights[i].innerRects.Count; j++)
                            coloredRectList.Add(new PDFColoredRect(listOfHightlights[i].innerRects[j], listOfHightlights[i].color));//each line
                    }
                }

                return coloredRectList;
            }
        }
#endif

        return null;
    }

}
