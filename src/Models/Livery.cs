using System.Xml.Serialization;

namespace VmrGenerator.Models
{
    [XmlType("ModelMatchRule")]
    public class Livery
    {
        [XmlAttribute("CallsignPrefix")]
        public string CallsignPrefix { get; set; }


        [XmlAttribute("FlightNumberRange")]
        public string FlightNumberRange { get; set; }

        [XmlAttribute("TypeCode")]

        public string TypeCode { get; set; }

        [XmlAttribute("ModelName")]

        public string ModelName { get; set; }

        public Livery()
        {
        }
    }
}