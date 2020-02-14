using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CamouflageXmlEditor
{
    class Camouflages
    {
        private readonly Dictionary<int, Camouflage> hashedCamouflages;
        private readonly Ships ships;
        public Camouflages(XmlNodeList camos, XmlNodeList groups)
        {
            ships = new Ships(groups);
            hashedCamouflages = new Dictionary<int, Camouflage>();
            foreach (XmlNode camo in camos)
            {
                var x = new Camouflage(camo);
                var h = x.GetHashCode();
                hashedCamouflages.Add(h, x);
            }
        }

        public bool HasColorScheme(int hash)
        {
            return hashedCamouflages[hash].ColorSchemes.Any();
        }

        public Dictionary<int, Camouflage> AssociatedWith(string ship)
        {
            var hashedCamo = new Dictionary<int, Camouflage>();
            foreach (var camo in hashedCamouflages)
            {
                var sim = camo.Value.ShipGroups.Intersect(ships.GroupsOf(ship)).ToList();
                if (sim.Count > 0)
                {
                    hashedCamo.Add(camo.Key, camo.Value);
                }
                if (camo.Value.TargetShips.Exists(s => s == ship))
                {
                    hashedCamo.Add(camo.Key, camo.Value);
                }
            }
            return hashedCamo;
        }
    }

    class Camouflage
    {
        private readonly XmlNode camouflage;
        public Camouflage(XmlNode camouflage)
        {
            this.camouflage = camouflage;
        }

        public string Annotation
        {
            get => camouflage["annotation"].InnerText;
        }

        public string Name
        {
            get => camouflage["name"].InnerText;
        }

        public List<string> ShipGroups
        {
            get
            {
                if (camouflage["shipGroups"] != null)
                {
                    return camouflage["shipGroups"].InnerText.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }

        public List<string> TargetShips
        {
            get
            {
                var targetShips = new List<string>();
                if (camouflage.SelectNodes("targetShip") != null)
                {
                    foreach (XmlNode target in camouflage.SelectNodes("targetShip"))
                    {
                        targetShips.Add(target.InnerText);
                    }
                }
                return targetShips;
            }
        }

        public bool Tiled
        {
            get
            {
                if (camouflage["tiled"] != null)
                {
                    return bool.Parse(camouflage["tiled"].InnerText);
                }
                return false;
            }
        }

        public List<string> ColorSchemes
        {
            get
            {
                var cs = new List<string>();
                if (camouflage.SelectNodes("colorSchemes") != null)
                {
                    foreach (XmlNode node in camouflage.SelectNodes("colorSchemes"))
                    {
                        cs.Add(node.InnerText);
                    }
                }
                return cs;
            }
        }

        public List<string> Textures
        {
            get
            {
                var textures = camouflage["Textures"];
                var usedTextures = new List<string>();
                foreach (XmlNode textureNode in textures.ChildNodes)
                {
                    FilterPaths(ref usedTextures, textureNode.Value);
                    foreach (XmlNode y in textureNode.ChildNodes)
                    {
                        FilterPaths(ref usedTextures, y.Value);
                    }
                }
                usedTextures = usedTextures.Distinct().ToList();
                return usedTextures;
            }
        }

        private void FilterPaths(ref List<string> a, string b)
        {
            if (!string.IsNullOrWhiteSpace(b))
            {
                if (b.Trim().Contains('\\'))
                {
                    a.Add(b.Trim().Split('\\').Last());
                }
                else
                {
                    a.Add(b.Trim().Split('/').Last());
                }
            }
        }
    }
}
