using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFServiceAWS.Services.Implementation.PdfServiceFormats
{
    public abstract class LabelFormat
    {
        public abstract PdfCell LabelCell { get; }

        public abstract float ColumnSpacing { get; }
        public abstract float RowSpacing { get; }

        public abstract int LabelsPerPage { get; }

        public abstract int CellsCntPerHorizontal { get; }
        public abstract int CellsCntPerVertical { get; }
    }

    /*
    // http://www.bscichicago.com/documents/CreatingAvery5160MailingLabelsSSRS.pdf
    //Top 0.5", Bottom 0.5", Left 0.21975", Right 0.21975" 
    //Size: 2.625" x 1"
    //ColumnSpacing property to 0.14in.
    public class Avery5160Format : LabelFormat
    {
        public override float LeftMarging { get { return 0.21975f; } }
        public override float TopMarging { get { return 0.21975f; } }
        public override float LabelWidth { get { return 2.5935f; } }
        public override float LabelHeight { get { return 1f; } }
        public override float ColumnSpacing { get { return 0.14f; } }
        public override float RowSpacing { get { return 0.04f; } }

        public override int LabelsPerPage { get { return 30; } }
        public override int CellsCntPerHorizontal { get { return 3; } }
        public override int CellsCntPerVertical { get { return 10; } }
    }
    */
    public class Avery5160Format : LabelFormat
    {
        private PdfCell _labelCell;
        public override PdfCell LabelCell
        {
            get
            {
                if (_labelCell == null)
                {
                    _labelCell = new PdfCell();
                    _labelCell.Width = 2.63f;
                    _labelCell.Height = 1f;

                    _labelCell.Left = 0.19f;
                    _labelCell.Top = 0.5f;

                    _labelCell.LeftMargin = 0.1f;
                    _labelCell.TopMargin = 0.1f;

                }
                return _labelCell;
            }
        }

        public override float ColumnSpacing { get { return 0.12f; } }
        public override float RowSpacing { get { return 0; } }

        public override int LabelsPerPage { get { return 30; } }
        public override int CellsCntPerHorizontal { get { return 3; } }
        public override int CellsCntPerVertical { get { return 10; } }
    }
}