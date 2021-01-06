using Paroxe.PdfRenderer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PDFViewerPageHelper : UIBehaviour, IPointerClickHandler, IPointerUpHandler , IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject m_PageToDraw;
    GameObject drawingPage;

    public PDFViewer m_PDFViewer;
    private float holdTime = 0.5f;
    int m_PagesToDismiss;
    //public int m_TypeOfOnScreenMenu, m_SubMenuOnScreen;
    
    bool m_ComingFromLongPress;
    //GUIStyle styleSideBar;

    Vector3 m_PositionWhenLongPress;
    PointerEventData m_EventDataWhenInvoked;
    //PDFPostItAdjust CurrentPostIt;
    //freeHandControl m_freeHandControl, m_freeHandControlPrev, m_freeHandControlNext;

    
    //public PDFRadialInterface m_PDFRadialInterface;
    //public PDFHightlight m_PDFHighlight;
    //public PDFPostIt m_PDFPostit;
    

    public PDFPage m_Page;

    /*
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
    */

    PDFViewerHelper m_PDFViewerHelper;

    //RectTransform rtVerticalScrollBar;
    //int currentScreenWidth = -1;


    /*
    public enum OnScreenMenuAnimationState { HIDDEN, APPEARING, SHOWN, DISSAPEARING };
    enum OnScreenSubMenuAnimationState { HIDDEN, APPEARING, SHOWN, DISSAPEARING };

    public OnScreenMenuAnimationState stateOnScreenMenu;
    OnScreenSubMenuAnimationState stateOnScreenSubMenu;
    OnScreenMenuAnimationState nextStateOnScreenMenu;
    */
    //static bool m_MenuOnScreenActive = false;
    //int m_XOnScreenMenu, m_YOnScreenMenu;
    //int m_XOnScreenSubMenu, m_YOnScreenSubMenu;
    //int nOfSubMenus, indexSubMenus;


    // Use this for initialization
    protected override void OnEnable()
    {
        if (m_PDFViewer == null)
            m_PDFViewer = GetComponentInParent<PDFViewer>();

        resetSideBar();

        m_PagesToDismiss = 2;

        //m_TypeOfOnScreenMenu = -1;

        m_PDFViewer.m_AllowedToPinchZoom = true;
    }

    protected override void OnDisable()
    {
        PDFRadialInterface.IsActive = false;

        if (PDFViewerHelper.m_MenuOnScreenActive ==  true)//leaving with the free hand tool on!
        {
            if (m_PDFViewerHelper.m_TypeOfOnScreenMenu == 1)//freehand tool
            {
                if (m_PDFViewerHelper.m_freeHandControl != null)
                {
                    //m_PDFViewer.CleanUp();

                    m_PDFViewerHelper.m_freeHandControl.hasMadeChanges = true;
                    //m_freeHandControl.Save();

                    m_PDFViewerHelper.m_freeHandControl.setEditing(false);

                    RawImage ii = m_PDFViewerHelper.m_freeHandControl.GetComponent<RawImage>();
                    ii.raycastTarget = false;

                    if (m_PDFViewerHelper.m_freeHandControlPrev != null)
                    {
                        RawImage iip = m_PDFViewerHelper.m_freeHandControlPrev.GetComponent<RawImage>();
                        iip.raycastTarget = false;
                    }
                    if (m_PDFViewerHelper.m_freeHandControlNext != null)
                    {
                        RawImage iin = m_PDFViewerHelper.m_freeHandControlNext.GetComponent<RawImage>();
                        iin.raycastTarget = false;
                    }

                    m_PDFViewerHelper.stateOnScreenSubMenu = PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING;
                    m_PDFViewerHelper.nextStateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING;

                    m_PDFViewerHelper.m_freeHandControl.Save();

                    m_PDFViewer.m_AllowedToPinchZoom = true;
                }


            }
            else if(m_PDFViewerHelper.m_TypeOfOnScreenMenu == 0)//post it
            {
                if (m_PDFViewerHelper.CurrentPostIt !=null)
                    m_PDFViewerHelper.CurrentPostIt.RealCloseFunction(1);

                m_PDFViewerHelper.stateOnScreenSubMenu = PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING;
                m_PDFViewerHelper.nextStateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING;

            }
            else if (m_PDFViewerHelper.m_TypeOfOnScreenMenu == 2)//highlights
            {
                m_PDFViewerHelper.stateOnScreenSubMenu = PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING;
                m_PDFViewerHelper.nextStateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING;

                if (m_PDFViewerHelper.m_PDFHighlight !=null)
                    m_PDFViewerHelper.m_PDFHighlight.SaveHightlight();
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {

        if ((m_PDFViewerHelper==null) && (m_PDFViewer!=null))
            m_PDFViewerHelper = m_PDFViewer.GetComponent<PDFViewerHelper>();

        if ((m_PDFViewer != null) && (m_PDFViewerHelper.m_PDFRadialInterface.myMenu != null))
        {
            int currentPage = m_PDFViewer.CurrentPageIndex;

            if (Math.Abs(m_PDFViewerHelper.m_PDFRadialInterface.pageIndexWhereRadialMenuHasStarted - currentPage) >= m_PagesToDismiss)
            {


                m_PDFViewerHelper.m_PDFHighlight.CancelHightlight();
                m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();
            }
        }
        
        if ((m_PDFViewerHelper.stateOnScreenMenu == PDFViewerHelper.OnScreenMenuAnimationState.SHOWN) || (m_PDFViewerHelper.stateOnScreenMenu == PDFViewerHelper.OnScreenMenuAnimationState.APPEARING))
        {
            int currentPage = m_PDFViewer.CurrentPageIndex;
            if (Math.Abs(PDFViewerHelper.m_PageWhereSideBarWasInvoked - currentPage) >= m_PagesToDismiss)
            {
                m_PDFViewerHelper.m_PDFHighlight.CancelHightlight();
                m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();

                if (m_PDFViewerHelper.m_TypeOfOnScreenMenu == 1)
                {
                    m_PDFViewerHelper.m_freeHandControl.setEditing(false);
                    RawImage ii = m_PDFViewerHelper.m_freeHandControl.GetComponent<RawImage>();
                    ii.raycastTarget = false;
                    m_PDFViewerHelper.stateOnScreenSubMenu = PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING;
                    m_PDFViewerHelper.nextStateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING;
                    m_PDFViewer.m_AllowedToPinchZoom = true;
                }

                m_PDFViewerHelper.stateOnScreenSubMenu = PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING;
                m_PDFViewerHelper.nextStateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING;
            }
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        Invoke("OnLongPress", holdTime);
        //Debug.Log("OnPointerDown");
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
        CancelInvoke("OnLongPress");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");
        m_EventDataWhenInvoked = eventData;
        if (m_PDFViewer == null)
        {
            m_PDFViewer = GetComponentInParent<PDFViewer>();
        }

        if (m_PDFViewer.Document == null)
        {
            return;
        }

        if (m_Page == null)
            m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit");
        m_EventDataWhenInvoked = eventData;
        //CancelInvoke("OnLongPress");

        if (m_Page != null)
            m_Page.Dispose();

        m_Page = null;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick");
        if (m_ComingFromLongPress == false)
        {
            if (m_Page == null)
                m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if ((PDFRadialInterface.IsActive == false) && (PDFViewerHelper.m_MenuOnScreenActive == false))
                {
                    using (PDFTextPage textPage = m_Page.GetTextPage())
                    {
                        Vector2 pos = eventData.pressPosition;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, pos, GetComponent<Camera>(), out pos);
                        RectTransform rt = transform as RectTransform;
                        pos += rt.sizeDelta.x * 0.5f * Vector2.right;
                        pos += rt.sizeDelta.y * 0.5f * Vector2.up;
                        pos = pos.x * (rt.sizeDelta.y / rt.sizeDelta.x) * Vector2.right + pos.y * Vector2.up;

                        Vector2 pagePoint = m_Page.DeviceToPage(0, 0, (int)rt.sizeDelta.y, (int)rt.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos.x, (int)pos.y);
                        Vector2 pageSize = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

                        bool found = false;

                        for (int i = 0; i < PDFViewerHelper.listOfHightlights.Count; i++)
                        {
                            PDFViewerHelper.RectHightlights item = PDFViewerHelper.listOfHightlights[i];
                            for (int j = 0; j < item.innerRects.Count; j++)
                            {
                                if ((pagePoint.x > item.innerRects[j].xMin) && (pagePoint.x < item.innerRects[j].xMax) && (pagePoint.y < item.innerRects[j].yMin) && (pagePoint.y > (item.innerRects[j].yMin - item.innerRects[j].height)))
                                {
                                    m_PDFViewerHelper.m_PDFHighlight.DoTheTable(textPage, m_Page.PageIndex);

                                    //m_PDFHighlight.highlightedTextToEdit = i;
                                    m_PDFViewerHelper.m_PDFHighlight.SetHighlightedTextToEdit(i);
                                    m_PDFViewerHelper.m_PDFHighlight.PutMarkers(m_Page.PageIndex, i, pageSize, item.rect.xMin, item.rect.xMax, item.rect.yMin, item.rect.yMin - item.rect.height);
                                    found = true;


                                    m_PDFViewerHelper.stateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.APPEARING;
                                    PDFViewerHelper.m_PageWhereSideBarWasInvoked = transform.GetSiblingIndex();

                                    m_PDFViewerHelper.m_TypeOfOnScreenMenu = 2;//2=highlight, 0=postit
                                    PDFViewerHelper.m_MenuOnScreenActive = true;


                                    m_PDFViewerHelper.m_PDFHighlight.UpdateMarkers(true);

                                    break;
                                }
                            }
                            if (found == true)
                                break;
                        }


                    }
                }
                else// if ((PressSideBarButton == false) && (m_MenuOnScreenActive == true) && (stateOnScreenMenu != OnScreenMenuAnimationState.DISSAPEARING) && (stateOnScreenSubMenu != OnScreenSubMenuAnimationState.DISSAPEARING))
                {
                    bool k = false;
                    int wMenu = 128;
                    //int hMenu = 128 * 3;
                    int hIMenu = 128;
                    int offset = 8;

                    Vector2 pos = eventData.pressPosition;

                    for (int i = 0; i < 3; i++)
                    {
                        if ((pos.x > (m_PDFViewerHelper.m_XOnScreenMenu + offset)) && (pos.x < (m_PDFViewerHelper.m_XOnScreenMenu + offset + wMenu)) && (pos.y > (m_PDFViewerHelper.m_YOnScreenMenu + (i * hIMenu) + offset)) && (pos.y < (m_PDFViewerHelper.m_YOnScreenMenu + (i * hIMenu) + offset + hIMenu)))
                        {
                            k = true;
                            break;
                        }
                    }
                    for (int j = 0; j < m_PDFViewerHelper.nOfSubMenus; j++)
                    {
                        if ((pos.x > (m_PDFViewerHelper.m_XOnScreenSubMenu + (j * hIMenu) + offset)) && (pos.x < (m_PDFViewerHelper.m_XOnScreenSubMenu + (j * hIMenu) + offset + wMenu)) && (pos.y > m_PDFViewerHelper.m_YOnScreenSubMenu + offset) && (pos.y < (m_PDFViewerHelper.m_YOnScreenSubMenu + hIMenu - offset * 2)))
                        {
                            k = true;
                            break;
                        }
                    }
                    if ((k == false) && (PDFViewerHelper.m_MenuOnScreenActive == true) && (m_PDFViewerHelper.stateOnScreenMenu != PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING) && (m_PDFViewerHelper.stateOnScreenSubMenu != PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING) && (m_PDFViewerHelper.stateOnScreenMenu != PDFViewerHelper.OnScreenMenuAnimationState.SHOWN))
                    {
                        m_PDFViewerHelper.stateOnScreenSubMenu = PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING;
                        m_PDFViewerHelper.nextStateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING;
                        if (m_PDFViewerHelper.m_PDFHighlight.m_MarkersOn == true)
                        {
                            m_PDFViewerHelper.m_PDFHighlight.CancelHightlight();
                        }
                    }
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if ((PDFRadialInterface.IsActive == false) && (PDFViewerHelper.m_MenuOnScreenActive == false))
                {
                    if (m_PDFViewer == null)
                        m_PDFViewer = GetComponentInParent<PDFViewer>();
                    if (m_Page == null)
                        m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());

                    bool k = EditHighlight(m_Page, eventData.pressPosition);

                    if (k == false)
                    {
                        Texture2D[] textures = new Texture2D[4];
                        textures[0] = m_PDFViewerHelper.m_PDFRadialInterface.ImagePostIt;
                        textures[1] = m_PDFViewerHelper.m_PDFRadialInterface.ImageFreeHand;
                        textures[2] = m_PDFViewerHelper.m_PDFRadialInterface.ImageHightlight;
                        textures[3] = m_PDFViewerHelper.m_PDFRadialInterface.ImageCancel;
                        m_PDFViewerHelper.m_PDFRadialInterface.CreateRadialMenu(this, GetType(), "MainRadialMenuButton", eventData.pressPosition, PDFRadialInterface.RadialMenuTypes.THREE, m_PDFViewer.ScaleFactorMenu, true, 0.5f, textures, false);

                        m_PositionWhenLongPress = eventData.pressPosition;
                    }
                }
            }
        }
        m_ComingFromLongPress = false;
    }


    void MainRadialMenuButton(int n)
    {
        switch (n)
        {
            case 2://hightlight

                if (m_Page == null)
                    m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());

                m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();
                //  m_PDFHighlight.CheckToCancelHighlight();

                m_EventDataWhenInvoked.pressPosition = m_PositionWhenLongPress;
                bool k = m_PDFViewerHelper.m_PDFHighlight.DoTheHightlight(m_EventDataWhenInvoked, m_Page.PageIndex);

                if (k == true)
                {

                    if (PDFViewerHelper.listOfHightlights.Count > 0)
                    {
                    Vector2 pos = m_PositionWhenLongPress;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, pos, GetComponent<Camera>(), out pos);
                    RectTransform rt = transform as RectTransform;
                    pos += rt.sizeDelta.x * 0.5f * Vector2.right;
                    pos += rt.sizeDelta.y * 0.5f * Vector2.up;
                    pos = pos.x * (rt.sizeDelta.y / rt.sizeDelta.x) * Vector2.right + pos.y * Vector2.up;
                    //Vector2 pagePoint = m_Page.DeviceToPage(0, 0, (int)rt.sizeDelta.y, (int)rt.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos.x, (int)pos.y);

                        Vector2 pageSize = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

                        if (m_PDFViewerHelper.m_PDFHighlight.GetHighlightedTextToEdit() < PDFViewerHelper.listOfHightlights.Count)
                        {
                            PDFViewerHelper.RectHightlights item = PDFViewerHelper.listOfHightlights[m_PDFViewerHelper.m_PDFHighlight.GetHighlightedTextToEdit()];
                            m_PDFViewerHelper.m_PDFHighlight.PutMarkers(m_Page.PageIndex, m_PDFViewerHelper.m_PDFHighlight.GetHighlightedTextToEdit(), pageSize, item.rect.xMin, item.rect.xMax, item.rect.yMin, item.rect.yMin - item.rect.height);

                            m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();


                            m_PDFViewerHelper.stateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.APPEARING;
                            PDFViewerHelper.m_PageWhereSideBarWasInvoked = transform.GetSiblingIndex();
                            m_PDFViewerHelper.m_TypeOfOnScreenMenu = 2;//2=highlight, 0=postit
                            PDFViewerHelper.m_MenuOnScreenActive = true;

                        }
                    }

                }

                break;

            case 0://postit

                if (m_Page == null)
                    m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());

                m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();

                m_EventDataWhenInvoked.pressPosition = m_PositionWhenLongPress;

                Vector2 pos2 = m_PositionWhenLongPress;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, pos2, GetComponent<Camera>(), out pos2);
                RectTransform rt2 = transform as RectTransform;
                pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
                pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
                pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;

                Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
                Vector2 sizePage = m_Page.GetPageSize();

                PDFViewerHelper.RectPostIt newRectPostIt = new PDFViewerHelper.RectPostIt();

                Vector2 sizePostIt = new Vector2((sizePage.x * 15) / 100, (sizePage.y * 10) / 100);
                newRectPostIt.pos = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));
                newRectPostIt.pos.x *= m_PDFViewer.ZoomFactor;
                newRectPostIt.pos.y *= m_PDFViewer.ZoomFactor;

                newRectPostIt.color = new Color((float)0xFF / (float)0xFF, (float)0xE6 / (float)0xFF, (float)0x6E / (float)0xFF, 1.0f);
                newRectPostIt.size = new Vector2(220, 150) * m_PDFViewer.ZoomFactor;
                newRectPostIt.text = "";
                newRectPostIt.page = m_Page.PageIndex;
                newRectPostIt.collapsed = false;

                m_PDFViewerHelper.m_PDFPostit.m_PDFViewerHelper = m_PDFViewerHelper;

                m_PDFViewerHelper.CurrentPostIt = m_PDFViewerHelper.m_PDFPostit.DoThePostit(this, GetType(), newRectPostIt, true);

                m_PDFViewerHelper.CurrentPostIt.MinWidth = sizePostIt.x;
                m_PDFViewerHelper.CurrentPostIt.MinHeight = sizePostIt.y;

                m_PDFViewerHelper.CurrentPostIt.setEdit(true);

                m_PDFViewerHelper.stateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.APPEARING;
                PDFViewerHelper.m_PageWhereSideBarWasInvoked = transform.GetSiblingIndex();

                m_PDFViewerHelper.m_TypeOfOnScreenMenu = 0;//2=highlight, 0=postit
                PDFViewerHelper.m_MenuOnScreenActive = true;


                break;

            case 1://free hand

                int sib = transform.GetSiblingIndex();

                if (m_Page == null)
                    m_Page = m_PDFViewer.Document.GetPage(sib);

                m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();

                PDFViewerPageHelper[] instances = m_PDFViewer.m_Internal.m_PageContainer.GetComponentsInChildren<PDFViewerPageHelper>();

                for (int x = sib - 1; x <= sib + 1; x++)
                {
                    if ((x >= 0) && (x < instances.Length))
                    {


                        RawImage[] pagesToDraw = instances[x].GetComponentsInChildren<RawImage>();

                        if (pagesToDraw[pagesToDraw.Length - 1].name.Contains("FreeHand") == false)//create a new one
                        {

                            Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(x) as Transform;

                            GameObject newPageToPaint = (GameObject)Instantiate(m_PageToDraw) as GameObject;

                            newPageToPaint.transform.SetParent(currentPage.transform);

                            newPageToPaint.transform.localPosition = new Vector2(0, 0);
                            newPageToPaint.transform.localScale = new Vector3(1, 1, 1);

                            RawImage img = newPageToPaint.GetComponent<RawImage>();


                            img.color = new Color(1, 1, 1, 1);

                            Vector2 pageSize = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

                            if ((m_PDFViewerHelper == null) && (m_PDFViewer != null))
                                m_PDFViewerHelper = m_PDFViewer.GetComponent<PDFViewerHelper>();


                            Texture2D t2d = new Texture2D((int)((float)m_PDFViewer.Document.GetPageWidth(0) * m_PDFViewerHelper.initialZoom), (int)((float)m_PDFViewer.Document.GetPageHeight(0) * m_PDFViewerHelper.initialZoom), TextureFormat.RGBA32, false);
                            img.texture = t2d;
                            newPageToPaint.GetComponent<RectTransform>().sizeDelta = pageSize;// new Vector2(W, H);




                            freeHandControl fhc = newPageToPaint.GetComponent<freeHandControl>();

                            fhc.theTexture = t2d;

                            fhc.init(currentPage);

                            fhc.setHelper(m_PDFViewerHelper);

                            pagesToDraw = currentPage.GetComponentsInChildren<RawImage>();
                        }


                        drawingPage = pagesToDraw[pagesToDraw.Length - 1].gameObject;


                        if (x == sib-1)
                        {
                            m_PDFViewerHelper.m_freeHandControlPrev = drawingPage.GetComponent<freeHandControl>();
                        }
                        if (x == sib + 1)
                        {
                            m_PDFViewerHelper.m_freeHandControlNext = drawingPage.GetComponent<freeHandControl>();
                        }
                        if (x == sib)
                        {
                            m_PDFViewerHelper.m_freeHandControl = drawingPage.GetComponent<freeHandControl>();
                            m_PDFViewerHelper.m_freeHandControl.hasMadeChanges = false;
                        }

                        if (m_Page == null)
                            m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());

                        if (x == sib)
                        {
                            m_PDFViewerHelper.m_freeHandControl.setEditing(true);
                            m_PDFViewerHelper.m_freeHandControl.setMode(freeHandControl.modes.PAINTING);
                        }

                        pagesToDraw[pagesToDraw.Length - 1].raycastTarget = true;
                    }

                }

                m_PDFViewerHelper.stateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.APPEARING;
                PDFViewerHelper.m_PageWhereSideBarWasInvoked = transform.GetSiblingIndex();

                m_PDFViewerHelper.m_TypeOfOnScreenMenu = 1;
                PDFViewerHelper.m_MenuOnScreenActive = true;

                m_PDFViewer.m_AllowedToPinchZoom = false;

                break;

            case 3:// cancel
                m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();

                break;
        }
    }




    public bool EditHighlight(PDFPage m_Page, Vector2 pos)
    {
        using (PDFTextPage textPage = m_Page.GetTextPage())
        {
            //Vector2 pos = eventData.pressPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, pos, GetComponent<Camera>(), out pos);
            RectTransform rt = transform as RectTransform;
            pos += rt.sizeDelta.x * 0.5f * Vector2.right;
            pos += rt.sizeDelta.y * 0.5f * Vector2.up;
            pos = pos.x * (rt.sizeDelta.y / rt.sizeDelta.x) * Vector2.right + pos.y * Vector2.up;

            Vector2 pagePoint = m_Page.DeviceToPage(0, 0, (int)rt.sizeDelta.y, (int)rt.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos.x, (int)pos.y);
            Vector2 pageSize = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

            bool found = false;

            for (int i = 0; i < PDFViewerHelper.listOfHightlights.Count; i++)
            {
                PDFViewerHelper.RectHightlights item = PDFViewerHelper.listOfHightlights[i];
                if (m_Page.PageIndex == item.page)
                {
                    for (int j = 0; j < item.innerRects.Count; j++)
                    {
                        if ((pagePoint.x > item.innerRects[j].xMin) && (pagePoint.x < item.innerRects[j].xMax) && (pagePoint.y < item.innerRects[j].yMin) && (pagePoint.y > (item.innerRects[j].yMin - item.innerRects[j].height)))
                        {
                            m_PDFViewerHelper.m_PDFHighlight.DoTheTable(textPage, m_Page.PageIndex);

                            //m_PDFHighlight.highlightedTextToEdit = i;
                            m_PDFViewerHelper.m_PDFHighlight.SetHighlightedTextToEdit(i);
                            m_PDFViewerHelper.m_PDFHighlight.PutMarkers(m_Page.PageIndex, i, pageSize, item.rect.xMin, item.rect.xMax, item.rect.yMin, item.rect.yMin - item.rect.height);
                            found = true;


                            m_PDFViewerHelper.stateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.APPEARING;
                            PDFViewerHelper.m_PageWhereSideBarWasInvoked = transform.GetSiblingIndex();
                            m_PDFViewerHelper.m_TypeOfOnScreenMenu = 2;
                            PDFViewerHelper.m_MenuOnScreenActive = true;


                            m_PDFViewerHelper.m_PDFHighlight.UpdateMarkers(true);

                            break;
                        }
                    }
                }
                if (found == true)
                    break;
            }

            return found;

        }

    }


    private void OnLongPress()
    {
        


        if ((PDFRadialInterface.IsActive == false) && (PDFViewerHelper.m_MenuOnScreenActive == false))
        {
            if (m_PDFViewer == null)
                m_PDFViewer = GetComponentInParent<PDFViewer>();
            if (m_Page == null)
                m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());

            bool k = EditHighlight(m_Page, m_EventDataWhenInvoked.pressPosition);

            if (k == false)
            {

                Texture2D[] textures = new Texture2D[4];
                textures[0] = m_PDFViewerHelper.m_PDFRadialInterface.ImagePostIt;
                textures[1] = m_PDFViewerHelper.m_PDFRadialInterface.ImageFreeHand;
                textures[2] = m_PDFViewerHelper.m_PDFRadialInterface.ImageHightlight;
                textures[3] = m_PDFViewerHelper.m_PDFRadialInterface.ImageCancel;
                m_PDFViewerHelper.m_PDFRadialInterface.CreateRadialMenu(this, GetType(), "MainRadialMenuButton", m_EventDataWhenInvoked.pressPosition, PDFRadialInterface.RadialMenuTypes.THREE, m_PDFViewer.ScaleFactorMenu, true, 0.5f, textures, false);


                m_PositionWhenLongPress = m_EventDataWhenInvoked.pressPosition;
            }

            m_ComingFromLongPress = true;
        }
        


    }

/*    
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
        int scrollBarWidth=0;

        if (m_PDFViewer == null)
            m_PDFViewer = GetComponentInParent<PDFViewer>();

        if (rtVerticalScrollBar == null)
        {
            rtVerticalScrollBar = m_PDFViewer.m_Internal.m_VerticalScrollBar.GetComponent<RectTransform>();
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

        if (styleSideBar==null)
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

                        if (m_XOnScreenSubMenu <= m_XOnScreenMenu - (wMenu * nOfSubMenus) )//(Screen.width - (wMenu * (nOfSubMenus + 1)) - (int)(rtVerticalScrollBar.sizeDelta.x / 2)) )
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
                alpha /= (m_XOnScreenMenu - (m_XOnScreenMenu - (wMenu * nOfSubMenus) ));
                alpha = 1.0f - alpha;
                GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, alpha);

                if ((GUI.Button(new Rect(m_XOnScreenMenu - (wMenu * nOfSubMenus) + (j* wMenu), m_YOnScreenSubMenu + offset, wMenu - (offset * 2), hIMenu - (offset * 2)), "", styleSideBar)) &&
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
                        if (m_freeHandControl.getMode()==0)//paint
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

                                    if (m_freeHandControlPrev!=null)
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

                                    m_PDFViewer.m_AllowedToPinchZoom = true;

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
*/

    void returnFunctionFromPostItDismiss(int id)
    {
        m_PDFViewerHelper.stateOnScreenSubMenu = PDFViewerHelper.OnScreenSubMenuAnimationState.DISSAPEARING;
        m_PDFViewerHelper.nextStateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.DISSAPEARING;
    }

    void returnFunctionFromPostIt(int id)
    {
        //int id = go[0];
        if (
            (PDFRadialInterface.IsActive == false) &&
            ((PDFViewerHelper.m_MenuOnScreenActive == false) || ((PDFViewerHelper.m_MenuOnScreenActive == true) && (m_PDFViewerHelper.m_TypeOfOnScreenMenu == 0)))
            )
        {
            if (m_PDFViewer == null)
                m_PDFViewer = GetComponentInParent<PDFViewer>();

            //if ((CurrentPostIt != null) && (CurrentPostIt.getEdit() == true))
            //{
            //    CurrentPostIt.setEdit(false);
            //}

            int maxSiblingIndex = -1;
            PDFPostItAdjust cItem = null;
            for (int i = 0; i < m_PDFViewerHelper.listOfPostIts.Count; i++)
            {

                PDFViewerHelper.RectPostIt item = m_PDFViewerHelper.listOfPostIts[i];

                int s = item.instanceOfPostIt.transform.GetSiblingIndex();
                if (s > maxSiblingIndex)
                {
                    maxSiblingIndex = s;
                    cItem = item.instanceOfPostIt;
                }

                if (id == item.id)
                {
                    m_PDFViewerHelper.CurrentPostIt = item.instanceOfPostIt;
                    m_PDFViewerHelper.CurrentPostIt.setEdit(true);
                    m_PDFViewerHelper.CurrentPostIt.ResizePostIt(0, 0);

                    //CurrentPostIt.transform.GetSiblingIndex
                    //break;
                }
                else
                    item.instanceOfPostIt.setEdit(false);
            }
            int c = m_PDFViewerHelper.CurrentPostIt.transform.GetSiblingIndex();
            m_PDFViewerHelper.CurrentPostIt.transform.SetSiblingIndex(maxSiblingIndex);
            if (cItem != null)
            {
                cItem.transform.SetSiblingIndex(c);
            }

            //if ((cItem.collapsed == false) && (stateOnScreenMenu == OnScreenMenuAnimationState.HIDDEN))
            if (m_PDFViewerHelper.stateOnScreenMenu == PDFViewerHelper.OnScreenMenuAnimationState.HIDDEN)
            {
                m_PDFViewerHelper.stateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.APPEARING;

                PDFViewerHelper.m_PageWhereSideBarWasInvoked = m_PDFViewer.CurrentPageIndex;

                m_PDFViewerHelper.m_TypeOfOnScreenMenu = 0;
                PDFViewerHelper.m_MenuOnScreenActive = true;
            }
        }
    }



    public void resetSideBar()
    {

        if ((m_PDFViewerHelper == null) && (m_PDFViewer != null))
            m_PDFViewerHelper = m_PDFViewer.GetComponent<PDFViewerHelper>();

        m_PDFViewerHelper.m_XOnScreenMenu = Screen.width;
        m_PDFViewerHelper.stateOnScreenMenu = PDFViewerHelper.OnScreenMenuAnimationState.HIDDEN;
        PDFViewerHelper.m_MenuOnScreenActive = false;

        if (m_PDFViewerHelper.m_PDFRadialInterface != null)
            m_PDFViewerHelper.m_PDFRadialInterface.DestroyRadialMenu();
        
    }

}
