using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml;

namespace CamouflageXmlEditor
{
    enum SchemeColor
    {
        BLACK,
        RED,
        GREEN,
        BLUE,
        UI
    }

    class Schemes
    {
        public Schemes(XmlNodeList colorSchemes)
        {
            Scheme = new Dictionary<int, ColorScheme>();
            foreach (XmlNode cs in colorSchemes)
            {
                var csc = new ColorScheme(cs);
                Scheme.Add(csc.GetHashCode(), csc);
            }
        }

        public ColorScheme GetScheme(string name)
        {
            foreach (KeyValuePair<int, ColorScheme> kvp in Scheme)
            {
                if (kvp.Value.Name == name)
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        public Dictionary<int, ColorScheme> Scheme
        {
            get;
        }
    }

    class ColorScheme
    {
        private readonly XmlNode scheme;

        public ColorScheme(XmlNode scheme)
        {
            this.scheme = scheme;
            DefaultBlack = Black;
            DefaultRed = Red;
            DefaultGreen = Green;
            DefaultBlue = Blue;
            DefaultUi = Ui;
        }

        public string Name
        {
            get => scheme["name"].InnerText;
        }

        private Color GetColor(string index)
        {
            if (scheme[index] != null)
            {
                var a = scheme[index].InnerText.Split(' ');
                double[] b = a.Select(double.Parse).ToArray();
                byte[] c = b.Select(x => (byte)Math.Round(x * 255)).ToArray();
                var final = Color.FromArgb(c[3], c[0], c[1], c[2]);
                return final;
            }
            return Color.FromArgb(0, 0, 0, 0);
        }

        private void ModifyColor(string index, Color color)
        {
            var alpha = Calculate(color.A);
            var red = Calculate(color.R);
            var green = Calculate(color.G);
            var blue = Calculate(color.B);
            var floatColor = string.Format("{0} {1} {2} {3}", red, green, blue, alpha);
            scheme[index].InnerText = floatColor;
        }

        private string Calculate(int channelValue)
        {
            double x = (double)channelValue / 255;
            double y = Math.Round(x, 3);
            return string.Format("{0:0.000}", y);
        }

        public Color Black
        {
            get => GetColor("color0");
            set { ModifyColor("color0", value); }
        }
        public Color Red
        {
            get => GetColor("color1");
            set { ModifyColor("color1", value); }
        }
        public Color Green
        {
            get => GetColor("color2");
            set { ModifyColor("color2", value); }
        }
        public Color Blue
        {
            get => GetColor("color3");
            set { ModifyColor("color3", value); }
        }
        public Color Ui
        {
            get => GetColor("colorUI");
            set { ModifyColor("colorUI", value); }
        }

        public Color DefaultBlack { get; }
        public Color DefaultRed { get; }
        public Color DefaultGreen { get; }
        public Color DefaultBlue { get; }
        public Color DefaultUi { get; }
    }
}
