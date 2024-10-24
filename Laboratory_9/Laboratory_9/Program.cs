using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Laboratory_9
{
    class Program
    {
        private static List<Car> myCars = new List<Car>()
            {
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };
        static void Main(string[] args)
        {

            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-= LINQ for A6 model =-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            first();
            Console.WriteLine();
            var fileName = "CarsCollection.xml";
            serialize(fileName);
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= Serialization =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            foreach (var x in myCars)
            {
                Console.WriteLine($" >> Year: {x.Year}, Motor Model: {x.Motor.Model}, Horsepower: {x.Motor.Horsepower}, Displacement: {x.Motor.Displacement}");
            }
            myCars = deserialize(fileName);

            Console.WriteLine();
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= Deserialization =-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            foreach (var x in myCars)
            {
                Console.WriteLine($" >> Year: {x.Year}, Motor Model: {x.Motor.Model}, Horsepower: {x.Motor.Horsepower}, Displacement: {x.Motor.Displacement}");
            }
            Console.WriteLine();
            Xpath(fileName);
            LinqSerialization();
            MyCarsToXHTMLTable();
            ModifyCarsCollectionXML();
        }

        //1. Projekcja elementów kolekcji myCars i grupowanie według engineType (0.5 pkt)
        private static void first()
        {
            var projectedCars = myCars
            .Where(c => c.Model == "A6")
            .Select(c => new
            {
                engineType = c.Motor.Model == "TDI" ? "diesel" : "petrol",
                hppl = (double)c.Motor.Horsepower / c.Motor.Displacement
            });

            var groupedCars = projectedCars.GroupBy(c => c.engineType);

            foreach (var group in groupedCars)
            {
                double avgHppl = group.Average(c => c.hppl);
                Console.WriteLine($">> [INFO] HPPL values for {group.Key}: {string.Join(", ", group.Select(c => $"{c.hppl:N2}"))}");
                Console.WriteLine($"    >> Average: {avgHppl:N2}");
            }
        }
        
        //2. Serializacja i deserlializacja (0.2 + 0.2 pkt)
        private static void serialize(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, fileName);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, myCars);
            }
        }
        private static List<Car> deserialize(String fileName)
        {
            List<Car> list;
            XmlSerializer serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
            using (Stream reader = new FileStream(fileName, FileMode.Open))
            {
                list = (List<Car>)serializer.Deserialize(reader);
            }
            return list;
        }

        //3. XPath - Obliczanie średniej mocy i unikalnych modeli samochodów (0.5 + 0.5 pkt)
        private static void Xpath(String fileName)
        {
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= XPath =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            XElement rootNode = XElement.Load(fileName);
            var countAvarageXPath = "sum(//car/engine[@Model!=\"TDI\"]/Horsepower) div count(//car/engine[@Model!=\"TDI\"]/Horsepower)";
            Console.WriteLine($">> [INFO] 3.1. Average HorsePower (non-TDI engines): {(double)rootNode.XPathEvaluate(countAvarageXPath)}");
            var notduplice = "//car/engine[@Model and not(@Model = preceding::car/engine/@Model)]";
            IEnumerable<XElement> models = rootNode.XPathSelectElements(notduplice);

            Console.WriteLine(">> [INFO] 3.2. Car models without repetition:");
            foreach (var model in models)
            {
                Console.WriteLine("    >> " + model.Attribute("Model").Value);
            }
            Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
        }

        //4. Generowanie pliku XML z kolekcji myCars za pomocą LINQ (0.5 pkt)
        private static void LinqSerialization()
        {
            IEnumerable<XElement> nodes = myCars
                .Select(n =>
                new XElement("car",
                    new XElement("model", n.Model),
                    new XElement("engine",
                        new XAttribute("model", n.Motor.Model),
                        new XElement("displacement", n.Motor.Displacement),
                        new XElement("horsePower", n.Motor.Horsepower)),
                    new XElement("year", n.Year)));
            XElement rootNode = new XElement("cars", nodes);
            rootNode.Save("CarsCollectionLinq.xml");
        }

        //5. Generowanie tabeli XHTML na podstawie kolekcji myCars za pomocą LINQ to XML (1 pkt)
        private static void MyCarsToXHTMLTable()
        {
            IEnumerable<XElement> rows =
                from car in myCars
                select new XElement("tr",
                    new XAttribute("style", "border: 2px solid black"),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.Model),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.Motor.Model),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.Motor.Displacement),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.Motor.Horsepower),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.Year)
                );

            XElement table = new XElement("table",
                new XAttribute("style", "border: 2px double black"),
                rows
            );

            string templatePath = "templateDone.html";
            if (!File.Exists(templatePath))
            {
                CreateTemplateFile(templatePath);
            }

            XElement template = XElement.Load(templatePath);
            XElement body = template.Element("{http://www.w3.org/1999/xhtml}body");
            if (body != null)
            {
                body.Remove();
            }

            body = new XElement("{http://www.w3.org/1999/xhtml}body");
            template.Add(body);

            body.Add(table);
            template.Save("templateDone.html");

        }

        private static void CreateTemplateFile(string templatePath)
        {
            XElement template = new XElement("{http://www.w3.org/1999/xhtml}html",
                new XElement("{http://www.w3.org/1999/xhtml}head",
                    new XElement("{http://www.w3.org/1999/xhtml}title", "My Cars")
                ),
                new XElement("{http://www.w3.org/1999/xhtml}body")
            );

            template.Save(templatePath);
        }

        //6. Modyfikacja dokumentu XML: zmiana nazwy elementu horsePower na hp oraz przekształcenie year na atrybut (0.5 + 0.5 pkt)
        private static void ModifyCarsCollectionXML()
        {
            XDocument doc = XDocument.Load("CarsCollection.xml");

            foreach (XElement car in doc.Root.Elements())
            {
                foreach (XElement field in car.Elements())
                {
                    if (field.Name == "engine")
                    {
                        foreach (XElement engineElement in field.Elements())
                        {
                            if (engineElement.Name == "Horsepower")
                            {
                                engineElement.Name = "hp";
                            }
                        }
                    }
                    else if (field.Name == "Model")
                    {
                        var yearField = car.Element("Year");
                        XAttribute attribute = new XAttribute("Year", yearField.Value);
                        field.Add(attribute);
                        yearField.Remove();
                    }
                }
            }

            doc.Save("CarsCollectionModified.xml");
        }

    }
}
