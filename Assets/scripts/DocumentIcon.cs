using UnityEngine;

public class DocumentIcon : MonoBehaviour
{
    public Sprite m_documentIcon = null;
    public bool m_isDuplex = false;

    public PDFViewerHelper m_PDFViewerHelper;

    public Sprite Icon
    {
        get { return m_documentIcon; }
        set { m_documentIcon = value; }
    }

    public bool isDuplex
    {
        get { return m_isDuplex; }
        set { m_isDuplex = value; }
    }
    void Start()
    {
        m_PDFViewerHelper.m_IconToShow = Icon;
        m_PDFViewerHelper.m_IsDocumentDuplex = isDuplex;
    }

}