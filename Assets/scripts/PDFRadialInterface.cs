using Paroxe.PdfRenderer;
using Paroxe.PdfRenderer.Internal.Viewer;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PDFRadialInterface : MonoBehaviour {


    public enum MenuAnimationStates { STOPPED, GROWING_CIRCLES, GROWING_PETALS, FINISH, STATIC };
    public enum RadialMenuTypes { TWO, THREE, FOUR};

    public struct StructForMenus
    {
        public Image ShadowImage;
        public Image backgroundImage, imageWhole, circleOutside, circleInside;
        public GameObject central, rtElements;
        public MenuAnimationStates myState;
    };
    
    public GameObject myMenu;
    StructForMenus radialMenu;
    GameObject choosenObject;
    float SPEED;
    public GameObject radialMenu2, radialMenu3, radialMenu4;
    public Texture2D ImageHightlight, ImageFreeHand, ImagePostIt, ImageCancel, ImageBookmark, ImageBack, ImageColor, ImageSave;
    public Texture2D ImageHightlightSelected, ImagePostItSelected;
    public Texture2D ImageColor1, ImageColor2, ImageColor3, ImageColor4;
    object callerObject;
    System.Reflection.MethodInfo info;
    public PDFViewer m_PDFViewer;
    public bool IsAnimated;
    public static bool IsActive;
    Vector2[] DynamicPositions;
    float WR, HR;
    public int pageIndexWhereRadialMenuHasStarted;
    float MAX_GROW_CIRCLE_INSIDE = 0.5f;
    float MAX_GROW_CIRCLE_OUTSIDE = 0.6f;
    //float MAX_GROW_RING = 0.55f;
    float MAX_GROW_CENTRAL = 0.65f;

    public Vector2 wholeDocumentSize;
    Vector2 newWholeDocumentSize;
    RectTransform rt;
    PDFViewerHelper instanceOfHelper;

    void Start ()
    {
        //W = Screen.width;
        //H = Screen.height;
        radialMenu.myState = MenuAnimationStates.STOPPED;
        IsActive = false;
        WR = 0;
        HR = 0;

        DynamicPositions =  new Vector2[2];

        instanceOfHelper = m_PDFViewer.GetComponent<PDFViewerHelper>();
    }

    public void DestroyRadialMenu()
    {
        if (myMenu != null)
        {
            if (m_PDFViewer == null)
            {
                m_PDFViewer = GetComponentInParent<PDFViewer>();
            }
            m_PDFViewer.m_AllowedToPinchZoom = true;

            radialMenu.myState = MenuAnimationStates.STOPPED;
            GameObject.Destroy(myMenu.gameObject);
            IsActive = false;
        }
    }

    public void CreateRadialMenu(object o, System.Type t, string function, Vector3 v, RadialMenuTypes type, float scale, bool animation, float speed, Texture2D [] textures, bool realPosition, Vector2 leftPos = new Vector2(), Vector2 rightPos = new Vector2())
    {
        PDFPage m_Page;

        SPEED = speed;
        callerObject = o;
        info = t.GetMethod(function, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        switch (type)
        {
            case RadialMenuTypes.TWO:
                choosenObject = radialMenu2;
                break;

            case RadialMenuTypes.THREE:
                choosenObject = radialMenu3;
                break;

            case RadialMenuTypes.FOUR:
                choosenObject = radialMenu4;
                break;
        }

        DestroyRadialMenu();
        myMenu = (GameObject)Instantiate(choosenObject) as GameObject;
        myMenu.transform.localScale = new Vector3(scale, scale, scale);

        if (myMenu != null)
        {
            if (m_PDFViewer == null)
            {
                m_PDFViewer = GetComponentInParent<PDFViewer>();
            }
            //m_PDFViewer.m_AllowedToPinchZoom = false;

            int SiblingIndex = transform.GetSiblingIndex();
            instanceOfHelper.m_PageWhereRadialMenuStarted = SiblingIndex;

            m_Page = m_PDFViewer.Document.GetPage(SiblingIndex);
            Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(SiblingIndex) as Transform;

            Transform prevPage;
            float offset = 0;
            if (SiblingIndex > 0)
            {
                prevPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(SiblingIndex - 1) as Transform;
                RectTransform rtx1, rtx2;
                rtx1 = currentPage.GetComponent<RectTransform>();
                rtx2 = prevPage.GetComponent<RectTransform>();

                offset = Mathf.Abs(rtx1.anchoredPosition.y) - (Mathf.Abs(rtx2.anchoredPosition.y) + rtx2.rect.height);
            }
            else
            {
                offset = 0;
            }

            offset /= m_PDFViewer.ZoomFactor;

            pageIndexWhereRadialMenuHasStarted = m_PDFViewer.CurrentPageIndex;

            Vector2 pos=new Vector2();
            if (realPosition == false)
            {
                Vector2 pos2 = v;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos2, GetComponent<Camera>(), out pos2);
                RectTransform rt2 = currentPage as RectTransform;
                pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
                pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
                pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;

                Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
                Vector2 sizePage = m_Page.GetPageSize();

                pos = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));
                pos.y = ((SiblingIndex * sizePage.y) + ((pos.y * -1.0f) + (sizePage.y / 2))) * -1.0f;

                pos.y -= (offset * SiblingIndex);

            }
            else
            {
                //Vector2 sizePage = m_Page.GetPageSize();
                pos = v;
                //pos.y = ((SiblingIndex * sizePage.y) + ((pos.y * -1.0f) + (sizePage.y / 2))) * -1.0f;
            }

            Vector2 sizeMenu = myMenu.GetComponent<RectTransform>().sizeDelta;

            //pos = new Vector2(pos.x, pos.y + ((sizeMenu.y/2) / m_PDFViewer.ZoomFactor));

            myMenu.transform.SetParent(currentPage.parent);


            if ((leftPos.Equals(Vector2.zero)==false) && (rightPos.Equals(Vector2.zero) == false))
            {
                SetAndChooseDynamicPositions(leftPos, rightPos);
            }
            else
            {
                if (realPosition == false)
                {
                    pos.x *= m_PDFViewer.ZoomFactor;
                    pos.y *= m_PDFViewer.ZoomFactor;
                }
                myMenu.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            }

            if (animation == true)
            {
                radialMenu.myState = MenuAnimationStates.GROWING_CIRCLES;
            }
            else
            {
                radialMenu.myState = MenuAnimationStates.STATIC;
            }
            IsAnimated = animation;

            PDFRadialMenu rm = myMenu.GetComponent<PDFRadialMenu>();
            rm.SetScale(scale);
            rm.m_PDFViewer = m_PDFViewer;
            radialMenu.ShadowImage = rm.ShadowImage;
            radialMenu.backgroundImage = rm.ImageBackground;
            radialMenu.imageWhole = rm.ImageWhole;
            radialMenu.circleOutside = rm.CircleOutside;
            radialMenu.circleInside = rm.CircleInside;
            radialMenu.central = rm.central;
            radialMenu.rtElements = rm.rtElements;

            rm.type = type;

            Button[] b = myMenu.GetComponentsInChildren<Button>();

            float fractionOfCircle = 360.0f / (float)(b.Length - 1);

            for (int j = 0; j < b.Length; j++)
            {
                Sprite newSprite = Sprite.Create(textures[j], new Rect(0, 0, textures[j].width, textures[j].height), new Vector2(0.5f, 0.5f));
                Image[] images = b[j].GetComponentsInChildren<Image>();
                images[images.Length - 1].sprite = newSprite;
                images[images.Length - 1].transform.Rotate(new Vector3(0,0,1), (float)j * fractionOfCircle);
                int jj = j;
                b[j].onClick.AddListener(() => ButtonClicked(jj));
            }

        }

        wholeDocumentSize = m_PDFViewer.m_Internal.m_PageContainer.sizeDelta;
        rt = myMenu.GetComponent<RectTransform>();
        IsActive = true;
    }

    public void updateRadialMenu()
    {
        //int SiblingIndex = transform.GetSiblingIndex();

        //PDFPage m_Page = m_PDFViewer.Document.GetPage(SiblingIndex);

        newWholeDocumentSize = m_PDFViewer.m_Internal.m_PageContainer.sizeDelta;

        rt.anchoredPosition = new Vector2((rt.anchoredPosition.x / wholeDocumentSize.x) * newWholeDocumentSize.x, (rt.anchoredPosition.y / wholeDocumentSize.y) * newWholeDocumentSize.y);

        wholeDocumentSize = newWholeDocumentSize;
    }

    public void SetAndChooseDynamicPositions(Vector2 leftPos, Vector2 rightPos)
    {
        int SiblingIndex = transform.GetSiblingIndex();
        //PDFPage m_Page = m_PDFViewer.Document.GetPage(SiblingIndex);
        Vector2 sizePage = m_PDFViewer.m_Internal.m_PageContainer.rect.size;// m_Page.GetPageSize(m_PDFViewer.ZoomFactor);

        if ((WR == 0) && (HR == 0))
        {
            WR = myMenu.GetComponent<RectTransform>().rect.width * myMenu.transform.localScale.x;
            HR = myMenu.GetComponent<RectTransform>().rect.height * myMenu.transform.localScale.y;
        }

        DynamicPositions[0] = new Vector2((rightPos.x - ((rightPos.x - leftPos.x) / 2)), leftPos.y + (HR / 2) + 64);
        DynamicPositions[1] = new Vector2((rightPos.x - ((rightPos.x - leftPos.x) / 2)), rightPos.y - (HR / 2) - 64);
        //DynamicPositions[2] = new Vector2(leftPos.x - (WR / 2) - 48, leftPos.y - ((leftPos.y - rightPos.y) / 2));
        //DynamicPositions[3] = new Vector2(rightPos.x + (WR / 2) + 48, leftPos.y - ((leftPos.y - rightPos.y) / 2));
        //DynamicPositions[4] = new Vector2((rightPos.x - ((rightPos.x - leftPos.x) / 2)), leftPos.y - ((leftPos.y - rightPos.y) / 2));

        /*
        int choosen = 4;
        for (int i = 0;i < 5;i++)
        {
            if ((DynamicPositions[i].x > ((-sizePage.x / 2))) && (DynamicPositions[i].x < ((sizePage.x / 2))) && 
                (DynamicPositions[i].y > ((-sizePage.y / 2))) && (DynamicPositions[i].y < ((sizePage.y / 2))))
            {
                choosen = i;
                break;
            }
        }
        */
        int choosen = 0;
        if (SiblingIndex == 0)
            choosen = 1;


        if (myMenu != null)
        {
            //DynamicPositions[choosen].y = ((SiblingIndex * sizePage.y) + ((DynamicPositions[choosen].y * -1.0f) + (sizePage.y / 2))) * -1.0f;
            myMenu.transform.localPosition = DynamicPositions[choosen];
        }
    }

    public MenuAnimationStates GetAnimationState()
    {
        return radialMenu.myState;
    }

    void Update()
    {
        switch (radialMenu.myState)
        {

            case MenuAnimationStates.STATIC:

                radialMenu.circleInside.transform.localScale = new Vector3(MAX_GROW_CIRCLE_INSIDE, MAX_GROW_CIRCLE_INSIDE, MAX_GROW_CIRCLE_INSIDE);
                radialMenu.circleOutside.transform.localScale = new Vector3(MAX_GROW_CIRCLE_OUTSIDE, MAX_GROW_CIRCLE_OUTSIDE, MAX_GROW_CIRCLE_OUTSIDE);
                
                radialMenu.imageWhole.transform.localScale = new Vector3(1, 1, 1);
                radialMenu.backgroundImage.transform.localScale = new Vector3(1, 1, 1);

                radialMenu.imageWhole.transform.localScale = new Vector3(2.75f, 2.75f, 2.75f);
                radialMenu.backgroundImage.transform.localScale = new Vector3(2.75f, 2.75f, 2.75f);

                radialMenu.rtElements.transform.localScale = new Vector3(1, 1, 1);
                radialMenu.central.transform.localScale = new Vector3(MAX_GROW_CENTRAL, MAX_GROW_CENTRAL, MAX_GROW_CENTRAL);

                radialMenu.ShadowImage.rectTransform.sizeDelta = new Vector3(radialMenu.backgroundImage.rectTransform.sizeDelta.x + 15, radialMenu.backgroundImage.rectTransform.sizeDelta.y + 15, 1);
                radialMenu.ShadowImage.transform.localScale = radialMenu.backgroundImage.transform.localScale;

                radialMenu.myState = MenuAnimationStates.STOPPED;

                break;

            case MenuAnimationStates.GROWING_CIRCLES:

                if (radialMenu.circleInside.transform.localScale.x < MAX_GROW_CIRCLE_INSIDE)
                radialMenu.circleInside.transform.localScale = new Vector3(radialMenu.circleInside.transform.localScale.x + SPEED, radialMenu.circleInside.transform.localScale.y + SPEED, radialMenu.circleInside.transform.localScale.z + SPEED);

                if (radialMenu.circleOutside.transform.localScale.x < MAX_GROW_CIRCLE_OUTSIDE)
                radialMenu.circleOutside.transform.localScale = new Vector3(radialMenu.circleOutside.transform.localScale.x + SPEED, radialMenu.circleOutside.transform.localScale.y + SPEED, radialMenu.circleOutside.transform.localScale.z + SPEED);

                if ((radialMenu.circleInside.transform.localScale.x >= MAX_GROW_CIRCLE_INSIDE) && (radialMenu.circleOutside.transform.localScale.x >= MAX_GROW_CIRCLE_OUTSIDE))
                {
                    radialMenu.circleInside.transform.localScale = new Vector3(MAX_GROW_CIRCLE_INSIDE, MAX_GROW_CIRCLE_INSIDE, MAX_GROW_CIRCLE_INSIDE);
                    radialMenu.circleOutside.transform.localScale = new Vector3(MAX_GROW_CIRCLE_OUTSIDE, MAX_GROW_CIRCLE_OUTSIDE, MAX_GROW_CIRCLE_OUTSIDE);
                    radialMenu.myState = MenuAnimationStates.GROWING_PETALS;

                    radialMenu.imageWhole.transform.localScale = new Vector3(1, 1, 1);
                    radialMenu.backgroundImage.transform.localScale = new Vector3(1, 1, 1);
                }

                break;

            case MenuAnimationStates.GROWING_PETALS:

                if (radialMenu.imageWhole.transform.localScale.x < 2.75f)
                    radialMenu.imageWhole.transform.localScale = new Vector3(radialMenu.imageWhole.transform.localScale.x + SPEED, radialMenu.imageWhole.transform.localScale.y + SPEED, radialMenu.imageWhole.transform.localScale.z + SPEED);

                if (radialMenu.backgroundImage.transform.localScale.x < 2.75f)
                    radialMenu.backgroundImage.transform.localScale = new Vector3(radialMenu.backgroundImage.transform.localScale.x + SPEED, radialMenu.backgroundImage.transform.localScale.y + SPEED, radialMenu.backgroundImage.transform.localScale.z + SPEED);

                if ((radialMenu.imageWhole.transform.localScale.x >= 2.75f) && (radialMenu.backgroundImage.transform.localScale.x >= 2.75f))
                {
                    radialMenu.imageWhole.transform.localScale = new Vector3(2.75f, 2.75f, 2.75f);
                    radialMenu.backgroundImage.transform.localScale = new Vector3(2.75f, 2.75f, 2.75f);

                    radialMenu.myState = MenuAnimationStates.FINISH;
                }

                break;

            case MenuAnimationStates.FINISH:

                radialMenu.rtElements.transform.localScale = new Vector3(1,1,1);
                radialMenu.central.transform.localScale = new Vector3(MAX_GROW_CENTRAL, MAX_GROW_CENTRAL, MAX_GROW_CENTRAL);

                radialMenu.ShadowImage.rectTransform.sizeDelta = new Vector3(radialMenu.backgroundImage.rectTransform.sizeDelta.x + 15, radialMenu.backgroundImage.rectTransform.sizeDelta.y + 15, 1);
                radialMenu.ShadowImage.transform.localScale = radialMenu.backgroundImage.transform.localScale;


                radialMenu.myState = MenuAnimationStates.STOPPED;
            break;

        }



    }


    void ButtonClicked(int n)
    {

        m_PDFViewer.m_AllowedToPinchZoom = true;
        object[] o = new object[1];
        o[0] = n;
        info.Invoke(callerObject, o);

    }

}
