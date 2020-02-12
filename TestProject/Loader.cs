using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.Schema;

namespace TestProject
{
    class Loader
    {
        private XmlDocument doc;
        public bool Load(string path)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream schemaStream = myAssembly.GetManifestResourceStream("TestProject.camouflages.xsd");
            XmlSchema schema = XmlSchema.Read(schemaStream, null);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(schema);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            XmlReader reader = XmlReader.Create(path, settings);
            doc = new XmlDocument();
            try
            {
                doc.Load(reader);
                return true;
            }
            catch (XmlSchemaValidationException)
            {
                return false;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        public XmlNodeList ShipGroup
        {
            get => doc.SelectNodes("/data/ShipGroups/shipGroup");
        }

        public XmlNodeList Camouflage
        {
            get => doc.SelectNodes("/data/Camouflages/camouflage");
        }

        public XmlNodeList ColorScheme
        {
            get => doc.SelectNodes("/data/ColorSchemes/colorScheme");
        }
    }
}
