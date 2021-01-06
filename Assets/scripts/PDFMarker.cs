using Paroxe.PdfRenderer;
using Paroxe.PdfRenderer.Internal.Viewer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PDFMarker : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDropHandler, IDragHandler
{

    bool m_FirstTime, m_BeingClicked;
    float m_Ix, m_Iy, m_ZoomFactor;
    int m_Who, pageIndex;
    Vector3 m_PosOther;
    ScrollRect m_Sr;
    Vector2 m_OldPosition, m_NewPosition, m_SizeOther, m_Delta;
    PDFViewerPageHelper m_Caller;
    private PDFViewer m_PDFViewer;
    private PDFPage m_Page;
    Vector2 pageSize;
    PDFViewerHelper m_PDFViewerHelper;

    void Start()
    {
        m_Ix = 0;
        m_Iy = 0;
    }

    public void SetClicked(bool t)
    {
        m_BeingClicked = t;
    }

    void Update()
    {

        if ((m_PDFViewerHelper == null) && (m_PDFViewer != null))
            m_PDFViewerHelper = m_PDFViewer.GetComponent<PDFViewerHelper>();

        if ((m_Caller!=null) && (pageSize.Equals(new Vector2())==true))
        {
            m_PDFViewer = m_Caller.GetComponentInParent<PDFViewer>();

            if (m_Page == null)
            {
                //m_Page = m_PDFViewer.Document.GetPage(transform.parent.GetSiblingIndex());
                
                m_Page = m_PDFViewer.Document.GetPage(pageIndex);
            }

            pageSize = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);
        }

        if (m_BeingClicked == true)
        {
            
            if (m_FirstTime == true)
            {
                m_FirstTime = false;
                m_Ix = 0;
                m_Iy = 0;
                //m_Caller.m_PDFHighlight.UpdateHighlightedBox(m_Ix, m_Iy, m_Who);
                //m_Caller.m_PDFHighlight.UpdateMarkers(true);
                m_PDFViewerHelper.m_PDFHighlight.UpdateHighlightedBox(m_Ix, m_Iy, m_Who);
                m_PDFViewerHelper.m_PDFHighlight.UpdateMarkers(true);
            }
            else
            {
                m_Ix = m_NewPosition.x - m_OldPosition.x;
                m_Iy = m_NewPosition.y - m_OldPosition.y;

                if ((m_Ix != 0) || (m_Iy != 0))
                {
                    if (
                        //((m_Who == 0) && ((m_Caller.m_PDFHighlight.m_MarkerLeft.transform.localPosition.x+m_Ix < m_Caller.m_PDFHighlight.m_MarkerRight.transform.localPosition.x) && (m_Caller.m_PDFHighlight.m_MarkerLeft.transform.localPosition.y+m_Iy > m_Caller.m_PDFHighlight.m_MarkerRight.transform.localPosition.y))) ||
                        //((m_Who == 1) && ((m_Caller.m_PDFHighlight.m_MarkerRight.transform.localPosition.x+m_Ix > m_Caller.m_PDFHighlight.m_MarkerLeft.transform.localPosition.x) && (m_Caller.m_PDFHighlight.m_MarkerRight.transform.localPosition.y+m_Iy < m_Caller.m_PDFHighlight.m_MarkerLeft.transform.localPosition.y)))
                        ((m_Who == 0) && ((m_PDFViewerHelper.m_PDFHighlight.m_MarkerLeft.transform.localPosition.x+m_Ix < m_PDFViewerHelper.m_PDFHighlight.m_MarkerRight.transform.localPosition.x) && (m_PDFViewerHelper.m_PDFHighlight.m_MarkerLeft.transform.localPosition.y+m_Iy > m_PDFViewerHelper.m_PDFHighlight.m_MarkerRight.transform.localPosition.y))) ||
                        ((m_Who == 1) && ((m_PDFViewerHelper.m_PDFHighlight.m_MarkerRight.transform.localPosition.x+m_Ix > m_PDFViewerHelper.m_PDFHighlight.m_MarkerLeft.transform.localPosition.x) && (m_PDFViewerHelper.m_PDFHighlight.m_MarkerRight.transform.localPosition.y+m_Iy < m_PDFViewerHelper.m_PDFHighlight.m_MarkerLeft.transform.localPosition.y)))

                        )
                    {
                        //m_Caller.m_PDFHighlight.UpdateHighlightedBox(m_Ix, m_Iy, m_Who);
                        //m_Caller.m_PDFHighlight.UpdateMarkers(true);
                        m_PDFViewerHelper.m_PDFHighlight.UpdateHighlightedBox(m_Ix, m_Iy, m_Who);
                        m_PDFViewerHelper.m_PDFHighlight.UpdateMarkers(true);

                        m_OldPosition = m_NewPosition;
                    }

                }

            }

        }
        
    }

    public void SetCaller(PDFViewerPageHelper p, int w, int _i)
    {
        m_Caller = p;
        
        m_Who = w;

        pageIndex = _i;
        /*
        if (m_Who == 0)
        {
            m_Caller.m_PDFHighlight.m_PosLeft = transform.localPosition;
        }
        else
        {
            m_Caller.m_PDFHighlight.m_PosRight = transform.localPosition;
        }
        */
    }

    public void SetViewer(ScrollRect _s)
    {
        m_Sr = _s;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_PDFViewer.m_AllowedToPinchZoom = false;

        m_Sr.horizontal = false;
        m_Sr.vertical = false;

        int index = m_Caller.transform.GetSiblingIndex();

        Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(index) as Transform;
        Vector2 pos2 = eventData.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos2, GetComponent<Camera>(), out pos2);
        RectTransform rt2 = currentPage as RectTransform;
        pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
        pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
        pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;

        Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
        Vector2 sizePage = m_Page.GetPageSize();

        m_NewPosition = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));
        m_OldPosition = m_NewPosition;

        m_FirstTime = true;
        m_BeingClicked = true;
        //m_Caller.m_PDFHighlight.SetMarkerUpdating(true);
        m_PDFViewerHelper.m_PDFHighlight.SetMarkerUpdating(true);
        /*
        if (m_Who == 0)
        {
            m_Caller.m_PDFHighlight.m_PosLeft = m_NewPosition;
        }
        else
        {
            m_Caller.m_PDFHighlight.m_PosRight = m_NewPosition;
        }
        */
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_PDFViewer.m_AllowedToPinchZoom = true;
        m_Sr.horizontal = true;
        m_Sr.vertical = true;
        m_BeingClicked = false;
        //m_Caller.m_PDFHighlight.SetMarkerUpdating(false);
        m_PDFViewerHelper.m_PDFHighlight.SetMarkerUpdating(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        m_Sr.horizontal = true;
        m_Sr.vertical = true;
        m_BeingClicked = false;
        //m_Caller.m_PDFHighlight.SetMarkerUpdating(false);
        m_PDFViewerHelper.m_PDFHighlight.SetMarkerUpdating(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Sr.horizontal = false;
        m_Sr.vertical = false;
        m_BeingClicked = true;

        m_NewPosition = eventData.position;// Input.mousePosition;

        int index = m_Caller.transform.GetSiblingIndex();
        
        Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(index) as Transform;
        Vector2 pos2 = eventData.position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos2, GetComponent<Camera>(), out pos2);
        RectTransform rt2 = currentPage as RectTransform;
        pos2 += rt2.sizeDelta.x * 0.5f * Vector2.right;
        pos2 += rt2.sizeDelta.y * 0.5f * Vector2.up;
        pos2 = pos2.x * (rt2.sizeDelta.y / rt2.sizeDelta.x) * Vector2.right + pos2.y * Vector2.up;

        Vector2 pagePoint2 = m_Page.DeviceToPage(0, 0, (int)rt2.sizeDelta.y, (int)rt2.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos2.x, (int)pos2.y);
        Vector2 sizePage = m_Page.GetPageSize();

        m_NewPosition = new Vector2(pagePoint2.x - (sizePage.x / 2), pagePoint2.y - (sizePage.y / 2));

        /*
        if (m_Who == 0)
        {
            m_Caller.m_PDFHighlight.m_PosLeft = m_NewPosition;
        }
        else
        {
            m_Caller.m_PDFHighlight.m_PosRight = m_NewPosition;
        }
        */
    }
}