using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.Schema;

namespace CamouflageXmlEditor
{
    class Loader
    {
        private XmlDocument doc;
        public bool Load(string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream schemaStream = assembly.GetManifestResourceStream("CamouflageXmlEditor.camouflages.xsd"))
            {
                try
                {
                    XmlSchema schema = XmlSchema.Read(schemaStream, null);
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.Schemas.Add(schema);
                    settings.ValidationType = ValidationType.Schema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                    using (XmlReader reader = XmlReader.Create(path, settings))
                    {
                        doc = new XmlDocument();
                        doc.Load(reader);
                        return true;
                    }
                }
                catch (XmlException)
                {
                    return false;
                }
                catch (XmlSchemaValidationException)
                {
                    return false;
                }
            }
        }

        public void Save(string filename)
        {
            doc.Save(filename);
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
