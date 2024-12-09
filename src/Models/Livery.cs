namespace VmrGenerator.Models
{
    using System.Xml.Serialization;

    /// <summary>
    /// Information about an individual livery in MSFS2024.
    /// </summary>
    [XmlType("ModelMatchRule")]
    public class Livery
    {
        /// <summary>
        /// Gets or sets the airline the livery is for.
        /// </summary>
        /// <example>ASA for Alaska Airlines liveries.</example>
        [XmlAttribute("CallsignPrefix")]
        public string CallsignPrefix { get; set; }

        /// <summary>
        /// Gets or sets the flight number range for the livery.
        /// </summary>
        /// <example>1200-1300.</example>
        [XmlAttribute("FlightNumberRange")]
        public string FlightNumberRange { get; set; }

        /// <summary>
        /// Gets or sets the type code for the livery.
        /// </summary>
        /// <example>B738.</example>
        [XmlAttribute("TypeCode")]

        public string TypeCode { get; set; }

        /// <summary>
        /// Gets or sets the model name for the livery.
        /// </summary>
        /// <example>FSLTL_CRJ7_ZZZZ.</example>
        [XmlAttribute("ModelName")]

        public string ModelName { get; set; }
    }
}