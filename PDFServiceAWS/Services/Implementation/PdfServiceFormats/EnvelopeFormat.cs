using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFServiceAWS.Services.Implementation.PdfServiceFormats
{
    public class EnvelopeFormat
    {
        public PdfCell SenderCell { get; private set; }

        public PdfCell RecipientCell { get; private set; }

        /// <summary>
        /// envelope total page width
        /// </summary>
        public double Width;

        /// <summary>
        /// envelope total page height
        /// </summary>
        public double Height;

        public EnvelopeFormat()
        {
            InitSenderCell();
            InitRecipientCell();

            Width = 720;    // this is equal to 10inches // 684; //9.5f;
            Height = 297;   //4.125f;
        }

        private void InitSenderCell()
        {
            SenderCell = new PdfCell();
            SenderCell.Left = 0f;
            SenderCell.Top = 0f;
            SenderCell.Width = 4.75f;
            SenderCell.Height = 1.375f;
            SenderCell.TopMargin = 0.39f;
            SenderCell.LeftMargin = 0.39f;
        }

        private void InitRecipientCell()
        {
            RecipientCell = new PdfCell();
            RecipientCell.Left = 0.5f;
            RecipientCell.Top = 1.375f;
            RecipientCell.Width = 9f;
            RecipientCell.Height = 2.125f;
            RecipientCell.TopMargin = 0;
            RecipientCell.LeftMargin = 4.25f;
        }
    }

    public class EnvelopeAddress
    {
        public int TransactionId { get; private set; }
        public string SenderText { get; private set; }
        public string RecipientText { get; private set; }

        public EnvelopeAddress(int transId, string senderText, string recipientText)
        {
            TransactionId = transId;
            SenderText = senderText;
            RecipientText = recipientText;
        }
    }
}