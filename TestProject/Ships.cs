using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestProject
{
    class Ships
    {
        private readonly Dictionary<string, List<string>> shipGroups;
        public Ships(XmlNodeList groups)
        {
            shipGroups = new Dictionary<string, List<string>>();
            foreach (XmlNode group in groups)
            {
                var ships = Split(group);
                var name = group["name"].InnerText;
                shipGroups.Add(name, ships);
            }
        }

        public List<string> GroupsOf(string shipName)
        {
            return shipGroups.Where(x => x.Value.Exists(y => y == shipName)).Select(z => z.Key).ToList();
        }

        public List<string> ShipsOf(string groupName)
        {
            if (shipGroups[groupName] != null)
            {
                return shipGroups[groupName];
            }
            else
            {
                return new List<string>();
            }
        }

        public void PrintShipGroups()
        {
            foreach (KeyValuePair<string, List<string>> kv in shipGroups)
            {
                Console.WriteLine(kv.Key);
                kv.Value.ForEach(x => Console.WriteLine("\t{0}", x));
            }
        }

        private List<string> Split(XmlNode group)
        {
            return group["ships"].InnerText.Split(' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
        }

        public List<string> Names
        {
            get
            {
                var y = shipGroups.SelectMany(x => x.Value).Distinct().ToList();
                y.Sort();
                return y;
            }
        }

        public List<string> Groups
        {
            get => shipGroups.Select(x => x.Key).ToList();
        }
    }
}
