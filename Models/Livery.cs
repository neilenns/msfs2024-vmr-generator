using System.Xml.Serialization;

namespace vmr_generator.Models
{
    [XmlType("ModelMatchRule")]
    public class Livery
    {
        [XmlAttribute("TypeCode")]

        public string TypeCode { get; set; }
        [XmlAttribute("ModelName")]

        public string ModelName { get; set; }

        public Livery()
        {
        }
    }
}