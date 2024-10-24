using Laboratory_9;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Laboratory_9
{
    [XmlType("car")]
    public class Car
    {
        public string Model { get; set; }
        [XmlElement(ElementName = "engine")]
        public Engine Motor { get; set; }
        public int Year { get; set; }

        public Car() { }
        public Car(string model, Engine motor, int year)
        {
            Model = model;
            Motor = motor;
            Year = year;
        }

    }

}
