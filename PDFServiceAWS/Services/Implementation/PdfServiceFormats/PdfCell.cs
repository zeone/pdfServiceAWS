using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PdfSharp.Drawing;

namespace PDFServiceAWS.Services.Implementation.PdfServiceFormats
{
    public class PdfCell
    {
        public float Left;
        public float Top;
        public float Width;
        public float Height;

        public float TopMargin = 0;
        public float LeftMargin = 0;

        public bool ShowMarkupLines = false;

        public XFont XFont;
    }
}