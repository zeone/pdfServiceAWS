using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PDFServiceAWS.Dto
{
    public sealed class FamilyAddressDto
    {
        /// <summary>
        /// Client state of this DTO model. Default value is [Unchanged], thus
        /// no changes were made to the model. If any property changes, 
        /// then the state changes. 
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("ClientState", Required = Required.Always)]
        public DtoClientState ClientState { get; set; }

        /// <summary>
        /// Record has any Label/Salutation property set
        /// </summary>
        [JsonProperty("HasLabelSal")]
        public string HasLabelSal { get; set; }

        /// <summary>
        /// No additional address properties used
        /// </summary>
        [JsonProperty("IsQuickForm")]
        public bool IsQuickForm { get; set; }

        /// <summary>
        /// ID of the address
        /// </summary>
        [JsonProperty("AddressId")]
        public int AddressId { get; set; }

        /// <summary>
        /// ID of this address type
        /// </summary>
        [JsonProperty("AddressTypeId")]
        public byte AddressTypeId { get; set; }

        /// <summary>
        /// ID of a family this address belongs to
        /// </summary>
        [JsonProperty("FamilyId")]
        public int FamilyId { get; set; }

        /// <summary>
        /// Address line 1 (not required) todo: fix this issue. At least Address Line 1 should be required
        /// </summary>
        [JsonProperty("Address", NullValueHandling = NullValueHandling.Include)]
        public string Address { get; set; }

        /// <summary>
        /// Address line 2 (not required)
        /// </summary>
        [JsonProperty("Address2", NullValueHandling = NullValueHandling.Include)]
        public string Address2 { get; set; }

        /// <summary>
        /// Value of City
        /// </summary>
        [JsonProperty("City", NullValueHandling = NullValueHandling.Include)]
        public string City { get; set; }

        /// <summary>
        /// Value of State
        /// </summary>
        [JsonProperty("State", NullValueHandling = NullValueHandling.Include)]
        public string State { get; set; }

        /// <summary>
        /// Value of Country
        /// </summary>
        [JsonProperty("Country", NullValueHandling = NullValueHandling.Include)]
        public string Country { get; set; }

        /// <summary>
        /// Value of Zip code (postal code)
        /// </summary>
        [JsonProperty("Zip", NullValueHandling = NullValueHandling.Include)]
        public string Zip { get; set; }

        /// <summary>
        /// Value of Plus4 (extension to Zip code)
        /// </summary>
        [JsonProperty("Plus4", NullValueHandling = NullValueHandling.Include)]
        public string Plus4 { get; set; }

        /// <summary>
        /// Value of the Street Name
        /// </summary>
        [JsonProperty("StreetName", NullValueHandling = NullValueHandling.Include)]
        public string StreetName { get; set; }

        /// <summary>
        /// Value of the Street No
        /// </summary>
        [JsonProperty("StreetNo", NullValueHandling = NullValueHandling.Include)]
        public string StreetNo { get; set; }

        /// <summary>
        /// Value of the Street Apt (street apartment)
        /// </summary>
        [JsonProperty("StreetApt", NullValueHandling = NullValueHandling.Include)]
        public string StreetApt { get; set; }

        /// <summary>
        /// Quick note about this address
        /// </summary>
        [JsonProperty("Note", NullValueHandling = NullValueHandling.Include)]
        public string Note { get; set; }

        /// <summary>
        /// This address is marked as the PRIMARY
        /// </summary>
        [JsonProperty("IsPrimary")]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// This address was marked as CURRENT
        /// </summary>
        [JsonProperty("IsCurrent")]
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Property [NoMail] (not available to mail to this address)
        /// </summary>
        [JsonProperty("IsNoMail")]
        public bool IsNoMail { get; set; }

        /// <summary>
        /// Title of the company located at this address
        /// </summary>
        [JsonProperty("Company", NullValueHandling = NullValueHandling.Include)]
        public string Company { get; set; }

        [JsonProperty("AddressLabel")]
        public string AddressLabel
        {
            get
            {
                var res = "";
                if (!string.IsNullOrEmpty(Address)) res += Address;
                if (!string.IsNullOrEmpty(Address2)) res += $" | {Address2}";
                res += " |";
                if (!string.IsNullOrEmpty(City)) res += $" {City}";
                if (!string.IsNullOrEmpty(City) && (!string.IsNullOrEmpty(State) || !string.IsNullOrEmpty(Zip) || !string.IsNullOrEmpty(Country))) res += ",";
                if (!string.IsNullOrEmpty(State)) res += $" {State}";
                if (!string.IsNullOrEmpty(Zip)) res += $" {Zip}";
                if (!string.IsNullOrEmpty(Country)) res += $" {Country}";
                return res;
            }
        }

        #region === EXTRA ADDRESS PROPERTIES ===

        /// <summary>
        /// Label of the family located at this address
        /// </summary>
        [JsonProperty("Label", NullValueHandling = NullValueHandling.Include)]
        public string Label { get; set; }

        /// <summary>
        /// ID of the label setting. (Traditional, Men, Women, Both, Manual)
        /// </summary>
        [JsonProperty("LabelSettingId", NullValueHandling = NullValueHandling.Include)]
        public byte? LabelSettingId { get; set; }

        /// <summary>
        /// Primary male label  
        /// </summary>
        [JsonProperty("HisLabel", NullValueHandling = NullValueHandling.Include)]
        public string HisLabel { get; set; }

        /// <summary>
        /// Primary female label
        /// </summary>
        [JsonProperty("HerLabel", NullValueHandling = NullValueHandling.Include)]
        public string HerLabel { get; set; }

        /// <summary>
        /// Salutation to the family
        /// </summary>
        [JsonProperty("Sal", NullValueHandling = NullValueHandling.Include)]
        public string Salutation { get; set; }

        /// <summary>
        /// Setting ID for the Salutation label (Personal, Formal, Manual)
        /// </summary>
        [JsonProperty("SalSettingId", NullValueHandling = NullValueHandling.Include)]
        public byte? SalSettingId { get; set; }

        /// <summary>
        /// Salutation to the primary male of the family
        /// </summary>
        [JsonProperty("HisSal", NullValueHandling = NullValueHandling.Include)]
        public string HisSalutation { get; set; }

        /// <summary>
        /// Salutation to the primary female of the family
        /// </summary>
        [JsonProperty("HerSal", NullValueHandling = NullValueHandling.Include)]
        public string HerSalutation { get; set; }
        #endregion
    }
}