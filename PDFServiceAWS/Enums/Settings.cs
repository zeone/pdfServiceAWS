using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFServiceAWS.Enums
{
    public static class Settings
    {
        #region family, address settings
        public static readonly string SalSettingId = "SalSettingId";
        public static readonly string LabelSettingId = "LabelSettingId";

        //TODO: implement LabelAnd and SalAnd in Family Settings (values: '&' or 'And')
        public static readonly string LabelAnd = "LabelAnd";
        public static readonly string SalAnd = "SalAnd";

        public static readonly string AddressTypeId = "AddressTypeId";      //1-Home 2- Work
        public static readonly string DefaultCountry = "DefaultCountry";    // United States by Default
        public static readonly string DefaultState = "DefaultState";
        public static readonly string DefaultCity = "DefaultCity";
        #endregion

        public static readonly string DefaultClasses = "DefaultClasses";

        public static readonly string BatchTypeAction = "BatchTypeAction";
        public static readonly string BatchTypeOption = "BatchTypeOption";
        public static readonly string BatchEmptyLetterText = "BatchEmptyLetterText";

        public static readonly string PreviewLetters = "PreviewLetters";
        public static readonly string LabelsFont = "LabelsFont";
        public static readonly string EnvelopesFont = "EnvelopesFont";

        //setting for payment, can be set to Auto/Manual
        public static readonly string AutoCharge = "AutoCharge";

        public static readonly string AutoYahrtzeit = "AutoYahrtzeit";

        //Use for scheduler
        public static readonly string Sunset = "Sunset";
        public static readonly string Nightfall = "Nightfall";
        public static readonly string Timezone = "Timezone";
        public static readonly string DaysOfTheWeek = "DaysOfTheWeek";
        public static readonly string ChargeTime = "ChargeTime";

        //Mail chimp
        public static readonly string MailChimpDomain = "MCDomain";
        public static readonly string MailChimpAPIKey = "MCApiKey";

        //Mail
        public static readonly string MailNonPrimaryMembers = "MailNonPrimaryMembers";
        public static readonly string MailNonPrimaryEmails = "MailNonPrimaryEmails";
        public static readonly string MailNoCallEmails = "MailNCEmails";

        public static readonly string InvoiceText = "InvoiceText";
        public static readonly string MailBody = "MailBody";
        //TODO: Completed mailing addresses
        // Instant Receipt, Receipt Interval, Receipt Output
        //Batches Preview option on/off
        public static readonly string YahrtzeitNotifyDays = "YahrtzeitNotifyDays";

        public static readonly string YahrtzeitLeapYear = "YahrtzeitLeapYear";

        public static readonly string SendEmailOptions = "SendEmailOptions";

        public static readonly string SendGridApiKey = "SendGridApiKey";

        public static readonly string CaptchaScore = "CaptchaScore";

        public static readonly string Currency = "Currency";
    }
}