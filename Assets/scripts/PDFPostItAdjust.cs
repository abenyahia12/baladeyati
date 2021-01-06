using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Paroxe.PdfRenderer;
using System.Collections.Generic;
using Paroxe.PdfRenderer.Internal.Viewer;
using System.Collections;

public class PDFPostItAdjust : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDropHandler, IDragHandler, IPointerClickHandler, IEndDragHandler
{
    public static Vector2 oldDocumentSize;
    bool hasChanged, isOkToCallBackToSelect;
    public int m_MaxCharacters = 50;
    public int m_MaxLengthWord = 10;
    float m_Threshold, zoomWhenCollapsed, timeWhenStoppedTheScroll;
    int timeClicked;
    bool toBeEdited, hasStoppedTheScroll;
    bool m_BoolFirstTime;
    public Image background,shadow, fake;
    public InputField inputfield;
    public Image ImageExpand;
    public PDFRadialInterface m_PDFRadialInterface;
    public Text textComponent, textComponentReal;
    public Vector2 pos1, pos2, originalSizeBackground, originalSizeText;
    public PDFViewer m_PDFViewer;
    public bool collapsed;
    public float MinWidth, MinHeight;
    public int SizeCorner, SizeCollapsedPostIt, SizeConfirmButton;
    public int page;
    GUIStyle myStyle;
    public string previousString, currentString;
    public Texture2D[] postitcollapsed;
    bool movingFullSize;
    bool moving;
    public RectTransform[] rt;
    //public Texture2D[] PostItsBackgrounds;
    public PDFHightlight m_PDFHighlight;
    public PDFViewerPage m_PDFViewerPage;
    public PDFViewerHelper m_PDFViewerHelper;
    public enum ActionOnPostIt { RESIZING, MOVING};
    ActionOnPostIt MyAction;
    wordPart[][] wordsInPostIt;
    enum DraggingMode { IN, OUT };
    DraggingMode MyDraggingMode;

    public int id;
    public int indexColor;
    PDFPage m_Page;
    //Vector2 initialLocation, endLocation;
    public object callerObject;
    public System.Reflection.MethodInfo info;//, infoDismiss;
    public GameObject PrefabEditablePostIt;
    //public float itemOriginalPosY;
    bool IsBeingEdited;
    public Vector2 wholeDocumentSize;
    Vector2 newWholeDocumentSize;
    public RectTransform rtMain;
    //public int m_offset_x, m_offset_y;
    public Sprite[] backgroundSprites;
    public Outline m_Outline;


    public Color[] m_HighlightcolorsP2 =
    {
        new Color((float)0xFF/(float)0xFF, (float)0xE6/(float)0xFF, (float)0x6E/(float)0xFF, 1.0f),//yellow
        new Color((float)0x9E/(float)0xFF, (float)0xDF/(float)0xFF, (float)0xFF/(float)0xFF, 1.0f),//blue
        new Color((float)0xA1/(float)0xFF, (float)0xEF/(float)0xFF, (float)0x9B/(float)0xFF, 1.0f),//green
        new Color((float)0xFF/(float)0xFF, (float)0x24/(float)0xFF, (float)0x19/(float)0xFF, 1.0f),//red
    };

    public bool getEdit()
    {
        return IsBeingEdited;
    }

    public struct wordPart
    {
        public wordPart(string _str, int _t)
        {
            str = _str;
            type = _t;
        }
        public string str;
        public int type;//0: whole word. 1: first part of a longer word. 2: middle part. 3: end part
    };

    public void setEdit(bool k)
    {
        
        IsBeingEdited = k;
        if (IsBeingEdited == true)
        {
            //if (collapsed == false)
            {
                //ImageConfirm.rectTransform.sizeDelta = new Vector2(SizeConfirmButton, SizeConfirmButton);
                m_Outline.effectColor = new Color(1.0f,0.27f,0.012f,1.0f);
            }
        }
        else
        {
            //ImageConfirm.rectTransform.sizeDelta = new Vector2(0, 0);
            m_Outline.effectColor = new Color(1.0f, 0.27f, 0.012f, 0.0f);
        }
        
    }

    public void ResizePostIt(float ix, float iy)
    {
        rtMain.sizeDelta = new Vector2(rtMain.sizeDelta.x + ix, rtMain.sizeDelta.y + iy);
        background.rectTransform.sizeDelta = new Vector2(background.rectTransform.sizeDelta.x + ix, background.rectTransform.sizeDelta.y + iy);

        if (collapsed == false)
            fake.rectTransform.sizeDelta = new Vector2(background.rectTransform.sizeDelta.x, background.rectTransform.sizeDelta.y);
        else
            fake.rectTransform.sizeDelta = Vector2.zero;

        if (collapsed == false)
        {
            shadow.rectTransform.sizeDelta = new Vector2(background.rectTransform.sizeDelta.x + 20, background.rectTransform.sizeDelta.y + 20);

            textComponent.rectTransform.sizeDelta = new Vector2(textComponent.rectTransform.sizeDelta.x + ix, textComponent.rectTransform.sizeDelta.y + iy);

            textComponentReal.rectTransform.sizeDelta = new Vector2(textComponentReal.rectTransform.sizeDelta.x + ix, textComponentReal.rectTransform.sizeDelta.y + iy);

            float offsetX = 4;
            float offsetY = 6;

            ImageExpand.transform.localPosition = new Vector2(rtMain.sizeDelta.x - ImageExpand.rectTransform.sizeDelta.x/2 - offsetX, -rtMain.sizeDelta.y + ImageExpand.rectTransform.sizeDelta.y/2 + offsetY);
            
            inputfield.transform.localPosition = new Vector2(0, 0);
            for (int i = 0; i < rt.Length; i++)
            {
                if (i == rt.Length - 1)
                {
                    rt[i].transform.localPosition = new Vector2(0, 0);//this is the Text element (has to be aligned to the top left)
                }
                else
                {
                    if (i == rt.Length - 2)
                        rt[i].sizeDelta = new Vector2(0,0);
                    else
                        rt[i].transform.localPosition = new Vector2((rtMain.sizeDelta.x * 48.0f) / 961.0f, -(rtMain.sizeDelta.y * 139.0f) / 674.0f);
                }

            }
            hasChanged = true;
        }
        
        AdjustScale();

    }
    
    public void OnDrag(PointerEventData eventData)
    {
        isOkToCallBackToSelect = false;

        if ((rtMain != null) && (moving == true))
        {
            Vector2 pos2 = eventData.position;
            float ix = (pos2.x - pos1.x);// * m_PDFViewer.ZoomFactor;
            float iy = ((pos2.y - pos1.y) * -1.0f);// * m_PDFViewer.ZoomFactor;

            if ((collapsed == false) && (movingFullSize == false))
            {

                MyAction = ActionOnPostIt.RESIZING;

                switch (MyDraggingMode)
                {
                    case DraggingMode.IN:
                        if (rtMain.sizeDelta.x + ix < (MinWidth * m_PDFViewer.ZoomFactor))
                        {
                            ix = 0;
                            MyDraggingMode = DraggingMode.OUT;
                        }
                        if (rtMain.sizeDelta.y + iy < (MinHeight * m_PDFViewer.ZoomFactor))
                        {
                            iy = 0;
                            MyDraggingMode = DraggingMode.OUT;
                        }
                        ResizePostIt(ix, iy);
                        pos1 = pos2;
                    break;

                    case DraggingMode.OUT:

                        Vector2 localCursor = new Vector2();
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor);

                        if ((localCursor.x > (rtMain.sizeDelta.x - ImageExpand.rectTransform.sizeDelta.x/2)) && (localCursor.y < -rtMain.sizeDelta.y + (ImageExpand.rectTransform.sizeDelta.y / 2)))
                        {
                            ix = localCursor.x - (rtMain.sizeDelta.x - (ImageExpand.rectTransform.sizeDelta.x / 2)); 
                            iy = Mathf.Abs(localCursor.y - (-rtMain.sizeDelta.y + (ImageExpand.rectTransform.sizeDelta.y / 2)));

                            ResizePostIt(ix, iy);

                            MyDraggingMode = DraggingMode.IN;
                            pos1 = eventData.position;
                        }
                        else if ((localCursor.x > (MinWidth * m_PDFViewer.ZoomFactor)) && (localCursor.y < (-MinHeight * m_PDFViewer.ZoomFactor)))
                        {
                            ix = localCursor.x - (rtMain.sizeDelta.x - (ImageExpand.rectTransform.sizeDelta.x / 2));
                            iy = localCursor.y - (rtMain.sizeDelta.y + (ImageExpand.rectTransform.sizeDelta.y / 2));

                            if ((rtMain.sizeDelta.x + ix) < 0)
                            {
                                float bix = ((rtMain.sizeDelta.x + ix) * -1.0f) - rtMain.sizeDelta.x;
                                ix = bix;
                            }
                            if ((rtMain.sizeDelta.y + iy) < 0)
                            {
                                float biy = ((rtMain.sizeDelta.y + iy) * -1.0f) - rtMain.sizeDelta.y;
                                iy = biy;
                            }

                            ResizePostIt(ix, iy);

                            MyDraggingMode = DraggingMode.IN;
                            pos1 = eventData.position;
                        }
                        
                        pos1 = pos2;// eventData.position;
                    break;
                }



            }
            //CancelInvoke("OnLongPress");

        }
        if ((collapsed==true) || ((collapsed == false) && (movingFullSize == true)))
        {
            Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(page) as Transform;
            Vector2 pos2 = eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos2, GetComponent<Camera>(), out pos2);
            RectTransform rt2 = currentPage as RectTransform;
            pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
            pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
            pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;
            Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
            Vector2 sizePage = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);
            pos2 = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));

            float ix = (pos2.x - pos1.x) * m_PDFViewer.ZoomFactor;
            float iy = ((pos2.y - pos1.y) * -1.0f) * m_PDFViewer.ZoomFactor;

            MyAction = ActionOnPostIt.MOVING;
            rtMain.localPosition = new Vector2(rtMain.localPosition.x + ix, rtMain.localPosition.y - iy);

            pos1 = pos2;
        }

    }

    public void SetFontSize(Vector2 sizeBackground)
    {

        //In order to calculate the relation between the area of a rectangle (post-it) and its font size, 3 postit sizes has been choosen arbitrarily, and for each one of these a font size has been choosen
        //Given this, I have create a system of 3 linear equations of the form Y = A*X^2 + B*X + C
        //X will be the area of any given postit, and Y its relative font size
        //The following values are calculated using Cramer's rule, and therefore, I can calculate the font size for any given area with the formula fontsize = (A * (area * area)) + (B * area) + C;
        float A, B, C;
        A = 443480.0f / -272484354555000.0f;
        B = -191280419400.0f / -272484354555000.0f;
        C = -4456541049980000.0f / -272484354555000.0f;

        float area = sizeBackground.x * sizeBackground.y;
        float fontSize = (A * (area * area)) + (B * area) + C;
        textComponent.fontSize = (int)fontSize * 2;
        textComponentReal.fontSize = (int)fontSize * 2;

        myStyle.fontSize = textComponent.fontSize;

        AdjustScale();
        
    }

    public void AdjustScale()
    {
        if ((rtMain != null) && (collapsed == false))
        {
            Vector2 input = rtMain.sizeDelta;// inputfield.GetComponent<RectTransform>().sizeDelta;

            rt[rt.Length - 1].localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //rt[rt.Length - 1].sizeDelta = new Vector2(((input.x * 755.0f) / 961.0f) * 2.0f, ((input.y * 380.0f) / 674.0f)*2.0f);
            rt[rt.Length - 1].sizeDelta = new Vector2(((input.x * 855.0f) / 961.0f) * 2.0f, ((input.y * 380.0f) / 674.0f) * 2.0f);
        }
        
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        isOkToCallBackToSelect = true;
        

        if ((rtMain != null) && ((moving == true) || (movingFullSize == true)))
        {
            //endLocation = transform.localPosition;

            List<PDFViewerHelper.RectPostIt> listOfPostIts = m_PDFViewerHelper.listOfPostIts;
            for (int i = 0; i < listOfPostIts.Count; i++)
            {
                PDFViewerHelper.RectPostIt p = listOfPostIts[i];
                if ((p.id == id) && (p.page == page))
                {
                    float w, h, x, y;
                    Vector2 newDocumentSize = m_PDFViewer.GetDocumentSize();

                    if (MyAction == ActionOnPostIt.RESIZING)
                    {
                        p.size = rtMain.sizeDelta;

                        w = (p.size.x / newDocumentSize.x) * oldDocumentSize.x;
                        h = (p.size.y / newDocumentSize.y) * oldDocumentSize.y;

                        p.size = new Vector2(w, h);
                    }
                    else
                    {
                        p.pos = rtMain.localPosition;

                        x = (p.pos.x / newDocumentSize.x) * oldDocumentSize.x;
                        y = (p.pos.y / newDocumentSize.y) * oldDocumentSize.y;

                        p.pos = new Vector2(x, y);
                    }


                    listOfPostIts[i] = p;
                    moving = false;
                    break;
                }
            }

            MyAction = ActionOnPostIt.RESIZING;

            save();

        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (isOkToCallBackToSelect == true)
        {
            object[] myObjects = new object[1];
            myObjects[0] = id;
            info.Invoke(callerObject, myObjects);
        }

        bool k = false;
        if (collapsed == true)
        {
            if (MyAction == ActionOnPostIt.RESIZING)
            {

                if (zoomWhenCollapsed != 0)
                {
                    rtMain.sizeDelta = originalSizeBackground / (zoomWhenCollapsed / m_PDFViewer.ZoomFactor);
                    textComponent.rectTransform.sizeDelta = originalSizeText / (zoomWhenCollapsed / m_PDFViewer.ZoomFactor);
                    textComponentReal.rectTransform.sizeDelta = originalSizeText / (zoomWhenCollapsed / m_PDFViewer.ZoomFactor);
                    background.rectTransform.sizeDelta = originalSizeBackground / (zoomWhenCollapsed / m_PDFViewer.ZoomFactor);
                }
                else
                {
                    rtMain.sizeDelta = originalSizeBackground;
                    textComponent.rectTransform.sizeDelta = originalSizeText;
                    textComponentReal.rectTransform.sizeDelta = originalSizeText;
                    background.rectTransform.sizeDelta = originalSizeBackground;
                }


                shadow.rectTransform.sizeDelta = new Vector2(background.rectTransform.sizeDelta.x + 20, background.rectTransform.sizeDelta.y + 20);
                fake.rectTransform.sizeDelta = new Vector2(background.rectTransform.sizeDelta.x - 2, background.rectTransform.sizeDelta.y - 2);

                ImageExpand.rectTransform.sizeDelta = new Vector2(SizeCorner, SizeCorner);

                inputfield.GetComponent<RectTransform>().sizeDelta = new Vector2(rtMain.sizeDelta.x - 20, rtMain.sizeDelta.y - (ImageExpand.rectTransform.sizeDelta.y * 2));
                background.sprite = backgroundSprites[indexColor];

                inputfield.interactable = true;
                collapsed = false;
                textComponent.transform.localScale = new Vector3(1, 1, 1);
                textComponentReal.transform.localScale = new Vector3(1, 1, 1);

                inputfield.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                for (int i = 0; i < rt.Length - 1; i++)
                {
                    rt[i].sizeDelta = new Vector2(0, 0);
                }

                k = false;
                AdjustScale();
                
            }
            else
            {
                k = true;
            }

            
            List<PDFViewerHelper.RectPostIt> listOfPostIts = m_PDFViewerHelper.listOfPostIts;
            for (int i = 0; i < listOfPostIts.Count; i++)
            {
                PDFViewerHelper.RectPostIt p = listOfPostIts[i];
                if ((p.id == id) && (p.page == page))
                {
                    p.collapsed = k;
                    
                    //p.size = originalSizeBackground;// background.rectTransform.sizeDelta;
                    
                    //if (MyAction == ActionOnPostIt.RESIZING)
                    //{
                    //    if (m_PDFViewer.ZoomFactor != p.zoomWhenCreated)
                    //    {
                    //        float fact = m_PDFViewer.ZoomFactor / p.backupZoomWhenCreated;
                    //        rtMain.sizeDelta = originalSizeBackground * fact;
                    //        textComponent.rectTransform.sizeDelta = originalSizeText;
                    //        textComponentReal.rectTransform.sizeDelta = originalSizeText;
                    //        background.rectTransform.sizeDelta = originalSizeBackground * fact;
                    //        shadow.rectTransform.sizeDelta = originalSizeBackground * fact;
                    //        fake.rectTransform.sizeDelta = originalSizeBackground * fact;
                    //        float offsetX = 4;
                    //        float offsetY = 6;
                    //        ImageExpand.transform.localPosition = new Vector2(rtMain.sizeDelta.x - ImageExpand.rectTransform.sizeDelta.x / 2 - offsetX, -rtMain.sizeDelta.y + ImageExpand.rectTransform.sizeDelta.y / 2 + offsetY);
                    //        p.backupZoomWhenCreated = m_PDFViewer.ZoomFactor;
                    //    }
                    //}
                    
                    listOfPostIts[i] = p;
                    break;
                }
            }
            SaveLoadPostIt.Save(listOfPostIts, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
            
            save();

            ResizePostIt(0, 0);
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_PDFViewer.m_AllowedToPinchZoom = false;

        toBeEdited = false;

        timeClicked = (System.DateTime.Now.Hour * 3600000) + (System.DateTime.Now.Minute * 60000) + (System.DateTime.Now.Second * 1000) + (System.DateTime.Now.Millisecond);

        inputfield.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

        Vector2 localCursor=new Vector2();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        if (collapsed == false)
        {
            if (rtMain != null)
            {
                if (
                    (localCursor.x < rtMain.sizeDelta.x) && (localCursor.x > (rtMain.sizeDelta.x - (ImageExpand.rectTransform.sizeDelta.x * 1.5f))) &&
                    (localCursor.y > -rtMain.sizeDelta.y) && (localCursor.y < (-rtMain.sizeDelta.y + (ImageExpand.rectTransform.sizeDelta.y * 1.5f)))
                    )
                {//right bottom corner
                    
                    Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(page) as Transform;
                    Vector2 pos2 = eventData.position;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos2, GetComponent<Camera>(), out pos2);
                    RectTransform rt2 = currentPage as RectTransform;
                    pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
                    pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
                    pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;

                    //Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
                    //Vector2 sizePage = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

                    //pos1 = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));

                    pos1 = eventData.position;
                    MyDraggingMode = DraggingMode.IN;
                    moving = true;
                    movingFullSize = false;

                }
                else
                if (
                    (localCursor.x > 0) &&
                    (localCursor.x < rtMain.sizeDelta.x) &&
                    (localCursor.y < 0) &&
                    (localCursor.y > -rtMain.sizeDelta.y)//(-(rtMain.sizeDelta.y * 139.0f) / 674.0f))//this is the actual size (height) of the top border (considering scale...)
                    )
                {

                    Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(page) as Transform;
                    Vector2 pos2 = eventData.position;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos2, GetComponent<Camera>(), out pos2);
                    RectTransform rt2 = currentPage as RectTransform;
                    pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
                    pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
                    pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;

                    Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
                    Vector2 sizePage = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

                    pos1 = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));

                    movingFullSize = true;

                    if ((localCursor.y < -ImageExpand.rectTransform.sizeDelta.y) && (localCursor.y > -rtMain.sizeDelta.y + ImageExpand.rectTransform.sizeDelta.y))
                        toBeEdited = true;
                }
                else
                    movingFullSize = false;
                /*
                if (
                    (localCursor.x > rtMain.sizeDelta.x - (ImageConfirm.rectTransform.sizeDelta.x / 4)) &&
                    (localCursor.x < (rtMain.sizeDelta.x - (ImageConfirm.rectTransform.sizeDelta.x / 4) + ImageConfirm.rectTransform.sizeDelta.x)) &&
                    (localCursor.y > -ImageConfirm.rectTransform.sizeDelta.y / 4) &&
                    (localCursor.y < (-(ImageConfirm.rectTransform.sizeDelta.y / 4) + ImageConfirm.rectTransform.sizeDelta.y))
                    )
                    {
                        object[] myObjects = new object[1];
                        myObjects[0] = id;
                        infoDismiss.Invoke(callerObject, myObjects);
                    }
                    else
                        movingFullSize = false;

                initialLocation = transform.localPosition;
                */
            }
        }
        else
        {
            //pos1 = localCursor;// eventData.position;
            Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(page) as Transform;
            Vector2 pos2 = eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos2, GetComponent<Camera>(), out pos2);
            RectTransform rt2 = currentPage as RectTransform;
            pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
            pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
            pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;

            Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
            Vector2 sizePage = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

            pos1 = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));
            moving = true;

            //initialLocation = transform.localPosition;
        }

    }

    public void RealCloseFunction(int n)
    {
        m_PDFRadialInterface.DestroyRadialMenu();

        if (n==0)//save
        {
            PDFViewerHelper.RectPostIt p = new PDFViewerHelper.RectPostIt();
            List<PDFViewerHelper.RectPostIt> listOfPostIts = m_PDFViewerHelper.listOfPostIts;
            int index = -1;
            for (int i = 0; i < listOfPostIts.Count; i++)
            {
                p = listOfPostIts[i];
                if ((p.id == id) && (p.page == page))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                if (p.instanceOfPostIt.gameObject != null)
                    GameObject.Destroy(p.instanceOfPostIt.gameObject);
                listOfPostIts.RemoveAt(index);
                
            }
            save();

            //SaveLoadPostIt.Save(listOfPostIts, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
        }

    }

    public void ColorRadialMenuButton(int n)
    {
        switch (n)
        {
            case 0:
            case 1:
            case 2:
            case 3:

              //  inputfield.image.color = new Color(m_HighlightcolorsP2[n].r, m_HighlightcolorsP2[n].g, m_HighlightcolorsP2[n].b, 0);

                //Sprite sprb = Sprite.Create(PostItsBackgrounds[n], new Rect(0, 0, PostItsBackgrounds[n].width, PostItsBackgrounds[n].height), new Vector2(0.5f, 0.5f));
                //background.sprite = sprb;
                background.sprite = backgroundSprites[n];

                indexColor = n;

                List<PDFViewerHelper.RectPostIt> listOfPostIts = m_PDFViewerHelper.listOfPostIts;
                for (int i = 0; i < listOfPostIts.Count; i++)
                {
                    PDFViewerHelper.RectPostIt p = listOfPostIts[i];
                    if ((p.id == id) && (p.page == page))
                    {
                        p.color = background.color;
                        p.indexColor = n;
                        listOfPostIts[i] = p;
                        break;
                    }
                }
                //SaveLoadPostIt.Save(listOfPostIts, m_PDFViewer.FileName);
                //SaveLoadPostIt.Save(listOfPostIts, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
                save();

            break;

            case 4:
            //    m_PDFRadialInterface.DestroyRadialMenu();
            break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_PDFViewer.m_AllowedToPinchZoom = true;

        int timeNow = (System.DateTime.Now.Hour * 3600000) + (System.DateTime.Now.Minute * 60000) + (System.DateTime.Now.Second * 1000) + (System.DateTime.Now.Millisecond);

        int lapseTime = timeNow - timeClicked;

        PDFViewerPageHelper pvh = m_PDFViewerPage.GetComponent<PDFViewerPageHelper>();

        if ((lapseTime < m_Threshold) && (toBeEdited == true) && (collapsed == false) && (m_PDFViewerHelper.stateOnScreenMenu == PDFViewerHelper.OnScreenMenuAnimationState.SHOWN) && (m_PDFViewerHelper.m_TypeOfOnScreenMenu != 2) && (m_PDFViewerHelper.m_TypeOfOnScreenMenu != 1))//m_TypeOfOnScreenMenu is the freehand tool. Don't allow editing text in post its if using freehand tool
        {
            EventSystem.current.SetSelectedGameObject(inputfield.gameObject, null);
            inputfield.OnPointerClick(eventData);
        }

    }


    void Start()
    {

        m_Threshold = 150;

        m_BoolFirstTime = true;

        hasStoppedTheScroll = false;

        isOkToCallBackToSelect = true;

        hasChanged = true;

        if (inputfield != null)
        {
            inputfield.onValueChanged.AddListener(delegate { ValueChange(); });
            inputfield.characterLimit = m_MaxCharacters;
        }

        //if (PrefabEditablePostIt != null)
        //    rtMain = PrefabEditablePostIt.GetComponent<RectTransform>();

        textComponent.raycastTarget = false;
        textComponentReal.raycastTarget = false;

        if ((collapsed == true) && (inputfield != null) && (rt != null) && (rtMain != null) && (ImageExpand != null))
        {
            inputfield.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            for (int i = 0; i < rt.Length; i++)
            {
                rt[i].sizeDelta = new Vector2(0, 0);
            }
        }
        if ((collapsed == false) && (inputfield != null) && (rt != null) && (rtMain != null) && (ImageExpand != null))
        {
            inputfield.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            for (int i = 0; i < rt.Length - 1; i++)
            {
                rt[i].sizeDelta = new Vector2(0, 0);
            }
        }

        moving = false;
        movingFullSize = false;

        ResizePostIt(0,0);
    }

    public void ValueChange()
    {

        List<PDFViewerHelper.RectPostIt> listOfPostIts = m_PDFViewerHelper.listOfPostIts;
        for (int i = 0; i < listOfPostIts.Count; i++)
        {
            PDFViewerHelper.RectPostIt p = listOfPostIts[i];
            if ((p.id == id) && (p.page == page))
            {
                p.text = inputfield.text;
                listOfPostIts[i] = p;
                break;
            }
        }

        hasChanged = true;

        //SaveLoadPostIt.Save(listOfPostIts, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
        save();
    }


    IEnumerator DelayedUnlockScrollRect()
    {
        while (Input.touchCount != 0)
            yield return null;

        ScrollRect scrollRect = m_PDFViewer.m_Internal.m_Viewport.GetComponent<ScrollRect>();

        scrollRect.inertia = true;
        scrollRect.horizontal = true;
        scrollRect.vertical = true;
    }

    public void stopScroll()
    {
        ScrollRect scrollRect = m_PDFViewer.m_Internal.m_Viewport.GetComponent<ScrollRect>();
        scrollRect.inertia = false;
        scrollRect.horizontal = false;
        scrollRect.vertical = false;
        StopCoroutine(DelayedUnlockScrollRect());
    }

    public void resumeScroll()
    {
        ScrollRect scrollRect = m_PDFViewer.m_Internal.m_Viewport.GetComponent<ScrollRect>();
        scrollRect.inertia = true;
        scrollRect.horizontal = true;
        scrollRect.vertical = true;
        StartCoroutine(DelayedUnlockScrollRect());
    }

    void Update()
    {


        if (m_Page == null)
            m_Page = m_PDFViewer.Document.GetPage(page);

        if ((inputfield.isFocused) && (m_BoolFirstTime == true))
        {
            timeWhenStoppedTheScroll = (System.DateTime.Now.Hour * 3600000) + (System.DateTime.Now.Minute * 60000) + (System.DateTime.Now.Second * 1000) + (System.DateTime.Now.Millisecond);
            hasStoppedTheScroll = true;
            //m_PDFViewer.stopScroll();
            stopScroll();


            m_BoolFirstTime = false;
            m_PDFViewer.m_Internal.m_PageContainer.anchoredPosition = new Vector2(m_PDFViewer.m_Internal.m_PageContainer.anchoredPosition.x, Mathf.Abs(transform.localPosition.y));
        }
        if (!inputfield.isFocused)
            m_BoolFirstTime = true;

        if (hasStoppedTheScroll == true)
        {
            float timeNow = (System.DateTime.Now.Hour * 3600000) + (System.DateTime.Now.Minute * 60000) + (System.DateTime.Now.Second * 1000) + (System.DateTime.Now.Millisecond);
            if (timeNow > timeWhenStoppedTheScroll + 1000)
            {
                hasStoppedTheScroll = false;
                //m_PDFViewer.resumeScroll();
                resumeScroll();
            }
        }
        
        if (hasChanged == true)
        {

            string originaltext = inputfield.text;
            string[] words = originaltext.Split(new[] {" " }, System.StringSplitOptions.RemoveEmptyEntries);
            string newString = "";
            string newStringWithCutWords = "";

            List<wordPart> newWords = new List<wordPart>();
            newWords.Clear();

            for (int i = 0; i < words.Length; i++)
            {

                if (words[i].Length > m_MaxLengthWord)
                {

                    int n = words[i].Length / m_MaxLengthWord;
                    for (int j = 0; j < n; j++)
                    {
                        string part = words[i].Substring(j * m_MaxLengthWord, m_MaxLengthWord);

                        wordPart newPart = new wordPart();
                        int type = 2;
                        if (j == 0)
                            type = 1;
                        else if ((j == n - 1) && (words[i].Length <= (n * m_MaxLengthWord)))
                            type = 3;

                        newPart.str = part;
                        newPart.type = type;

                        newWords.Add(newPart);
                    }
                    if (words[i].Length > (n * m_MaxLengthWord))//remaining text
                    {
                        string remaining = words[i].Substring(n * m_MaxLengthWord, words[i].Length - (n * m_MaxLengthWord));
                        wordPart newPart = new wordPart();
                        newPart.str = remaining;
                        newPart.type = 3;
                        newWords.Add(newPart);
                    }

                }
                else
                {
                    wordPart newPart = new wordPart(words[i], 0);
                    newWords.Add(newPart);
                }
                
            }

            if (newWords.Count > 0)
            {

                newStringWithCutWords = "";

                for (int i = 0; i < newWords.Count; i++)
                    newStringWithCutWords += newWords[i].str + " ";

                textComponentReal.text = newStringWithCutWords;
                inputfield.caretPosition = inputfield.text.Length;
                Canvas.ForceUpdateCanvases();
                int numberOfLines = textComponentReal.cachedTextGenerator.lines.Count;

                wordsInPostIt = new wordPart[numberOfLines][];
                int indexOfWords = 0;
                for (int i = 0; i < numberOfLines; i++)
                {
                    int index = textComponentReal.cachedTextGenerator.lines[i].startCharIdx;
                    int length;
                    if (i < numberOfLines - 1)
                        length = textComponentReal.cachedTextGenerator.lines[i + 1].startCharIdx - index;
                    else
                        length = newStringWithCutWords.Length - index - 1;

                    string newLine = newStringWithCutWords.Substring(index, length);
                    string[] wordsInNewLine = newLine.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);

                    wordsInPostIt[i] = new wordPart[wordsInNewLine.Length];
                    for (int j = 0; j < wordsInNewLine.Length; j++)
                    {
                        wordsInPostIt[i][j] = new wordPart(newWords[indexOfWords].str, newWords[indexOfWords].type);
                        indexOfWords++;
                    }
                }

                newString = "";

                for (int j = 0; j < numberOfLines; j++)
                {
                    for (int i = 0; i < wordsInPostIt[j].Length; i++)
                    {

                        wordPart jjWordPart = wordsInPostIt[j][i];
                        if (jjWordPart.type==0)//normal words!!!
                        {
                            
                            if ((newString.Length > 0) && (newString.Substring(newString.Length - 1, 1).CompareTo(" ") != 0))//last letter is NOT a space
                            {
                                newString += " " + jjWordPart.str + " ";
                            }
                            else
                            {
                                newString += jjWordPart.str + " ";
                            }

                        }
                        else if (jjWordPart.type == 1)//beginning of a long word
                        {
                            newString += jjWordPart.str;
                            if ((i == wordsInPostIt[j].Length - 1) && (j < numberOfLines - 1))
                                newString += "- ";
                        }
                        else if (jjWordPart.type == 2)//middle of a long word
                        {
                            newString += jjWordPart.str;
                            if ((i == wordsInPostIt[j].Length - 1) && (j < numberOfLines - 1))
                                newString += "- ";
                        }
                        else if (jjWordPart.type == 3)//end of a long word
                        {
                            newString += jjWordPart.str + " ";
                        }
                    }
                }

                textComponentReal.text = newString;

                inputfield.caretPosition = inputfield.text.Length;

                hasChanged = false;
            }

        }

    }



    void Awake()
    {
        MinWidth = 150.0f;
        MinHeight = 150.0f;
        SizeCorner = 32;
        SizeCollapsedPostIt = 48;
        SizeConfirmButton = 56;

        myStyle = new GUIStyle(GUIStyle.none)
        {
            fontSize = 16
        };
        myStyle.normal.textColor = Color.black;
    }

    public void CollapseFunction()
    {
        m_PDFRadialInterface.DestroyRadialMenu();

        originalSizeBackground = rtMain.sizeDelta;

        zoomWhenCollapsed = m_PDFViewer.ZoomFactor;

        originalSizeText = rtMain.sizeDelta;

        rtMain.sizeDelta = new Vector2(SizeCollapsedPostIt * m_PDFViewer.ZoomFactor, SizeCollapsedPostIt * m_PDFViewer.ZoomFactor);
        shadow.rectTransform.sizeDelta = Vector2.zero;
        fake.rectTransform.sizeDelta = Vector2.zero;

        background.rectTransform.sizeDelta = new Vector2(SizeCollapsedPostIt * m_PDFViewer.ZoomFactor, SizeCollapsedPostIt * m_PDFViewer.ZoomFactor);

        textComponent.rectTransform.sizeDelta = new Vector2(0, 0);
        textComponent.transform.localScale = new Vector3(0,0,0);

        textComponentReal.rectTransform.sizeDelta = new Vector2(0, 0);
        textComponentReal.transform.localScale = new Vector3(0, 0, 0);


        inputfield.interactable = false;

        ImageExpand.rectTransform.sizeDelta = new Vector2(0, 0);

        inputfield.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
        for (int i=0;i<rt.Length;i++)
            rt[i].sizeDelta = new Vector2(0, 0);

        collapsed = true;

        List<PDFViewerHelper.RectPostIt> listOfPostIts = m_PDFViewerHelper.listOfPostIts;

        float w, h;
        Vector2 newDocumentSize = m_PDFViewer.GetDocumentSize();

        for (int i = 0; i < listOfPostIts.Count; i++)
        {
            PDFViewerHelper.RectPostIt p = listOfPostIts[i];
            if ((p.id == id) && (p.page == page))
            {
                p.collapsed = true;
                p.size = originalSizeBackground;

                w = (p.size.x / newDocumentSize.x) * oldDocumentSize.x;
                h = (p.size.y / newDocumentSize.y) * oldDocumentSize.y;

                p.size = new Vector2(w, h);

                //p.zoomWhenCreated = m_PDFViewer.ZoomFactor;
                //p.backupZoomWhenCreated = m_PDFViewer.ZoomFactor;

                listOfPostIts[i] = p;
                Sprite spr = Sprite.Create(postitcollapsed[p.indexColor], new Rect(0, 0, postitcollapsed[p.indexColor].width, postitcollapsed[p.indexColor].height), new Vector2(0.5f, 0.5f));
                background.sprite = spr;
                break;
            }
        }
        //SaveLoadPostIt.Save(listOfPostIts, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
        save();
    }

    public void save()
    {

        List<PDFViewerHelper.RectPostIt> listOfPostIts = m_PDFViewerHelper.listOfPostIts;

        SaveLoadPostIt.Save(listOfPostIts, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
    }

}
