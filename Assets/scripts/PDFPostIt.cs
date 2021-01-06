using Paroxe.PdfRenderer;
using Paroxe.PdfRenderer.Internal.Viewer;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PDFPostIt : MonoBehaviour{

    public GameObject PrefabEditablePostIt;
    GameObject EditablePostIt;
    public Text textComponent;
    Text textComponentReal;
    public PDFViewer m_PDFViewer;
    public PDFViewerHelper m_PDFViewerHelper;
    public PDFViewerPageHelper m_PDFViewerPageHelper;
    public PDFViewerPage m_PDFViewerPage;
    public PDFPostItAdjust m_PostItAdjust;
    public Image background;
    public Image shadow;
    public Image fake;
    public Image imageExpand;
    public InputField inputfield;
    public Texture2D [] postitcollapsed;
    //public Texture2D[] PostItsBackgrounds;
    public PDFHightlight m_PDFHighlight;
    object callerObject;
    //public int m_offset_x;
    //public int m_offset_y;
    System.Reflection.MethodInfo info;
    //System.Reflection.MethodInfo infoDismiss;
    public Sprite [] backgroundSprites;


    public PDFPostItAdjust DoThePostit(object o, System.Type t, PDFViewerHelper.RectPostIt item, bool add)
    {
        if (m_PDFViewer == null)
        {
            m_PDFViewer = GetComponentInParent<PDFViewer>();
        }

        m_PDFViewerHelper = m_PDFViewer.GetComponent<PDFViewerHelper>();

        info = t.GetMethod("returnFunctionFromPostIt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //infoDismiss = t.GetMethod("returnFunctionFromPostItDismiss", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        callerObject = o;

        EditablePostIt = Instantiate(PrefabEditablePostIt) as GameObject;
        Image [] images = EditablePostIt.GetComponentsInChildren<Image>();

        Outline m_Outline = EditablePostIt.GetComponentInChildren<Outline>();

        shadow = images[0];
        fake = images[1];
        background = images[2];
        imageExpand = images[3];
        //imageConfirm = images[3];


        inputfield = EditablePostIt.GetComponentInChildren<InputField>();

        RectTransform [] rt = inputfield.GetComponentsInChildren<RectTransform>();  

        RectTransform rtMain = EditablePostIt.GetComponent<RectTransform>();
        rtMain.sizeDelta = item.size;

        Text[] texts = inputfield.GetComponentsInChildren<Text>();

        textComponent = inputfield.textComponent;
        textComponent.fontSize = 16;
        textComponent.color = Color.black;

        textComponentReal = texts[2];
        textComponentReal.fontSize = 16;
        textComponentReal.color = Color.black;


        inputfield.text = item.text;

        //m_offset_x = 25;
        //m_offset_y = 25;

        background.rectTransform.sizeDelta = item.size;

        if (item.collapsed == false)
        {
            //shadow.rectTransform.sizeDelta = new Vector2(item.size.x + m_offset_x, item.size.y + m_offset_y);
            shadow.rectTransform.sizeDelta = new Vector2(item.size.x, item.size.y);
            fake.rectTransform.sizeDelta = new Vector2(item.size.x - 2, item.size.y - 2);
        }
        else
        {
            shadow.rectTransform.sizeDelta = Vector2.zero;
            fake.rectTransform.sizeDelta = Vector2.zero;
        }


        Sprite sprb = backgroundSprites[item.indexColor];// Sprite.Create(PostItsBackgrounds[0], new Rect(0, 0, PostItsBackgrounds[item.indexColor].width, PostItsBackgrounds[item.indexColor].height), new Vector2(0.5f, 0.5f));
        background.sprite = sprb;

        textComponent.rectTransform.sizeDelta = item.size;
        textComponentReal.rectTransform.sizeDelta = item.size;

        m_PostItAdjust = EditablePostIt.GetComponentInChildren<PDFPostItAdjust>();

        m_PostItAdjust.m_Outline = m_Outline;

        //m_PostItAdjust.m_offset_x = m_offset_x;
        //m_PostItAdjust.m_offset_y = m_offset_y;

        m_PostItAdjust.info = info;
        m_PostItAdjust.callerObject = callerObject;

        m_PostItAdjust.background = background;
        m_PostItAdjust.fake = fake;

        m_PostItAdjust.shadow = shadow;
        m_PostItAdjust.inputfield = inputfield;

        m_PostItAdjust.textComponent = textComponent;
        m_PostItAdjust.textComponentReal = textComponentReal;

        m_PostItAdjust.rt = rt;
        m_PostItAdjust.rtMain = rtMain;

        //m_PostItAdjust.m_PDFRadialInterface = m_PDFViewerPageHelper.m_PDFRadialInterface;
        m_PostItAdjust.m_PDFRadialInterface = m_PDFViewerHelper.m_PDFRadialInterface;

        m_PostItAdjust.m_PDFViewer = m_PDFViewer;
        m_PostItAdjust.m_PDFViewerHelper = m_PDFViewerHelper;// (PDFViewerPageHelper)o;// m_PDFViewerHelper;
        m_PostItAdjust.page = item.page;
        m_PostItAdjust.postitcollapsed = postitcollapsed;
        m_PostItAdjust.id = item.id;
        m_PostItAdjust.backgroundSprites = backgroundSprites;
        m_PostItAdjust.indexColor = item.indexColor;
        m_PostItAdjust.m_PDFViewerPage = m_PDFViewerPage;
        m_PostItAdjust.m_PDFHighlight = m_PDFHighlight;

        m_PostItAdjust.wholeDocumentSize = m_PDFViewer.m_Internal.m_PageContainer.sizeDelta;

        if (item.collapsed == true)
        {
            m_PostItAdjust.ImageExpand.rectTransform.sizeDelta = new Vector2(0, 0);

            m_PostItAdjust.textComponent.rectTransform.sizeDelta = new Vector2(0, 0);
            m_PostItAdjust.textComponent.transform.localScale = new Vector3(0, 0, 0);

            m_PostItAdjust.textComponentReal.rectTransform.sizeDelta = new Vector2(0, 0);
            m_PostItAdjust.textComponentReal.transform.localScale = new Vector3(0, 0, 0);


            m_PostItAdjust.inputfield.GetComponent<RectTransform>().sizeDelta = new Vector3(0, 0, 0);
            for (int i = 0; i < rt.Length; i++)
                m_PostItAdjust.rt[i].sizeDelta = new Vector2(0, 0);

            m_PostItAdjust.originalSizeBackground = item.size;
            m_PostItAdjust.originalSizeText = item.size;

            Sprite spr = Sprite.Create(postitcollapsed[item.indexColor], new Rect(0, 0, postitcollapsed[item.indexColor].width, postitcollapsed[item.indexColor].height), new Vector2(0.5f, 0.5f));
            background.sprite = spr;

            m_PostItAdjust.background.rectTransform.sizeDelta = new Vector2(m_PostItAdjust.SizeCollapsedPostIt, m_PostItAdjust.SizeCollapsedPostIt);
            m_PostItAdjust.inputfield.interactable = false;
            m_PostItAdjust.collapsed = true;
        }
        else
        {
            m_PostItAdjust.collapsed = false;

            //m_PostItAdjust.ImageExpand.transform.localPosition = new Vector2(item.size.x - m_PostItAdjust.ImageExpand.rectTransform.sizeDelta.x / 2, -item.size.y + m_PostItAdjust.ImageExpand.rectTransform.sizeDelta.y / 2);
            

            //m_PostItAdjust.ImageConfirm.transform.localPosition = new Vector2(item.size.x + m_PostItAdjust.ImageConfirm.rectTransform.sizeDelta.x / 4, m_PostItAdjust.ImageConfirm.rectTransform.sizeDelta.y / 4);
            //imageConfirm.transform.localPosition = new Vector2(item.size.x + imageConfirm.rectTransform.sizeDelta.x / 4, imageConfirm.rectTransform.sizeDelta.y / 4);

            inputfield.transform.localPosition = new Vector2(0, 0);

        }

        float offsetX = 4;
        float offsetY = 6;
        m_PostItAdjust.ImageExpand.transform.localPosition = new Vector2(rtMain.sizeDelta.x - m_PostItAdjust.SizeCorner / 2 - offsetX, -rtMain.sizeDelta.y + m_PostItAdjust.SizeCorner / 2 + offsetY);

        m_PostItAdjust.previousString = m_PostItAdjust.inputfield.text;
        m_PostItAdjust.currentString = m_PostItAdjust.inputfield.text;

        if (item.collapsed == true)
            m_PostItAdjust.rtMain.sizeDelta = new Vector2(m_PostItAdjust.SizeCollapsedPostIt * m_PDFViewer.ZoomFactor, m_PostItAdjust.SizeCollapsedPostIt * m_PDFViewer.ZoomFactor);

        Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(item.page) as Transform;

        EditablePostIt.transform.SetParent(currentPage.parent);

        if (add == true)
            EditablePostIt.transform.localPosition = new Vector3(item.pos.x, currentPage.localPosition.y + item.pos.y);
        else
            EditablePostIt.transform.localPosition = new Vector3(item.pos.x, item.pos.y);
        //            EditablePostIt.transform.localPosition = new Vector3((item.pos.x / item.zoomWhenCreated), currentPage.localPosition.y + (item.pos.y / item.zoomWhenCreated));

        //m_PostItAdjust.itemOriginalPosY = item.pos.y / item.zoomWhenCreated;

        m_PostItAdjust.PrefabEditablePostIt = EditablePostIt;

        if (add == true)
        {
            int id = 0;
            
            if (m_PDFViewerHelper.listOfPostIts.Count > 0)
                id = m_PDFViewerHelper.listOfPostIts[m_PDFViewerHelper.listOfPostIts.Count - 1].id + 1;// id;

            m_PDFViewerHelper.AddPostIt(EditablePostIt.transform.localPosition, background.rectTransform.sizeDelta, item.page, false, textComponent.text, m_PostItAdjust.m_HighlightcolorsP2[0], 0, id, m_PDFViewer.ZoomFactor, m_PostItAdjust);

            m_PostItAdjust.id = id;
        }
        else
        {
            for (int i=0;i< m_PDFViewerHelper.listOfPostIts.Count;i++)
            {
                PDFViewerHelper.RectPostIt it = m_PDFViewerHelper.listOfPostIts[i];
                if (item.id == it.id)
                {
                    it.instanceOfPostIt = m_PostItAdjust;
                    m_PDFViewerHelper.listOfPostIts[i] = it;
                    break;
                }
            }
        }

        m_PostItAdjust.SetFontSize(new Vector2(item.size.x - 20, item.size.y - 64));
        m_PostItAdjust.AdjustScale();

        return m_PostItAdjust;
    }

}
