using Paroxe.PdfRenderer;
using Paroxe.PdfRenderer.Internal.Viewer;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PDFHightlight : MonoBehaviour {

    Vector2 m_PreviousPageSize;
    public GameObject m_MarkerLeft, m_MarkerRight;
    public Vector2 m_SizeMarker;
    float m_OriginalFinalLeft, m_OriginalFinalTop, m_OriginalFinalRight, m_OriginalFinalBottom;
    Color m_ChoosenColor;
    int m_IndexColor, m_pageIndex;
    int m_HighlightedTextToEdit, m_FinalFirstPoint, m_FinalSecondPoint;
    public PDFViewer m_PDFViewer;
    public PDFViewerPageHelper m_PDFViewerPageHelper;
    public PDFPage m_Page;
    public GameObject m_Buttonleftprefab, m_Buttonrightprefab;
    public Texture2D m_Saveprefab;
    public Texture2D m_Colorprefab;
    public PDFPostIt m_PDFPostIt;
    //public Vector3 m_PosLeft, m_PosRight;
    Button buttonLeft, buttonRight;
    Rect [] RectCharIndex;
    string[] CharIndex;
    Rect[] RectCharIndex2;
    string[] CharIndex2;

    static int lastPage=-1;
    public Button m_Prefabbutton;
    public bool m_MarkersOn;
    public Color[] m_HighlightcolorsHH;
    PDFViewerHelper m_PDFViewerHelper;


    public struct orderedLines
    {
        public float averageY;
        public List<int> lines;
    };


    private void Start()
    {
        m_PDFViewerHelper = m_PDFViewer.GetComponent<PDFViewerHelper>();
    }

    public Vector2 GetLeftMarkerPos()
    {
        return m_MarkerLeft.transform.localPosition;
    }
    public Vector2 GetRightMarkerPos()
    {
        return m_MarkerRight.transform.localPosition;
    }

    public int GetHighlightedTextToEdit()
    {
        return m_HighlightedTextToEdit;
    }
    public void SetHighlightedTextToEdit(int h)
    {
        m_HighlightedTextToEdit = h;
    }


    public void DoTheTable(PDFTextPage textPage, int page)
    {
        int c = textPage.CountChars();

        string[] extraEndLine = { "\r", "\n"};

        if (page != lastPage)
        {
            lastPage = page;
            RectCharIndex = new Rect[c+2];
            CharIndex = new string[c+2];

            RectCharIndex2 = new Rect[c+2];
            CharIndex2 = new string[c+2];

            List<orderedLines> lists = new List<orderedLines>();

            orderedLines element = new orderedLines();
            element.lines = new List<int>();
            element.lines.Clear();

            for (int i = 0; i < c + 2; i++)
            {
                RectCharIndex2[i] = new Rect();

                if (i < c)
                {
                    RectCharIndex2[i] = textPage.GetCharBox(i);
                    CharIndex2[i] = textPage.GetChar(i);
                }
                else
                {
                    CharIndex2[i] = extraEndLine[i-c];
                }

                element.averageY += RectCharIndex2[i].y;
                element.lines.Add(i);

                string t = CharIndex2[i];
                if ((t.CompareTo("\n") == 0) || (t.CompareTo("\u0002")==0))// || (i==(c-1)))
                {
                    element.averageY /= element.lines.Count;
                    lists.Add(element);
                    element = new orderedLines();
                    element.lines = new List<int>();
                    element.lines.Clear();
                }
            }

            lists.Sort((p1, p2) => p1.averageY.CompareTo(p2.averageY));

            int cont = 0;
            for (int i= lists.Count - 1; i>=0;i--)
            {
                for (int j=0;j< lists[i].lines.Count;j++)
                {
                    RectCharIndex[cont] = RectCharIndex2[lists[i].lines[j]];
                    CharIndex[cont] = CharIndex2[lists[i].lines[j]];
                    cont++;
                }
                
            }

        }
        
    }


    public void PutMarkers(int pageIndex, int n, Vector2 pageSize, float finalLeft, float finalRight, float finalTop, float finalBottom)
    {
        if (m_PDFViewer == null)
        {
            m_PDFViewer = GetComponentInParent<PDFViewer>();
        }

        if (m_Page == null)
        {
            //m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());
            m_Page = m_PDFViewer.Document.GetPage(pageIndex);
        }

        m_SizeMarker = new Vector2(90, 90);
        m_MarkersOn = true;

        Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(pageIndex) as Transform;
        m_pageIndex = pageIndex;

        finalLeft *= m_PDFViewer.ZoomFactor;
        finalRight *= m_PDFViewer.ZoomFactor;
        finalTop *= m_PDFViewer.ZoomFactor;
        finalBottom *= m_PDFViewer.ZoomFactor;

        finalLeft -= (pageSize.x / 2.0f);
        finalTop -= (pageSize.y / 2.0f);
        finalRight -= (pageSize.x / 2.0f);
        finalBottom -= (pageSize.y / 2.0f);

        finalTop = (currentPage.localPosition.y) + finalTop;
        finalBottom = (currentPage.localPosition.y) + finalBottom;


        m_OriginalFinalLeft = finalLeft;
        m_OriginalFinalTop = finalTop;
        m_OriginalFinalRight = finalRight;
        m_OriginalFinalBottom = finalBottom;

        m_MarkerLeft = Instantiate(m_Buttonleftprefab) as GameObject;
        m_MarkerLeft.transform.SetParent(currentPage.parent);
        m_MarkerLeft.transform.localScale = Vector3.one;
        m_MarkerLeft.transform.localRotation = Quaternion.identity;
        buttonLeft = m_MarkerLeft.GetComponent<Button>();
        buttonLeft.transform.localPosition = new Vector3(finalLeft - (m_SizeMarker.x / 2), finalTop + (m_SizeMarker.y / 2), 0);

        buttonLeft.image.rectTransform.sizeDelta = m_SizeMarker;
        m_MarkerLeft.GetComponent<PDFMarker>().SetViewer(m_PDFViewer.m_Internal.m_ScrollRect);
        m_MarkerLeft.GetComponent<PDFMarker>().SetCaller(m_PDFViewerPageHelper, 0, pageIndex);



        m_MarkerRight = Instantiate(m_Buttonrightprefab) as GameObject;
        m_MarkerRight.transform.SetParent(currentPage.parent);
        m_MarkerRight.transform.localScale = Vector3.one;
        m_MarkerRight.transform.localRotation = Quaternion.identity;
        buttonRight = m_MarkerRight.GetComponent<Button>();
        buttonRight.transform.localPosition = new Vector3(finalRight + (m_SizeMarker.x / 2), finalBottom - (m_SizeMarker.y / 2), 0);

        buttonRight.image.rectTransform.sizeDelta = m_SizeMarker;
        m_MarkerRight.GetComponent<PDFMarker>().SetViewer(m_PDFViewer.m_Internal.m_ScrollRect);
        m_MarkerRight.GetComponent<PDFMarker>().SetCaller(m_PDFViewerPageHelper, 1, pageIndex);

        m_PreviousPageSize = new Vector2(0, 0);

        //UpdateMarkers(true);
    }

    public void UpdateMarkers2(float z)
    {
        if (m_PDFViewer == null)
        {
            m_PDFViewer = GetComponentInParent<PDFViewer>();
        }

        if (m_Page == null)
        {
            //m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());
            m_Page = m_PDFViewer.Document.GetPage(m_pageIndex);
        }

        if ((m_MarkerLeft != null) && (m_MarkerRight != null))
        {

            Vector2 pageSize = m_Page.GetPageSize(z);
            //Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(transform.GetSiblingIndex()) as Transform;
            Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(m_pageIndex) as Transform;

            if ((pageSize.x != m_PreviousPageSize.x) || (pageSize.y != m_PreviousPageSize.y))
            {

                PDFViewerHelper.RectHightlights rh = PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit];

                float finalLeft, finalRight, finalTop, finalBottom;

                finalLeft = rh.rect.xMin * z;
                finalRight = rh.rect.xMax * z;
                finalTop = rh.rect.yMin * z;
                finalBottom = (rh.rect.yMin - rh.rect.height) * z;

                finalLeft -= (pageSize.x / 2.0f);
                finalTop -= (pageSize.y / 2.0f);
                finalRight -= (pageSize.x / 2.0f);
                finalBottom -= (pageSize.y / 2.0f);

                finalTop = (currentPage.localPosition.y) + finalTop;
                finalBottom = (currentPage.localPosition.y) + finalBottom;


                m_OriginalFinalLeft = finalLeft;
                m_OriginalFinalTop = finalTop;
                m_OriginalFinalRight = finalRight;
                m_OriginalFinalBottom = finalBottom;

                buttonLeft.transform.localPosition = new Vector3(finalLeft - (m_SizeMarker.x / 2), finalTop + (m_SizeMarker.y / 2), 0);
                buttonRight.transform.localPosition = new Vector3(finalRight + (m_SizeMarker.x / 2), finalBottom - (m_SizeMarker.y / 2), 0);

            }
        }
    }

    public void UpdateMarkers(bool force)
    {
        if (m_PDFViewer == null)
        {
            m_PDFViewer = GetComponentInParent<PDFViewer>();
        }

        if (m_Page == null)
        {
            //m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());
            m_Page = m_PDFViewer.Document.GetPage(m_pageIndex);
        }

        if ((m_MarkerLeft != null) && (m_MarkerRight != null))
        {

            Vector2 pageSize = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);
            //Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(transform.GetSiblingIndex()) as Transform;
            Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(m_pageIndex) as Transform;

            if ((pageSize.x != m_PreviousPageSize.x) || (pageSize.y != m_PreviousPageSize.y) || (force == true))
            {

                PDFViewerHelper.RectHightlights rh = PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit];

                float finalLeft, finalRight, finalTop, finalBottom;

                finalLeft = rh.rect.xMin * m_PDFViewer.ZoomFactor;
                finalRight = rh.rect.xMax * m_PDFViewer.ZoomFactor;
                finalTop = rh.rect.yMin * m_PDFViewer.ZoomFactor;
                finalBottom = (rh.rect.yMin - rh.rect.height) * m_PDFViewer.ZoomFactor;

                finalLeft -= (pageSize.x / 2.0f);
                finalTop -= (pageSize.y / 2.0f);
                finalRight -= (pageSize.x / 2.0f);
                finalBottom -= (pageSize.y / 2.0f);

                finalTop = (currentPage.localPosition.y) + finalTop;
                finalBottom = (currentPage.localPosition.y) + finalBottom;


                m_OriginalFinalLeft = finalLeft;
                m_OriginalFinalTop = finalTop;
                m_OriginalFinalRight = finalRight;
                m_OriginalFinalBottom = finalBottom;

                buttonLeft.transform.localPosition = new Vector3(finalLeft - (m_SizeMarker.x / 2), finalTop + (m_SizeMarker.y / 2), 0);
                buttonRight.transform.localPosition = new Vector3(finalRight + (m_SizeMarker.x / 2), finalBottom - (m_SizeMarker.y / 2), 0);



            }
        }
    }


    public bool DoTheHightlight(PointerEventData eventData, int pageIndex)
    {
        bool ret = true;

        //if (m_Page == null)
        //{
            m_Page = m_PDFViewer.Document.GetPage(pageIndex);
        //}

        

        using (PDFTextPage textPage = m_Page.GetTextPage())
        {
            DoTheTable(textPage, pageIndex);

            Vector2 pos = eventData.pressPosition;

            Transform currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(pageIndex) as Transform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(currentPage as RectTransform, pos, GetComponent<Camera>(), out pos);
            


            RectTransform rt = currentPage as RectTransform;
            pos += rt.sizeDelta.x * 0.5f * Vector2.right;
            pos += rt.sizeDelta.y * 0.5f * Vector2.up;
            pos = pos.x * (rt.sizeDelta.y / rt.sizeDelta.x) * Vector2.right + pos.y * Vector2.up;
            Vector2 pagePoint = m_Page.DeviceToPage(0, 0, (int)rt.sizeDelta.y, (int)rt.sizeDelta.y, PDFPage.PageRotation.Normal, (int)pos.x, (int)pos.y);

            Vector2 pageSize = m_Page.GetPageSize(m_PDFViewer.ZoomFactor);
            m_PreviousPageSize = pageSize;

            int index = GetCharIndexAtPos(pagePoint, new Vector2(5, 5));
            if (index == -1)
            {
                index = GetClosestCircle(textPage, pagePoint, m_Page.GetPageSize());
            }

            if (index == -1)
            {
                return false;
            }
            else
            {
                m_FinalFirstPoint = index;

                string text = CharIndex[index];// textPage.GetChar(index);
                Rect superRect = RectCharIndex[index];// textPage.GetCharBox(index);

                if (text[0] != 0)
                {
                    int indexPrev = index;
                    string t = CharIndex[index];
                    while ((t[0].CompareTo('\0') != 0) && (t[0].CompareTo('\n') != 0) && (t[0].CompareTo('\r') != 0) && (t[0].CompareTo(' ') != 0))
                    {
                        Rect r = RectCharIndex[index];
                        if (r.xMin < superRect.xMin)
                        {
                            superRect.xMin = r.xMin;
                        }

                        index--;
                        if (index < 0)
                            break;

                        t = CharIndex[index];
                    }

                    m_FinalFirstPoint = index + 1;

                    index = indexPrev;
                    t = CharIndex[index];
                    while ((t[0].CompareTo('\0') != 0) && (t[0].CompareTo('\n') != 0) && (t[0].CompareTo('\r') != 0) && (t[0].CompareTo(' ') != 0))
                    {
                        Rect r = RectCharIndex[index];
                        if (r.xMax > superRect.xMax)
                        {
                            superRect.xMax = r.xMax;
                        }

                        index++;
                        t = CharIndex[index];
                    }
                    m_FinalSecondPoint = index - 1;
                    if (m_FinalSecondPoint < m_FinalFirstPoint)
                    {
                        m_FinalSecondPoint = m_FinalFirstPoint;
                        Rect r = RectCharIndex[m_FinalFirstPoint];
                        superRect.xMax = r.xMax;
                    }

                    if ((m_ChoosenColor.r == 0) && (m_ChoosenColor.g == 0) && (m_ChoosenColor.b == 0) && (m_ChoosenColor.a == 0))
                    {
                        m_ChoosenColor = m_HighlightcolorsHH[0];
                    }
                    /*JJ
                    AddHightlight(superRect, m_Page.PageIndex, m_ChoosenColor);
                    m_HighlightedTextToEdit = listOfHightlights.Count - 1;//the one I have just put...
                    */
                    m_PDFViewerHelper.AddHightlight(superRect, m_Page.PageIndex, m_ChoosenColor);
                    m_HighlightedTextToEdit = PDFViewerHelper.listOfHightlights.Count - 1;//the one I have just put...

                }
                else
                {
                    ret = false;
                }
            }
        }

        return ret;
    }



    public void SaveHightlight()
    {
        /*JJ
        DestroyChildrensOfHighlightedText();
        SaveLoadHighlights.Save(listOfHightlights, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
        */
        DestroyChildrensOfHighlightedText();
        SaveLoadHighlights.Save(PDFViewerHelper.listOfHightlights, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());

    }

    public void CancelHightlight()
    {
        /*JJ
        if (m_HighlightedTextToEdit < listOfHightlights.Count)
        {
            listOfHightlights.RemoveAt(m_HighlightedTextToEdit);
        }

        m_PDFViewer.UpdateRenderer(0.1f);
        DestroyChildrensOfHighlightedText();

        SaveLoadHighlights.Save(listOfHightlights, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
        */
        if (m_HighlightedTextToEdit < PDFViewerHelper.listOfHightlights.Count)
        {
            PDFViewerHelper.listOfHightlights.RemoveAt(m_HighlightedTextToEdit);
        }

        m_PDFViewer.UpdateRenderer(0.1f);
        DestroyChildrensOfHighlightedText();

        SaveLoadHighlights.Save(PDFViewerHelper.listOfHightlights, m_PDFViewer.FileName + "-" + m_PDFViewer.DataBuffer.Length.ToString());
    }

    public int GetCharIndexAtPos(Vector2 v, Vector2 r)
    {
        int ret = -1;
        for (int i = 0;i < RectCharIndex.Length;i++)
        {
            if (((v.x + r.x) < RectCharIndex[i].x) || (v.x > (RectCharIndex[i].x + RectCharIndex[i].width)) || ((v.y + r.y) < RectCharIndex[i].y) || (v.y > (RectCharIndex[i].y + RectCharIndex[i].height)))
            {
            }
            else
            {
                ret = i;
                break;
            }
        }

        return ret;

    }

    public int GetClosestCircle(PDFTextPage textPage, Vector2 v, Vector2 size)
    {
        Vector2 vt = v;
        float R = 1.0f;
        int ret = -1;

        while ((ret == -1) && (vt.x > 0) && (vt.x < size.x) && (vt.y > 0) && (vt.y < size.y))
        {
            for (int i = 0; i < 360; i++)
            {
                vt.x = v.x + R * Mathf.Sin((float)i);
                vt.y = v.y + R * Mathf.Cos((float)i);
                if ((vt.x > 0) && (vt.x < size.x) && (vt.y > 0) && (vt.y < size.y))
                {
                    ret = GetCharIndexAtPos(vt, new Vector2(5, 5));
                    if (ret != -1)
                    {
                        break;
                    }
                }
            }
            R += 1.0f;
        }

        return ret;
    }


    int GetClosest(PDFTextPage textPage, Vector2 v, Vector2 v2, Vector2 size, int who)
    {
        int ret = -1;
        if (who==0)
        {
            for (float t = 0.05f;t < 1.0f;t += 0.05f)
            {
                Vector2 n = Vector2.Lerp(v, v2, t);
                ret = textPage.GetCharIndexAtPos(n, new Vector2(n.x - v.x, v.y - n.y));
                if (ret != -1)
                    break;
            }
        }
        return ret;
    }

    public void SetMarkerUpdating(bool k)
    {
        /*JJ
        if (m_PDFViewer == null)
        {
            m_PDFViewer = GetComponentInParent<PDFViewer>();
        }

        RectHightlights rh = listOfHightlights[m_HighlightedTextToEdit];

        rh.active = k;

        listOfHightlights[m_HighlightedTextToEdit] = rh;

        m_PDFViewer.UpdateRenderer(0.1f);
        */
        if (m_PDFViewer == null)
        {
            m_PDFViewer = GetComponentInParent<PDFViewer>();
        }

        PDFViewerHelper.RectHightlights rh = PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit];

        rh.active = k;

        PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit] = rh;

        m_PDFViewer.UpdateRenderer(0.1f);
    }

    bool IsInside(Rect r1, Rect r2)
    {
        if ((r1.width == 0) || (r1.height == 0))//no characters.... \n \r, etc...
            return false;
        else
        {
            if ((r1.xMin > r2.xMin) && (r1.xMax < r2.xMax) && (r1.y < r2.y) && (r1.y - r1.height > r2.y - r2.height))
                return true;
            else
                return false;
        }
    }


    public bool UpdateHighlightedBox(float ix, float iy, int who)
    {
        
        bool ret = false;
        
        if (m_HighlightedTextToEdit < PDFViewerHelper.listOfHightlights.Count)
        {
            if (m_Page == null)
            {
                //m_Page = m_PDFViewer.Document.GetPage(transform.GetSiblingIndex());
                m_Page = m_PDFViewer.Document.GetPage(m_pageIndex);

            }

            using (PDFTextPage textPage = m_Page.GetTextPage())
            {

                if (m_PDFViewer == null)
                {
                    m_PDFViewer = GetComponentInParent<PDFViewer>();
                }

                PDFViewerHelper.RectHightlights rh = PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit];

                
                if (who == 0)
                {
                    rh.rect = new Rect(rh.rect.xMin + ix, rh.rect.yMin + iy, rh.rect.width - ix, rh.rect.height + iy);
                    int k = m_FinalFirstPoint / 2;
                    for (int i = k;i < RectCharIndex.Length;i++)
                    {
                        if (IsInside(RectCharIndex[i],rh.rect) == true)
                        {
                            m_FinalFirstPoint = i;
                            break;
                        }
                    }

                }
                else if (who == 1)
                {
                    rh.rect = new Rect(rh.rect.xMin, rh.rect.yMin, rh.rect.width + ix, rh.rect.height - iy);
                    int k = RectCharIndex.Length - ((RectCharIndex.Length - m_FinalSecondPoint) / 2);
                    for (int i = k; i > 0; i--)
                    {
                        if (IsInside(RectCharIndex[i], rh.rect) == true)
                        {
                            m_FinalSecondPoint = i;
                            break;
                        }
                    }
                }

                Vector2 ps = m_Page.GetPageSize();
                if ((m_FinalFirstPoint <= m_FinalSecondPoint) && (rh.rect.y < ps.y) && (rh.rect.y - rh.rect.height > 0) && (rh.rect.x > 0) && (rh.rect.x + rh.rect.width < ps.x))
                {
                    rh.innerRects = new List<Rect>();
                    rh.innerRects.Clear();

                    for (int i = m_FinalFirstPoint; i <= m_FinalSecondPoint; i++)
                    {
                        string t = CharIndex[i];

                        Rect superRect = RectCharIndex[i];// textPage.GetCharBox(i);

                        if ((superRect.x > rh.rect.x) && (superRect.x + superRect.width < rh.rect.x + rh.rect.width))
                        {
                            int index = 0;
                            float max = float.MinValue;
                            float ymax = float.MinValue;

                            while ((t[0].CompareTo('\0') != 0) && (t[0].CompareTo('\r') != 0) && (t[0].CompareTo('\n') != 0) && (t[0].CompareTo('\u0002') != 0) && ((i + index) <= m_FinalSecondPoint))
                            {
                                Rect r;

                                r = RectCharIndex[i + index];

                                if (r.height > max)
                                {
                                    max = r.height;
                                }

                                if (r.y > ymax)
                                {
                                    ymax = r.y;
                                }

                                if ((r.x > rh.rect.x + rh.rect.width) || (r.x + r.width > rh.rect.x + rh.rect.width))
                                    break;

                                superRect.xMax = r.xMax;

                                index++;
                                if (i + index > m_FinalSecondPoint)
                                    break;

                                t = CharIndex[i + index];
                            }

                            superRect.height = max;
                            superRect.y = ymax;

                            rh.innerRects.Add(superRect);

                            if ((i + index) >= m_FinalSecondPoint)
                                break;

                            while ((t[0].CompareTo('\0') == 0) || (t[0].CompareTo('\r') == 0) || (t[0].CompareTo('\n') == 0) || (t[0].CompareTo('\u0002') == 0))
                            {
                                index++;
                                if ((i + index) < CharIndex.Length)
                                    t = CharIndex[i + index];

                            }

                            i += index - 1;
                        }

                    }

                    PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit] = rh;
                    m_PDFViewer.UpdateRenderer(0.05f);
                    ret = true;

                    if (who == 0)
                    {
                        m_OriginalFinalLeft += ix;
                        m_OriginalFinalTop += iy;
                    }
                    else
                    {
                        m_OriginalFinalRight += ix;
                        m_OriginalFinalBottom += iy;
                    }

                }

            }

        }
        
        return ret;
    }

    public int GetIndexColor()
    {
        return m_IndexColor;
    }

    public void ChooseColor(int i)
    {
        
        m_IndexColor =i;
        m_ChoosenColor = m_HighlightcolorsHH[m_IndexColor];

        PDFViewerHelper.RectHightlights newH = PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit];
        newH.color = m_ChoosenColor;
        PDFViewerHelper.listOfHightlights[m_HighlightedTextToEdit] = newH;
        m_PDFViewer.UpdateRenderer(0.1f);
        
    }

    public void DestroyChildrensOfHighlightedText()
    {
        if (m_MarkerLeft != null)
        {
            GameObject.Destroy(m_MarkerLeft.gameObject);
        }

        if (m_MarkerRight != null)
        {
            GameObject.Destroy(m_MarkerRight.gameObject);
        }

        m_MarkersOn = false;

    }

}
