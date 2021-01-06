using Paroxe.PdfRenderer;
using Paroxe.PdfRenderer.Internal.Viewer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class freeHandControl : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    Vector2 myPoint, sizePage;//, wholeSizePage;
    public Texture2D theTexture;
    public int penSize, eraserSize;
    int indexColor;
    public Color color;
    static Color[] emptyCanvas;
    bool m_Editing;
    public PDFViewer m_PDFViewer;
    private Vector2 curPosition, lastPosition;
    Transform m_currentPage;
    int[] fakeTexture;
    List<Vector2> listOfErasingLine;
    string savingName;
    public bool hasMadeChanges;
    List<int> listOfDeletedLines1;
    List<int> listOfDeletedLines2;

    public enum modes { PAINTING, ERASING };
    modes modeEditing;
    PDFViewerHelper m_PDFViewerHelper;

    static public Color[] cycleColors =
    {
        Color.yellow,
        Color.blue,
        Color.green,
        Color.red
    };

    public struct freeHandItems
    {
        public int page;
        public int items;
        public int color;
        public float xMin, xMax, yMin, yMax;
        public List<Vector2> listOfPoints;
        
        public freeHandItems(int _page, int _items, int _color, float _xMin, float _xMax, float _yMin, float _yMax, List<Vector2>_listOfPoints)
        {
            page=_page;
            items=_items;
            color=_color;
            xMin=_xMin;
            xMax=_xMax;
            yMin=_yMin;
            yMax = _yMax;
            listOfPoints = _listOfPoints;
        }
    }

    public struct CLines
    {
        public List<Vector2> listOfPoints;
        public Color color;
        public float xMin, xMax, yMin, yMax;
    }

    List<CLines> myColectionOfLines;

    public void setLinesFromLoad(CLines item)
    {
        if (myColectionOfLines == null)
        {
            myColectionOfLines = new List<CLines>();
            myColectionOfLines.Clear();
        }

        myColectionOfLines.Add(item);
        updatePage();

    }

    public void setHelper(PDFViewerHelper vh)
    {
        m_PDFViewerHelper = vh;
    }

    public void setEditing(bool b)
    {
        m_Editing = b;
    }

    public void setMode(modes m)
    {
        modeEditing = m;
    }

    public void toggleMode()
    {
        if (modeEditing == modes.PAINTING)
            modeEditing = modes.ERASING;
        else
            modeEditing = modes.PAINTING;
    }

    public modes getMode()
    {
        return modeEditing;
    }

    public void cycleColor()
    {
        indexColor++;
        if (indexColor > 3)
            indexColor = 0;

        color = cycleColors[indexColor];
    }

    public int getIndexColor()
    {
        return indexColor;
    }


    public void init(Transform _currentPage)
    {
        m_currentPage = _currentPage;

        myColectionOfLines = new List<CLines>();
        myColectionOfLines.Clear();

        sizePage = GetComponentInParent<RectTransform>().sizeDelta;
        //wholeSizePage = sizePage;

        int t = theTexture.width * theTexture.height;// (int)sizePage.x * (int)sizePage.y;

        emptyCanvas = new Color[t];
        for (int i = 0; i < t; i++)
        {
            emptyCanvas[i] = new Color(1, 1, 1, 0);
        }

        fakeTexture = new int[theTexture.width * theTexture.height];

        clean();

        hasMadeChanges = false;
    }


    void Start()
    {

        m_PDFViewer = GetComponentInParent<PDFViewer>();
        

        sizePage = GetComponentInParent<RectTransform>().sizeDelta;
        //wholeSizePage = sizePage;
        color = Color.yellow;
        //m_Threshold = 10;
        //m_TouchThreshold = 150;

        modeEditing = modes.PAINTING;
        indexColor = 0;

        myPoint = Vector2.zero;

        curPosition = Vector2.zero;
        lastPosition = Vector2.zero;

        listOfErasingLine = new List<Vector2>();
        listOfErasingLine.Clear();

        listOfDeletedLines1 = new List<int>();
        listOfDeletedLines1.Clear();

        listOfDeletedLines2 = new List<int>();
        listOfDeletedLines2.Clear();

        savingName = m_PDFViewer.DataBuffer.Length.ToString();

    }

    public void clean()
    {
        if (emptyCanvas != null)
        {
            theTexture.SetPixels(emptyCanvas);
            theTexture.Apply();
        }
        Array.Clear(fakeTexture, 0, fakeTexture.Length);
    }

    Vector2[] InterpolatePositions(Vector2 startPosition, Vector2 endPosition)
    {
        float units = Vector2.Distance(startPosition, endPosition);

        if (units == 0)
            units = 1;

        Vector2[] vec2Arr = new Vector2[(int)units + 1];

        float counter = 0.0f;

        while (counter <= units)
        {
            vec2Arr[(int)counter] = Vector2.Lerp(startPosition, endPosition, counter / units);
            counter += 1.0f;
        }
        return vec2Arr;
    }

    public void Brush(int cx, int cy, Color col, int line)
    {
        Color c1 = new Color(col.r, col.g, col.b, 1);
        Color c08 = new Color(col.r, col.g, col.b, 0.8f);
        Color c05 = new Color(col.r, col.g, col.b, 0.5f);
        Color c02 = new Color(col.r, col.g, col.b, 0.2f);
        Color c09 = new Color(col.r, col.g, col.b, 0.9f);
        Color c07 = new Color(col.r, col.g, col.b, 0.7f);
        Color c04 = new Color(col.r, col.g, col.b, 0.4f);
        Color c01 = new Color(col.r, col.g, col.b, 0.1f);
        Color c06 = new Color(col.r, col.g, col.b, 0.6f);
        Color c03 = new Color(col.r, col.g, col.b, 0.3f);

        SetPixel(cx, cy, c1, line);

        SetPixel(cx - 1, cy, c1, line);
        SetPixel(cx - 2, cy, c08, line);
        SetPixel(cx - 3, cy, c05, line);
        SetPixel(cx - 4, cy, c02, line);

        SetPixel(cx, cy - 1, c1, line);
        SetPixel(cx, cy - 2, c08, line);
        SetPixel(cx, cy - 3, c05, line);
        SetPixel(cx, cy - 4, c02, line);

        SetPixel(cx - 1, cy - 1, c09, line);
        SetPixel(cx - 2, cy - 1, c07, line);
        SetPixel(cx - 3, cy - 1, c04, line);
        SetPixel(cx - 4, cy - 1, c01, line);

        SetPixel(cx - 1, cy - 1, c09, line);
        SetPixel(cx - 1, cy - 2, c07, line);
        SetPixel(cx - 1, cy - 3, c04, line);
        SetPixel(cx - 1, cy - 4, c01, line);

        SetPixel(cx - 2, cy - 2, c06, line);
        SetPixel(cx - 3, cy - 2, c03, line);

        SetPixel(cx - 2, cy - 2, c06, line);
        SetPixel(cx - 2, cy - 3, c03, line);



        cx += 1;
        SetPixel(cx, cy, c1, line);
        SetPixel(cx + 1, cy, c1, line);
        SetPixel(cx + 2, cy, c08, line);
        SetPixel(cx + 3, cy, c05, line);
        SetPixel(cx + 4, cy, c02, line);

        SetPixel(cx, cy - 1, c1, line);
        SetPixel(cx, cy - 2, c08, line);
        SetPixel(cx, cy - 3, c05, line);
        SetPixel(cx, cy - 4, c02, line);

        SetPixel(cx + 1, cy - 1, c09, line);
        SetPixel(cx + 2, cy - 1, c07, line);
        SetPixel(cx + 3, cy - 1, c04, line);
        SetPixel(cx + 4, cy - 1, c01, line);

        SetPixel(cx + 1, cy - 1, c09, line);
        SetPixel(cx + 1, cy - 2, c07, line);
        SetPixel(cx + 1, cy - 3, c04, line);
        SetPixel(cx + 1, cy - 4, c01, line);

        SetPixel(cx + 2, cy - 2, c06, line);
        SetPixel(cx + 3, cy - 2, c03, line);

        SetPixel(cx + 2, cy - 2, c06, line);
        SetPixel(cx + 2, cy - 3, c03, line);




        cy += 1;
        SetPixel(cx, cy, c1, line);
        SetPixel(cx + 1, cy, c1, line);
        SetPixel(cx + 2, cy, c08, line);
        SetPixel(cx + 3, cy, c05, line);
        SetPixel(cx + 4, cy, c02, line);

        SetPixel(cx, cy + 1, c1, line);
        SetPixel(cx, cy + 2, c08, line);
        SetPixel(cx, cy + 3, c05, line);
        SetPixel(cx, cy + 4, c02, line);

        SetPixel(cx + 1, cy + 1, c09, line);
        SetPixel(cx + 2, cy + 1, c07, line);
        SetPixel(cx + 3, cy + 1, c04, line);
        SetPixel(cx + 4, cy + 1, c01, line);

        SetPixel(cx + 1, cy + 1, c09, line);
        SetPixel(cx + 1, cy + 2, c07, line);
        SetPixel(cx + 1, cy + 3, c04, line);
        SetPixel(cx + 1, cy + 4, c01, line);

        SetPixel(cx + 2, cy + 2, c06, line);
        SetPixel(cx + 3, cy + 2, c03, line);

        SetPixel(cx + 2, cy + 2, c06, line);
        SetPixel(cx + 2, cy + 3, c03, line);



        cx -= 1;
        SetPixel(cx, cy, c1, line);
        SetPixel(cx - 1, cy, c1, line);
        SetPixel(cx - 2, cy, c08, line);
        SetPixel(cx - 3, cy, c05, line);
        SetPixel(cx - 4, cy, c02, line);

        SetPixel(cx, cy + 1, c1, line);
        SetPixel(cx, cy + 2, c08, line);
        SetPixel(cx, cy + 3, c05, line);
        SetPixel(cx, cy + 4, c02, line);

        SetPixel(cx - 1, cy + 1, c09, line);
        SetPixel(cx - 2, cy + 1, c07, line);
        SetPixel(cx - 3, cy + 1, c04, line);
        SetPixel(cx - 4, cy + 1, c01, line);

        SetPixel(cx - 1, cy + 1, c09, line);
        SetPixel(cx - 1, cy + 2, c07, line);
        SetPixel(cx - 1, cy + 3, c04, line);
        SetPixel(cx - 1, cy + 4, c01, line);

        SetPixel(cx - 2, cy + 2, c06, line);
        SetPixel(cx - 3, cy + 2, c03, line);

        SetPixel(cx - 2, cy + 2, c06, line);
        SetPixel(cx - 2, cy + 3, c03, line);




    }

    private void SetPixel(int cx, int cy, Color col, int line)
    {
        if ((cx >= 0) && (cx < theTexture.width) && (cy < 0) && (cy > -theTexture.height))
        {
            Color c = theTexture.GetPixel(cx, cy);

            float iAlpha = 1 - col.a;

            float rOut = (col.r * col.a) + (c.r * iAlpha);
            float gOut = (col.g * col.a) + (c.g * iAlpha);
            float bOut = (col.b * col.a) + (c.b * iAlpha);
            float aOut = col.a + (c.a * iAlpha);

            c = new Color(rOut, gOut, bOut, aOut);

            theTexture.SetPixel(cx, cy, c);

            if ((line!=-1) && (cx >= 0) && (cx < theTexture.width) && (-cy > 0) && (-cy < theTexture.height))
                fakeTexture[(-cy * theTexture.width) + cx] = line;

        }

    }

    /*
    void OnGUI()
    {
        if (m_PDFViewer.CurrentPageIndex == 0)
            GUI.TextArea(new Rect(20, 50, 400, 32), curPosition.ToString()+", "+sizePage.ToString());
    }
    */
        /*
        float SortByDistance(Vector2 p, Vector2 f)
        {
            return GetSqrDist(p, f);
        }

        public float GetSqrDist(Vector2 a, Vector2 b)
        {
            Vector3 vector = new Vector2(a.x - b.x, a.y - b.y);
            return vector.sqrMagnitude;
        }

        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 relativePoint = point - lineStart;
            Vector3 lineDirection = lineEnd - lineStart;
            float length = lineDirection.magnitude;
            Vector3 normalizedLineDirection = lineDirection;
            if (length > .000001f)
                normalizedLineDirection /= length;

            float dot = Vector3.Dot(normalizedLineDirection, relativePoint);
            dot = Mathf.Clamp(dot, 0.0F, length);

            return lineStart + normalizedLineDirection * dot;
        }

        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
        }
        */

    void Update()
    {
        
        if (m_Editing == true)
        {

            //if ((Input.GetMouseButton(0)) && (myPoint.x >= 0) && (myPoint.x < sizePage.x) && (myPoint.y < 0) && (myPoint.y > -sizePage.y))
            {

                if (modeEditing == modes.PAINTING)
                {
                    
                    Vector2 pixelUV = myPoint;

                    curPosition = myPoint;

                    if (lastPosition != Vector2.zero)
                    {

                        Vector2[] pixelArr = InterpolatePositions(lastPosition, curPosition);
                        for (int i = 0; i < pixelArr.Length; i++)
                        {
                            Brush((int)pixelArr[i].x, (int)pixelArr[i].y, color, myColectionOfLines.Count);
                        }
                        
                        theTexture.Apply(false);
                    }
                    lastPosition = pixelUV;

                    if ((myPoint != Vector2.zero))
                    {

                        CLines item = myColectionOfLines[myColectionOfLines.Count - 1];

                        if ((item.listOfPoints.Count == 0) || ((item.listOfPoints.Count > 0) && (myPoint != item.listOfPoints[item.listOfPoints.Count - 1])))
                        {
                            item.listOfPoints.Add(myPoint);

                            if (myPoint.x < item.xMin)
                                item.xMin = myPoint.x;

                            if (myPoint.x > item.xMax)
                                item.xMax = myPoint.x;

                            if (myPoint.y < item.yMin)
                                item.yMin = myPoint.y;

                            if (myPoint.y > item.yMax)
                                item.yMax = myPoint.y;

                            myColectionOfLines[myColectionOfLines.Count - 1] = item;
                        }
                    }
                }
                else//erasing
                {

                    ////////////////Paint first a magenta line
                    Vector2 pixelUV = myPoint;

                    curPosition = myPoint;
                    bool k = false;
                    bool superK = false;
                    if (lastPosition != Vector2.zero)
                    {
                        k = false;
                        Vector2[] pixelArr = InterpolatePositions(lastPosition, curPosition);

                        listOfDeletedLines1.Clear();
                        listOfDeletedLines2.Clear();

                        for (int i = 0; i < pixelArr.Length; i++)
                        {
                            Brush((int)pixelArr[i].x, (int)pixelArr[i].y, Color.magenta, -1);
                            listOfErasingLine.Add(pixelArr[i]);

                            k = checkToErase(pixelArr[i]);
                            if (k == true)
                                superK = k;
                        }
                        if (superK == true)
                        {

                            listOfDeletedLines1.Sort((a, b) => -1 * a.CompareTo(b));
                            listOfDeletedLines2.Sort((a, b) => -1 * a.CompareTo(b));

                            for (int i = 0;i < listOfDeletedLines1.Count; i++)
                            {
                                m_PDFViewerHelper.listOfFreehandItems.RemoveAt(listOfDeletedLines1[i]);
                            }
                            for (int i = 0; i < listOfDeletedLines2.Count; i++)
                            {
                                myColectionOfLines.RemoveAt(listOfDeletedLines2[i]);
                            }
                            updatePage();
                        }

                        theTexture.Apply(false);
                    }
                    lastPosition = pixelUV;

                    if ((myPoint != Vector2.zero))
                    {

                        CLines item = myColectionOfLines[myColectionOfLines.Count - 1];

                        if ((item.listOfPoints.Count == 0) || ((item.listOfPoints.Count > 0) && (myPoint != item.listOfPoints[item.listOfPoints.Count - 1])))
                        {
                            item.listOfPoints.Add(myPoint);

                            if (myPoint.x < item.xMin)
                                item.xMin = myPoint.x;

                            if (myPoint.x > item.xMax)
                                item.xMax = myPoint.x;

                            if (myPoint.y < item.yMin)
                                item.yMin = myPoint.y;

                            if (myPoint.y > item.yMax)
                                item.yMax = myPoint.y;

                            myColectionOfLines[myColectionOfLines.Count - 1] = item;
                        }
                    }

                }

            }
            //else
            //{
                //lastPosition = Vector2.zero;
            //    m_Editing = false;
            //}
        }
    }


    bool checkToErase(Vector2 v2)
    {

        bool ret = false;

        CLines itemx = myColectionOfLines[myColectionOfLines.Count - 1];

        int index = (int)((((int)(Mathf.Abs(v2.y)) * theTexture.width) + (int)v2.x));
        if (index < fakeTexture.Length)
        {
            int c = fakeTexture[(int)((((int)(Mathf.Abs(v2.y)) * theTexture.width) + (int)v2.x))];

            if (c != 0)
            {
                c--;

                if ((c >= 0) && (c < myColectionOfLines.Count))
                {
                    CLines cl = myColectionOfLines[c];

                    int whichGlobal = -1;
                    for (int i = 0; i < m_PDFViewerHelper.listOfFreehandItems.Count; i++)
                    {
                        freeHandControl.freeHandItems cl2 = m_PDFViewerHelper.listOfFreehandItems[i];

                        if ((cl.listOfPoints.Count == cl.listOfPoints.Count) && ((int)cl.xMin == (int)cl2.xMin) && ((int)cl.xMax == (int)cl2.xMax) && ((int)cl.yMin == (int)cl2.yMin) && ((int)cl.yMax == (int)cl2.yMax))
                        {
                            whichGlobal = i;
                            break;
                        }
                    }
                    if (whichGlobal != -1)
                    {
                        if (listOfDeletedLines1.Contains(whichGlobal) == false)
                            listOfDeletedLines1.Add(whichGlobal);

                        if (listOfDeletedLines2.Contains(c) == false)
                            listOfDeletedLines2.Add(c);

                        ret = true;
                    }
                }
                //updatePage();
                //found = true;
            }
        }
        
        return ret;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 point = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_currentPage as RectTransform, eventData.position , eventData.pressEventCamera, out point);

        point.x += (sizePage.x / 2.0f);
        point.y -= (sizePage.y / 2.0f);

        point.x /= sizePage.x;
        point.y /= sizePage.y;

        point.x *= (float)theTexture.width;
        point.y *= (float)theTexture.height;

        myPoint = point;

    }

    void updatePage()
    {

        clean();

        Color color;

        for (int j = 0; j < myColectionOfLines.Count; j++)
        {
            CLines clines = myColectionOfLines[j];
            color = myColectionOfLines[j].color;

            for (int i = 0; i < clines.listOfPoints.Count - 1; i++)
            {
                Vector2[] points = InterpolatePositions(clines.listOfPoints[i], clines.listOfPoints[i + 1]);
                for (int k = 0; k < points.Length; k++)
                {
                    Brush( (int)points[k].x, (int)points[k].y, color, j+1);
                }
            }

        }
        theTexture.Apply(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_Editing == true)
        {
            if (modeEditing == modes.PAINTING)
            {
                curPosition = Vector2.zero;
                lastPosition = Vector2.zero;
                myPoint = Vector2.zero;

                PDFViewerPage p = GetComponentInParent<PDFViewerPage>();

                int currentPage = 0;

                if ((p != null) && (p.m_Page != null))
                {
                    currentPage = p.m_Page.PageIndex;
                }

                int items = myColectionOfLines.Count;

                if (items > 0)
                {
                    freeHandItems newFreeHandItem = new freeHandItems(currentPage, items, indexColor,
                        myColectionOfLines[myColectionOfLines.Count - 1].xMin, myColectionOfLines[myColectionOfLines.Count - 1].xMax,
                        myColectionOfLines[myColectionOfLines.Count - 1].yMin, myColectionOfLines[myColectionOfLines.Count - 1].yMax,
                        myColectionOfLines[myColectionOfLines.Count - 1].listOfPoints);

                    m_PDFViewerHelper.listOfFreehandItems.Add(newFreeHandItem);
                }
            }
            else
            {
                myColectionOfLines.RemoveAt(myColectionOfLines.Count-1);
                curPosition = Vector2.zero;
                lastPosition = Vector2.zero;
                myPoint = Vector2.zero;
            }

            updatePage();
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        hasMadeChanges = true;

        if ((m_Editing == true) && (modeEditing == modes.PAINTING))
        {
            CLines newItem = new CLines();
            newItem.color = color;
            newItem.listOfPoints = new List<Vector2>();
            newItem.listOfPoints.Clear();
            newItem.xMin = int.MaxValue;
            newItem.xMax = int.MinValue;
            newItem.yMin = int.MaxValue;
            newItem.yMax = int.MinValue;

            myColectionOfLines.Add(newItem);
        }

        if ((m_Editing == true) && (modeEditing == modes.ERASING))
        {
            CLines newItem = new CLines();
            newItem.color = Color.magenta;
            newItem.listOfPoints = new List<Vector2>();
            newItem.listOfPoints.Clear();
            newItem.xMin = int.MaxValue;
            newItem.xMax = int.MinValue;
            newItem.yMin = int.MaxValue;
            newItem.yMax = int.MinValue;

            myColectionOfLines.Add(newItem);

            listOfErasingLine.Clear();
        }

        //m_currentPage = GetComponentInParent<RectTransform>();
        //m_currentPage = m_PDFViewer.m_Internal.m_PageContainer.GetChild(0) as Transform;
        sizePage = m_currentPage.GetComponent<RectTransform>().sizeDelta;

    }

    public void Save()
    {
        if (hasMadeChanges == true)
        {
            SaveLoadFreehand.Save(m_PDFViewerHelper.listOfFreehandItems, m_PDFViewer.FileName + "-" + savingName);
        }
    }
}
