using UnityEngine;

namespace Paroxe.PdfRenderer
{
    /// <summary>
    /// Represent a colored rect within a page. This class is used mainly
    /// for search results highlightment. Will maybe used for text selection in
    /// the future.
    /// </summary>
    public struct PDFColoredRect
    {
        public Rect PageRect;
        public Color Color;
        public bool whole;
        public bool updating;

        public PDFColoredRect(Rect pageRect, Color color)
        {
            PageRect = pageRect;
            Color = color;
            whole = false;
            updating = false;
        }

        public PDFColoredRect(Rect pageRect, Color color, bool w, bool u)
        {
            PageRect = pageRect;
            Color = color;
            whole = w;
            updating = u;
        }
    }
}