using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TestProject
{
    class Loader
    {
        XmlDocument doc;
        public Loader(string path)
        {
            doc = new XmlDocument();
            doc.Load(path);
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
