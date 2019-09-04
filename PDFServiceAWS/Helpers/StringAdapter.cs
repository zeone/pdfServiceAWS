namespace PDFServiceAWS.Helpers
{
    /// <summary>
    /// Class used as a wrapper around a <see cref="string"/> object
    /// </summary>
    public sealed class StringAdapter
    {
        private string _stringValue;

        public StringAdapter(string value)
        {
            _stringValue = value;
        }

        public StringAdapter()
        {
            // empty NEW constructor
        }

        /// <summary>
        /// Value of the internal <seealso cref="string"/> object
        /// </summary>
        public string Value
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }

        public override string ToString()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return _stringValue;
        }
    }
}