using System;
using System.Xml.Serialization;

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
